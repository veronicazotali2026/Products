using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using Products.Presentation.ActionFilters;
using Products.Presentation.Applications;
using Products.Services;
using Shared.DataTransferObjects;

namespace Products.Presentation.APIS;

[Route("api/manufacturers")]
[ApiController]
[OutputCache(PolicyName = "120SecondsDuration")]
public class ManufacturersController(IServiceManager service) : CommandHandler
{
    [HttpGet("collection/({ids})", Name = "ManufacturerCollection")]
    public async Task<IActionResult> GetCompanyCollection
        ([ModelBinder(BinderType = typeof(ArrayModelBinder<>))] IEnumerable<Guid> ids)
    {
        var companies = await service.ManufacturerService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(companies);
    }
    
    [HttpGet("{id:guid}", Name = "ManufacturerById")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await service.ManufacturerService.GetManufacturerAsync(id, trackChanges: false);

        var etag = $"\"{Guid.NewGuid():n}\"";
        HttpContext.Response.Headers.ETag = etag;

        return Ok(company);
    }
    
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CreateManufacturerCommand command)  =>
        await ExecuteCreateCommandAsync<CreateManufacturerCommand, ManufacturerDto>(service.ManufacturerService.CreateManufacturerAsync, command, "ManufacturerById", new CancellationToken());
    
    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection
        ([FromBody] CreateCollectionCommand cmd) => await ExecuteCreateCommandAsync<CreateCollectionCommand, ManufacturerDto>(
        service.ManufacturerService.CreateManufacturerCollectionAsync, cmd, "ManufacturerCollection", new CancellationToken());

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id) => await ExecuteDeleteCommandAsync(service.ManufacturerService.DeleteManufacturerAsync,id, new CancellationToken());
    
    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateManufacturerCommand cmd)  => await ExecuteUpdateCommandAsync(
        service.ManufacturerService.UpdateManufacturerAsync, cmd, id, CancellationToken.None);
}