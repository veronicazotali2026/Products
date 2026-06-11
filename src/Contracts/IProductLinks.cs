using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace Contracts;

public interface IProductLinks
{
    LinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto,
        string fields, Guid manufacturerId, HttpContext httpContext);
}
