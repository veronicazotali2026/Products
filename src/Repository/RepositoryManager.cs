using Contracts;

namespace Repository;

public sealed class RepositoryManager(RepositoryContext repositoryContext) : IRepositoryManager
{
	private readonly Lazy<IProductRepository> _productRepository = new(() => new ProductRepository(repositoryContext));
	private readonly Lazy<IManufacturerRepository> _manufacturerRepository = new(() => new ManufacturerRepository(repositoryContext));

	public IProductRepository Product => _productRepository.Value;
	public IManufacturerRepository Manufacturer => _manufacturerRepository.Value;

	public async Task SaveAsync() => await repositoryContext.SaveChangesAsync();
}