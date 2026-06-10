using Entities.Models;
using Entities.Responses;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Products.Services;

public interface IProductService
{
    Task DeleteProductForManufacturerAsync(Guid manufacturerId, Guid id);
    Task<ApiBaseResponse> CreateProductForManufacturerAsync(Guid manufacturerId,
        CreateProductForManufacturerCommand command);
    Task<ApiBaseResponse> GetProductsAsync
        (Guid companyId, ProductParameters employeeParameters, bool trackChanges);
    Task<ApiBaseResponse> GetProductAsync(Guid manufacturerId, Guid productId, bool trackChanges);
    Task UpdateProductForManufacturerAsync(Guid manufacturerId,
        UpdateProductCommand productForUpdate);
    Task<ApiBaseResponse> GetProductForPatchAsync
        (Guid manufacturerId, Guid id, bool proTrackChanges, bool manTrackChanges);

    Task SaveChangesForPatchAsync(ProductDto productToPatch, Product productEntity);
}
