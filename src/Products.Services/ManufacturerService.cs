using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Entities.Responses;
using Products.Services;
using Serilog;
using Shared.DataTransferObjects;

internal sealed class ManufacturerService : IManufacturerService
{
    private readonly IRepositoryManager _repository;

    public ManufacturerService(IRepositoryManager repository, ILogger logger) => _repository = repository;

    public async Task<ApiBaseResponse> GetAllManufacturersAsync(bool trackChanges)
    {
        var manufacturers = await _repository.Manufacturer.GetAllManufacturersAsync(trackChanges);
        //Create your own mapper
        var manufacturerDtos = new List<ManufacturerDto>(); 
        return new ApiOkResponse<IEnumerable<ManufacturerDto>>(manufacturerDtos);
    }

    public async Task<ApiBaseResponse> GetManufacturerAsync(Guid id, bool trackChanges)
    {
        var manufacturer = await GetManufacturerAsync(id, trackChanges);

        var manufacturerDto = new ManufacturerDto();
        return new ApiOkResponse<ManufacturerDto>(manufacturerDto);
    }

    public async Task<ApiBaseResponse> CreateManufacturerAsync(CreateManufacturerCommand command)
    {
        var manufacturerEntity = new Manufacturer();

        _repository.Manufacturer.CreateManufacturer(manufacturerEntity);
        await _repository.SaveAsync();

        var companyToReturn = new ManufacturerDto();

        return new ApiOkResponse<ManufacturerDto>(companyToReturn);
    }

    public async Task<ApiBaseResponse> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdParametersBadRequestException();

        var manufacturerEntities = await _repository.Manufacturer.GetByIdsAsync(ids, trackChanges);
        if (ids.Count() != manufacturerEntities.Count())
            throw new CollectionByIdsBadRequestException();
        
        //Handle mapping
        var manufacturerDtos = new List<ManufacturerDto>();
        return new ApiOkResponse<IEnumerable<ManufacturerDto>>(manufacturerDtos);
    }

    public Task<ApiBaseResponse> CreateManufacturerCollectionAsync(CreateCollectionCommand cmd)
    {
        throw new NotImplementedException();
    }
    
    public Task UpdateManufacturerAsync(Guid manufacturerId, UpdateManufacturerCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiBaseResponse> CreateManufacturerCollectionAsync
        (IEnumerable<ManufacturerDto> manufacturerCollection)
    {
        if (manufacturerCollection is null)
            throw new CollectionByIdsBadRequestException();

        var manufacturerEntities = new List<Manufacturer>();
        foreach (var company in manufacturerEntities)
        {
            _repository.Manufacturer.CreateManufacturer(company);
        }

        await _repository.SaveAsync();

        var manufacturerCollectionToReturn = new List<ManufacturerDto>();
        var ids = string.Join(",", manufacturerCollectionToReturn.Select(c => c.Id));

        return new ApiOkResponse<(IEnumerable<ManufacturerDto> companies, string ids)>((manufacturerCollectionToReturn, ids));
    }

    public async Task DeleteManufacturerAsync(Guid manufacturerId)
    {
        var manufacturer = await GetManufacturerAndCheckIfItExists(manufacturerId, true);

        _repository.Manufacturer.DeleteManufacturer(manufacturer);
        await _repository.SaveAsync();
    }

    public async Task UpdateManufacturerAsync(Guid manufacturerId,
        UpdateManufacturerCommand command, bool trackChanges)
    {
        var manufacturer = await GetManufacturerAndCheckIfItExists(manufacturerId, trackChanges);
        await _repository.SaveAsync();
    }

    private async Task<Manufacturer> GetManufacturerAndCheckIfItExists(Guid id, bool trackChanges)
    {
        var manufacturer = await _repository.Manufacturer.GetManufacturerAsync(id, trackChanges);
        if (manufacturer is null)
            throw new ManufacturerNotFoundException();

        return manufacturer;
    }
}
