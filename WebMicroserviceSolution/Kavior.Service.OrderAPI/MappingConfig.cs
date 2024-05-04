using AutoMapper;
using Kavior.Service.OrderAPI.Models;
using Kavior.Service.OrderAPI.Models.Dto;

namespace Kavior.Service.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                .ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<Order, OrderDto>().ReverseMap();
                config.CreateMap<OrderDetail, OrderDetailsDto>().ReverseMap();

            });
            return mappingConfig;
        }
    }
}
