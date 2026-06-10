using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Product
{
    [Column("ProductId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Product name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Description is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Description is 60 characters.")]
    public string? Description { get; init; }
    
    public decimal? Price { get; set; }
    
    [ForeignKey(nameof(Manufacturer))]
    public Guid ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }
}