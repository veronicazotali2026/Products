using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Products.Services;
using Serilog;
using Shared.DataTransferObjects;


internal sealed class ManufacturerService : IManufacturerService
{
    private readonly IRepositoryManager _repository;

    public ManufacturerService(IRepositoryManager repository, ILogger logger)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ManufacturerDto>> GetAllManufacturersAsync(bool trackChanges)
    {
        var companies = await _repository.Manufacturer.GetAllManufacturersAsync(trackChanges);

        //var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
        var manufacturerDtos = new List<ManufacturerDto>(); 
        return manufacturerDtos;
    }

    public async Task<ManufacturerDto> GetManufacturerAsync(Guid id, bool trackChanges)
    {
        var manufacturer = await GetManufacturerAsync(id, trackChanges);

        var manufacturerDto = new ManufacturerDto();
        return manufacturerDto;
    }

    public async Task<ManufacturerDto> CreateManufacturerAsync(CreateManufacturerCommand command)
    {
        var manufacturerEntity = new Manufacturer();

        _repository.Manufacturer.CreateManufacturer(manufacturerEntity);
        await _repository.SaveAsync();

        var companyToReturn = new ManufacturerDto();

        return companyToReturn;
    }

    public async Task<IEnumerable<ManufacturerDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdParametersBadRequestException();

        var companyEntities = await _repository.Manufacturer.GetByIdsAsync(ids, trackChanges);
        if (ids.Count() != companyEntities.Count())
            throw new CollectionByIdsBadRequestException();

        //var companiesToReturn = _mapper.Map<IEnumerable<ManufacturerDto>>(companyEntities);

        var manufacturerDtos = new List<ManufacturerDto>();
        return manufacturerDtos;
    }

    public async Task<(IEnumerable<ManufacturerDto> companies, string ids)> CreateManufacturerCollectionAsync
        (IEnumerable<ManufacturerDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CollectionByIdsBadRequestException();

        //var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        var manufacturerEntities = new List<Manufacturer>();
        foreach (var company in manufacturerEntities)
        {
            _repository.Manufacturer.CreateManufacturer(company);
        }

        await _repository.SaveAsync();

        //var companyCollectionToReturn = _mapper.Map<IEnumerable<ManufacturerDto>>(companyEntities);
        var manufacturerCollectionToReturn = new List<ManufacturerDto>();
        var ids = string.Join(",", manufacturerCollectionToReturn.Select(c => c.Id));

        return (companies: manufacturerCollectionToReturn, ids: ids);
    }

    public async Task DeleteManufacturerAsync(Guid manufacturerId, bool trackChanges)
    {
        var manufacturer = await GetManufacturerAndCheckIfItExists(manufacturerId, trackChanges);

        _repository.Manufacturer.DeleteManufacturer(manufacturer);
        await _repository.SaveAsync();
    }

    public async Task UpdateManufacturerAsync(Guid manufacturerId,
        UpdateManufacturerCommand command, bool trackChanges)
    {
        var manufacturer = await GetManufacturerAndCheckIfItExists(manufacturerId, trackChanges);

        //_mapper.Map(companyForUpdate, company);
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
