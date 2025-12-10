using AutoMapper;
using Talabat.API.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;

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
            CreateMap<Talabat.Core.Entities.Identity.Address, AddressDTO>().ReverseMap();
            CreateMap<Talabat.Core.Entities.Order_Aggregate.Address, AddressDTO>().ReverseMap();

        }
    }
}
