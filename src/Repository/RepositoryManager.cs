using Contracts;

namespace Repository;

public sealed class RepositoryManager(RepositoryContext repositoryContext) : IRepositoryManager
{
	private readonly Lazy<IProductRepository> _productRepository = new(() => new ProductRepository(repositoryContext));

	public IProductRepository Product => _productRepository.Value;
	public IManufacturerRepository Manufacturer { get; }

	public async Task SaveAsync() => await repositoryContext.SaveChangesAsync();
}