using Entities.Responses;
using Shared.DataTransferObjects;

namespace Products.Services;

public interface IManufacturerService
{
    Task<ApiBaseResponse> GetAllManufacturersAsync(bool trackChanges);
    Task<ApiBaseResponse> GetManufacturerAsync(Guid id, bool trackChanges);

    Task<ApiBaseResponse> CreateManufacturerAsync(CreateManufacturerCommand command);

    Task<ApiBaseResponse> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    Task<ApiBaseResponse> CreateManufacturerCollectionAsync(CreateCollectionCommand cmd);

    Task DeleteManufacturerAsync(Guid manufacturerId);

    Task UpdateManufacturerAsync(Guid manufacturerId, UpdateManufacturerCommand command);
}