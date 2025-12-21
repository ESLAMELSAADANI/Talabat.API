using System.ComponentModel.DataAnnotations;

namespace Talabat.API.DTOs
{
    public class SetDeliveryMethodDTO
    {
            [Required]
            public int DeliveryMethodId { get; set; }
    }
}
