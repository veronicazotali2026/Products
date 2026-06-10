using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Manufacturer
{
    [Column("ManufacturerId")]
    public Guid ManufacturerId { get; set; }

    [Required(ErrorMessage = "Product name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; set; }
    
    public ICollection<Product>? Products { get; set; }
}