using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.Responses;
using Serilog;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using Shared.Response;
using ProductDto = Shared.DataTransferObjects.ProductDto;

namespace Products.Services;

public class ProductService(IRepositoryManager repository, IProductLinks productLinks) : IProductService
{
    public async Task<ApiBaseResponse> GetProductsAsync
        (Guid manufacturerId, LinkParameters linkParameters, bool trackChanges)
    {
        await CheckIfProductExists(manufacturerId, trackChanges);

        var productsWithMetaData = await repository.Product
            .GetProductsAsync(manufacturerId, linkParameters.ProductParameters, trackChanges);

        var productsDto = new List<ProductDto>();
        var links = productLinks.TryGenerateLinks(productsDto, linkParameters.ProductParameters.Fields!,
            manufacturerId, linkParameters.Context);

        return new ApiOkResponse<(LinkResponse, MetaData)>((links, productsWithMetaData.MetaData));
    }

    public async Task<ApiBaseResponse> GetProductAsync(Guid manufacturerId, Guid id, bool trackChanges)
    {
        await CheckIfProductExists(manufacturerId, trackChanges);

        var productDb = await GetProductForManufacturerAndCheckIfItExists(manufacturerId, trackChanges);
        var productDto = new ProductDto() { Id = productDb.Id, Name = productDb.Name };
        return new ApiOkResponse<ProductDto>(productDto);
    }
    
    public async Task<ApiBaseResponse> CreateProductForManufacturerAsync(Guid manufacturerId,
        CreateProductForManufacturerCommand employeeForCreation)
    {
        await CheckIfProductExists(manufacturerId, false);

        var productEntity = new Product()
        {
            ManufacturerId = manufacturerId,
          //  Name = employeeForCreation.Name,
        };

        repository.Product.CreateProductForManufacturer(manufacturerId, productEntity);
        await repository.SaveAsync();

        //var employeeToReturn = _mapper.Map<ProductDto>(productEntity);

        return new ApiOkResponse<ProductResponse>(new ProductResponse(manufacturerId));
    }

    public async Task DeleteProductForManufacturerAsync(Guid manufacturerId, Guid id)
    {
        await CheckIfProductExists(manufacturerId, false);

        var productDb = await GetProductForManufacturerAndCheckIfItExists(manufacturerId, false);

        repository.Product.DeleteProduct(productDb);
        await repository.SaveAsync();
    }

    public async Task UpdateProductForManufacturerAsync(Guid manufacturerId,
        UpdateProductCommand  productForUpdate)
    {
        await CheckIfProductExists(manufacturerId, false);

        var productDb = await GetProductForManufacturerAndCheckIfItExists(manufacturerId, false);

        //_mapper.Map(employeeForUpdate, employeeDb);
        await repository.SaveAsync();
    }

    public async Task<ApiBaseResponse> GetProductForPatchAsync
        (Guid manufacturerId, Guid id, bool proTrackChanges, bool manTrackChanges)
    {
        await CheckIfProductExists(manufacturerId, manTrackChanges);

        var productDb = await GetProductForManufacturerAndCheckIfItExists(manufacturerId, proTrackChanges);

        //var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);

       // return (employeeToPatch: employeeToPatch, employeeEntity: employeeDb);
       return new ApiOkResponse<(ProductDto,Product)>((new ProductDto(), new Product()));
    }

    public async Task SaveChangesForPatchAsync(ProductDto productToPatch, Product productEntity)
    {
        //_mapper.Map(employeeToPatch, employeeEntity);
        await repository.SaveAsync();
    }
     
    private async Task CheckIfProductExists(Guid productId, bool trackChanges)
    {
        var product = await repository.Product.GetProductAsync(productId, trackChanges);
        if (product is null)
            throw new ProductNotFoundException();
    }
     
     private async Task<Product> GetProductForManufacturerAndCheckIfItExists
         (Guid manufacturerId, bool trackChanges)
     {
         var employeeDb = await repository.Product.GetProductAsync(manufacturerId, trackChanges);
         
         if (employeeDb is null)
             throw new ProductNotFoundException();

         return employeeDb;
     }
}