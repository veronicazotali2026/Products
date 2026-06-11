using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

internal sealed class ManufacturerRepository(RepositoryContext repositoryContext)
    : RepositoryBase<Manufacturer>(repositoryContext), IManufacturerRepository
{
    public async Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync(bool trackChanges) =>
        await FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToListAsync();
  
    public async Task<Manufacturer> GetManufacturerAsync(Guid companyId, bool trackChanges) =>
        (await FindByCondition(c => c.ManufacturerId.Equals(companyId), trackChanges)
            .SingleOrDefaultAsync())!;

    public void CreateManufacturer(Manufacturer manufacturer) => Create(manufacturer);

    public async Task<IEnumerable<Manufacturer>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
        await FindByCondition(x => ids.Contains(x.ManufacturerId), trackChanges)
            .ToListAsync();

    public void DeleteManufacturer(Manufacturer manufacturer) => Delete(manufacturer);
}