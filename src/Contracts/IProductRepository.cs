using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IProductRepository
{
	Task<Product?> GetProductAsync(Guid productId, bool trackChanges);
	void CreateProduct(Product product);
	Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
	Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges);
	Task<PagedList<Product>> GetProductsAsync(Guid productId,
		ProductParameters productParameters, bool trackChanges);

	void DeleteProduct(Product product);

	void CreateProductForManufacturer(Guid manufacturerId, Product product);
}
