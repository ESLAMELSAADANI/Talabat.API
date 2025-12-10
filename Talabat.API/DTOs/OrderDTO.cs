using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.API.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string BuyerEmail { get; set; }
        [Required]
        public string BasketId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DeliveryMethodId is required.")]
        public int DeliveryMethodId { get; set; }
        [Required]
        public AddressDTO ShippingAddress { get; set; }
    }
}
