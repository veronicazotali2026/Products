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
using Shared.Response;

namespace Products.Presentation.APIS;

[Route("api/manufacturers")]
[ApiController]
[OutputCache(PolicyName = "120SecondsDuration")]
public class ManufacturersController(IServiceManager service, ILogger logger) : CommandHandler
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
        await ExecuteCreateCommandAsync<CreateManufacturerCommand, ManufacturerDto>(
            service.ManufacturerService.CreateManufacturerAsync, command, "ManufacturerById", new CancellationToken());
    // {
    //     var createdCompany = await service.ManufacturerService.CreateManufacturerAsync(command);
    //
    //     return CreatedAtRoute("ManufacturerById", new { id = createdCompany.Id }, createdCompany);
    // }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection
        ([FromBody] IEnumerable<ManufacturerDto> companyCollection)
    {
        var result = await service.ManufacturerService.CreateManufacturerCollectionAsync(companyCollection);

        return CreatedAtRoute("ManufacturerCollection", new { result.ids }, result.companies);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await service.ManufacturerService.DeleteManufacturerAsync(id, trackChanges: false);

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateManufacturerCommand command)
    {
        await service.ManufacturerService.UpdateManufacturerAsync(id, command, trackChanges: true);

        return NoContent();
    }
}