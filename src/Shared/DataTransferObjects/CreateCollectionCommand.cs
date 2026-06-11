namespace Shared.DataTransferObjects;

public record CreateCollectionCommand
{
    public required IEnumerable<ManufacturerDto> ManufacturerDtos { get; init; }
}