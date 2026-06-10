using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IManufacturerRepository
{
    Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync(bool trackChanges);

    Task<Manufacturer> GetManufacturerAsync(Guid companyId, bool trackChanges);

   void CreateManufacturer(Manufacturer manufacturer);

   Task<IEnumerable<Manufacturer>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

   void DeleteManufacturer(Manufacturer manufacturer);
}