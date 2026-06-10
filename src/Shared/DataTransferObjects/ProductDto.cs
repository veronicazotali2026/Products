namespace Shared.DataTransferObjects;

public record ProductDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FullAddress { get; init; }
}
