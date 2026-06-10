using System.Threading.RateLimiting;
using Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi;
using Polly;
using Products.RequestHandlers;
using Products.Services;
using Refit;
using Repository;

namespace Products.Extensions;

public static class ServiceExtensions
{
	public static void ConfigureCors(this IServiceCollection services) =>
		services.AddCors(options =>
		{
			options.AddPolicy("CorsPolicy", builder =>
			builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
		});

	
	public static void ConfigureRepositoryManager(this IServiceCollection services) =>
		services.AddScoped<IRepositoryManager, RepositoryManager>();
	
	public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
		services.AddDbContext<RepositoryContext>(opts =>
			opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

	public static void ConfigureRefitClient(this IServiceCollection services)
	{
		services
			.AddRefitClient<IInventoryServiceClient>()
			.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7061"))
			.AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
			.AddResilienceHandler("SomeResilienceStrategy", resilienceBuilder => // Adds resilience policy named "MyResilienceStrategy"
			{
				// Retry Strategy configuration. This is very basic, but it's a-must when there are critical downstream services.
				resilienceBuilder.AddRetry(new HttpRetryStrategyOptions // Configures retry behavior
				{
					MaxRetryAttempts = 2, // Maximum retries before throwing an exception (default: 3)
					Delay = TimeSpan.FromSeconds(2), // Delay between retries (default: varies by strategy)
					BackoffType = DelayBackoffType.Exponential, // Exponential backoff for increasing delays (default)
					UseJitter = true, // Adds random jitter to delay for better distribution (default: false)
					ShouldHandle = new PredicateBuilder<HttpResponseMessage>() // Defines exceptions to trigger retries
						.Handle<HttpRequestException>() // Includes any HttpRequestException
						.HandleResult(response => !response.IsSuccessStatusCode) // Includes non-successful responses
				});
		
				// Timeout Strategy configuration
				resilienceBuilder.AddTimeout(TimeSpan.FromSeconds(5)); // Sets a timeout limit for requests (throws TimeoutRejectedException)
		
				// Circuit Breaker Strategy configuration
				resilienceBuilder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions // Configures circuit breaker behavior
				{
					// Tracks failures within this time frame
					SamplingDuration = TimeSpan.FromSeconds(10),
		
					// Trips the circuit if failure ratio exceeds this within sampling duration (20% failures allowed)
					FailureRatio = 0.2,
		
					// Requires at least this many successful requests within sampling duration to reset
					MinimumThroughput = 3,
		
					// How long the circuit stays open after tripping
					BreakDuration = TimeSpan.FromSeconds(1),
		
					// Defines exceptions to trip the circuit breaker
					ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
						.Handle<HttpRequestException>() // Includes any HttpRequestException
						.HandleResult(response => !response.IsSuccessStatusCode) // Includes non-successful responses
				});
			});
	}
	
	public static void ConfigureRateLimitingOptions(this IServiceCollection services)
	{
		services.AddRateLimiter(opt =>
		{
			opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
				RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter",
					partition => new FixedWindowRateLimiterOptions
					{
						AutoReplenishment = true,
						PermitLimit = 5,
						QueueLimit = 2,
						QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
						Window = TimeSpan.FromMinutes(1)
					}));
           
			opt.AddPolicy("SpecificPolicy", context =>
				RateLimitPartition.GetFixedWindowLimiter("SpecificLimiter",
					partition => new FixedWindowRateLimiterOptions
					{
						AutoReplenishment = true,
						PermitLimit = 3,
						Window = TimeSpan.FromSeconds(10)
					}));

			opt.OnRejected = async (context, token) =>
			{
				context.HttpContext.Response.StatusCode = 429;

				if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
					await context.HttpContext.Response
						.WriteAsync($"Too many requests. Please try again after {retryAfter.TotalSeconds} second(s).", token);
				else
					await context.HttpContext.Response
						.WriteAsync("Too many requests. Please try again later.", token);
			};
		});
	}
	
	public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "Products API", 
                Version = "v1",
                Description = "Products & Inventory Services"
            });
            s.SwaggerDoc("v2", new OpenApiInfo { Title = "Code Maze API", Version = "v2" });
        });
    }
	
	public static void ConfigureOutputCaching(this IServiceCollection services) => 
		services.AddOutputCache(opt =>
		{
			//opt.AddBasePolicy(bp => bp.Expire(TimeSpan.FromSeconds(10)));
			opt.AddPolicy("120SecondsDuration", p => p.Expire(TimeSpan.FromSeconds(120)));
		});
}
