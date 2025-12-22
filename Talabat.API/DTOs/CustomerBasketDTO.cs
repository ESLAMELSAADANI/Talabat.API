using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTOs
{
    public class CustomerBasketDTO
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        public List<BasketItemDTO> Items { get; set; } = null!;
        public int? DeliveryMethodId { get; set; }

    }
}
