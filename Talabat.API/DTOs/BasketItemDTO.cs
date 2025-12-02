using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTOs
{
    public class BasketItemDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; } = null!;
        [Required]
        public string PictureUrl { get; set; } = null!;
        [Required]
        [Range(0.1, 9999999, ErrorMessage = "Price Must Be Greater Than Zero.")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Must Be 1 Item At Least.")]
        public int Quantity { get; set; }
        [Required]
        public string Category { get; set; } = null!;
        [Required]
        public string Brand { get; set; } = null!;
    }
}