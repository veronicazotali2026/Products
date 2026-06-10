using Entities.Responses;
using Shared.DataTransferObjects;

namespace Products.Services;

public interface IManufacturerService
{
    Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync(bool trackChanges);
    Task<ManufacturerDto> GetManufacturerAsync(Guid id, bool trackChanges);

    ApiBaseResponse CreateManufacturerAsync(CreateManufacturerCommand command);

    Task<IEnumerable<ManufacturerDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    Task<(IEnumerable<ManufacturerDto> companies, string ids)> CreateManufacturerCollectionAsync
        (IEnumerable<ManufacturerDto> manufacturerCollection);

    Task DeleteManufacturerAsync(Guid manufacturerId, bool trackChanges);

    Task UpdateManufacturerAsync(Guid manufacturerId,
        UpdateManufacturerCommand command, bool trackChanges);
}