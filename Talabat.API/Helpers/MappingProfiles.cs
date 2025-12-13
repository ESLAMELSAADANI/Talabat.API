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

            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(dest => dest.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(dest => dest.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(dest => dest.DeliveryMethodCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(dest => dest.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(dest => dest.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
                .ForMember(dest => dest.PictureUrl, o => o.MapFrom<OrderItemPictureUrlResolver>());







        }
    }
}
