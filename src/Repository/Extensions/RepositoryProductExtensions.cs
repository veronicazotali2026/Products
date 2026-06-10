using Entities.Models;
using System.Linq.Dynamic.Core;

namespace Repository.Extensions;

public static class RepositoryProductExtensions
{
    public static IQueryable<Product> FilterProducts(this IQueryable<Product> employees, uint minPrice, uint maxPrice) =>
        employees.Where(e => (e.Price >= minPrice && e.Price <= maxPrice));

    public static IQueryable<Product> Search(this IQueryable<Product> employees, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return employees.Where(e => e.Name!.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Product> Sort(this IQueryable<Product> employees, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return employees.OrderBy(e => e.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Product>(orderByQueryString);

        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(e => e.Name);

        return employees.OrderBy(orderQuery);
    }
}