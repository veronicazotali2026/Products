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
