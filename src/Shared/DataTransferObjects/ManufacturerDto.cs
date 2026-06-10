using Shared.Response;

namespace Shared.DataTransferObjects;

public record ManufacturerDto : BaseResponse
{
    public string? Name { get; init; }
}