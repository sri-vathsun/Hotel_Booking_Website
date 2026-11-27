using System.ComponentModel.DataAnnotations;

namespace StayEase.Api.Models;

public class Category
{
    public int CategoryId { get; set; }
    [Required, StringLength(100)] public string CategoryName { get; set; } = string.Empty;
    [StringLength(250)] public string? Description { get; set; }
    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
