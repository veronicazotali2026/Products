namespace Contracts;

public interface IRepositoryManager
{
	IProductRepository Product { get; }
	IManufacturerRepository Manufacturer { get; }
	Task SaveAsync();
}