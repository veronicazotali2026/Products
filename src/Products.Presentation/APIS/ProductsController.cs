using System;
using System.Threading;
using System.Threading.Tasks;
using Entities.Extensions;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Products.Presentation.ActionFilters;
using Products.Presentation.Applications;
using Products.Services;
using Shared.DataTransferObjects;
using Serilog;
using Shared.RequestFeatures;
using Shared.Response;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Products.Presentation.APIS;

[Route("api/manufacturers/{manufacturerId}/products")]
public class ProductsController(IServiceManager service) : CommandHandler
{
    [HttpGet]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetProductsForManufacturer(Guid manufacturerId,
        [FromQuery] ProductParameters productParameters)
    {
        var linkParams = new LinkParameters(productParameters, HttpContext);

        var response = await service.ProductService.GetProductsAsync(manufacturerId,
            linkParams, trackChanges: false);

        var result = response.GetResult<(LinkResponse, MetaData)>();
        
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Item2));

        return result.Item1.HasLinks ? Ok(result.Item1.LinkedEntities) :
            Ok(result.Item1.ShapedEntities);
    }

    [HttpGet("{id:guid}", Name = "GetProductForManufacturer")]
    public async Task<IActionResult> GetProductForManufacturer(Guid manufacturerId, Guid id) 
    {
        var product = await service.ProductService.GetProductAsync(manufacturerId, id, trackChanges: false);
        return Ok(product);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateProductForManufacturer
        (Guid companyId, [FromBody] CreateProductForManufacturerCommand command) =>
        await ExecuteCreateCommandAsync<CreateProductForManufacturerCommand, ProductResponse>(
            service.ProductService.CreateProductForManufacturerAsync, command, companyId,"GetProductForManufacturer", CancellationToken.None);

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid manufacturerId, Guid productId) => await ExecuteDeleteCommandAsync(
        service.ProductService.DeleteProductForManufacturerAsync, manufacturerId, productId, CancellationToken.None);

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateProductManufacturer(Guid manufacturerId, [FromBody] UpdateProductCommand cmd) =>
        await ExecuteUpdateCommandAsync(
            service.ProductService.UpdateProductForManufacturerAsync, cmd,  manufacturerId, CancellationToken.None);

    [HttpPatch("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid manufacturerId, Guid id,
        [FromBody] JsonPatchDocument<UpdateProductCommand> cmd)
    {
        var response = await service.ProductService.GetProductForPatchAsync(manufacturerId, id, false, true);

        var result = response.GetResult<(ProductDto,Product)>();
        
        //cmd.ApplyTo(result.Item1);

        TryValidateModel(result.Item1);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await service.ProductService.SaveChangesForPatchAsync(result.Item1, result.Item2);

        return NoContent();
    }
}