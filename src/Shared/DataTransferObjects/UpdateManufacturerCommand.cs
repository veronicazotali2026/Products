using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UpdateManufacturerCommand
{
    [Required(ErrorMessage = "Manufacturer name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Description is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Description is 60 characters.")]
    public required string Description { get; init; }
}