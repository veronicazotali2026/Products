using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository;

internal sealed class ProductRepository(RepositoryContext repositoryContext)
	: RepositoryBase<Product>(repositoryContext), IProductRepository
{
	public async Task<Product?> GetProductAsync(Guid productId, bool trackChanges) =>
		await FindByCondition(c => c.Id.Equals(productId), trackChanges)
		.SingleOrDefaultAsync();

	public async Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges) =>
		await FindAll(trackChanges)
			.OrderBy(c => c.Name)
			.ToListAsync();
	
	public async Task<PagedList<Product>> GetProductsAsync(Guid productId,
		ProductParameters productParameters, bool trackChanges)
	{
		var products = await FindByCondition(e => e.Id.Equals(productId), trackChanges)
			.FilterProducts(productParameters.MinPrice, productParameters.MaxPrice)
			.Search(productParameters.SearchTerm)
			.Sort(productParameters.OrderBy)
			.ToListAsync();

		return PagedList<Product>
			.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);
	}
	
	public void CreateProductForManufacturer(Guid manufacturerId, Product product)
	{
		product.ManufacturerId = manufacturerId;
		Create(product);
	}
	
	public void CreateProduct(Product product) => Create(product);
	public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) => await GetByIdsAsync(ids, trackChanges);
	
	public void DeleteProduct(Product product) => Delete(product);
}