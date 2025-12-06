using AutoMapper;
using Talabat.API.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Identity;

namespace Talabat.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(dest => dest.Brand, o => o.MapFrom(s => s.Brand.Name))
                .ForMember(dest => dest.Category, o => o.MapFrom(s => s.Category.Name))
                .ForMember(dest => dest.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDTO, CustomerBasket>();
            CreateMap<BasketItemDTO, BasketItem>();
            CreateMap<RegisterDTO, ApplicationUser>();
            CreateMap<Address, AddressDTO>();

        }
    }
}
