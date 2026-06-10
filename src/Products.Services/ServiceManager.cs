using Contracts;
using Serilog;

namespace Products.Services;

public sealed class ServiceManager(IRepositoryManager repositoryManager,  ILogger logger) : IServiceManager
{
	private readonly Lazy<IProductService> _productService = new(() => new ProductService(repositoryManager, logger));
	
	private readonly Lazy<IManufacturerService> _manufacturerService = new(() => new ManufacturerService(repositoryManager, logger));

	public IProductService ProductService => _productService.Value;
	public IManufacturerService ManufacturerService => _manufacturerService.Value;
}