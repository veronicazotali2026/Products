using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace Products.Hateoas;

public class ProductLinks(LinkGenerator linkGenerator, IDataShaper<ProductDto> dataShaper)
    : IProductLinks
{
    public Dictionary<string, MediaTypeHeaderValue> AcceptHeader { get; set; } = new();

    public LinkResponse TryGenerateLinks(IEnumerable<ProductDto> productsDto, string fields, Guid manufacturerId,
        HttpContext httpContext)
    {
        var shapedProducts = ShapeData(productsDto, fields);

        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkedProducts(productsDto, fields, manufacturerId, httpContext, shapedProducts);

        return ReturnShapedProducts(shapedProducts);
    }

    private List<Entity> ShapeData(IEnumerable<ProductDto> productsDto, string fields)
    {
        return dataShaper.ShapeData(productsDto, fields)
            .Select(e => e.Entity)
            .ToList();
    }

    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"]!;

        return mediaType!.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private LinkResponse ReturnShapedProducts(List<Entity> shapedProducts)
    {
        return new LinkResponse { ShapedEntities = shapedProducts };
    }

    private LinkResponse ReturnLinkedProducts(IEnumerable<ProductDto> productsDto,
        string fields, Guid manufacturerId, HttpContext httpContext, List<Entity> shapedProducts)
    {
        var productDtoList = productsDto.ToList();

        for (var index = 0; index < productDtoList.Count(); index++)
        {
            var productLinks = CreateLinksForProduct(httpContext, manufacturerId, productDtoList[index].Id, fields);
            shapedProducts[index].Add("Links", productLinks);
        }

        var productCollection = new LinkCollectionWrapper<Entity>(shapedProducts);
        var linkedProducts = CreateLinksForProducts(httpContext, productCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedProducts };
    }

    private List<Link>? CreateLinksForProduct(HttpContext httpContext, Guid manufacturerId, Guid id, string fields = "")
    {
        var links = new List<Link>
        {
            new(
                linkGenerator.GetUriByAction(httpContext, "GetProductForManufacturer",
                    values: new { manufacturerId, id, fields }),
                "self",
                "GET"),
            new(
                linkGenerator.GetUriByAction(httpContext, "DeleteProductForManufacturer",
                    values: new { manufacturerId, id }),
                "delete_product",
                "DELETE"),
            new(
                linkGenerator.GetUriByAction(httpContext, "UpdateProductForManufacturer",
                    values: new { manufacturerId, id }),
                "update_product",
                "PUT"),
            new(
                linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateProductForManufacturer",
                    values: new { manufacturerId, id }),
                "partially_update_product",
                "PATCH")
        };
        return links;
    }

    private LinkCollectionWrapper<Entity> CreateLinksForProducts(HttpContext httpContext,
        LinkCollectionWrapper<Entity> productsWrapper)
    {
        productsWrapper.Links.Add(new Link(
            linkGenerator.GetUriByAction(httpContext, "GetProductsForManufacturer", values: new { }),
            "self",
            "GET"));

        return productsWrapper;
    }
}