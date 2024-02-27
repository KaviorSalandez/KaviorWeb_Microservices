using AutoMapper;
using Kavior.Services.ProductAPI.Models;
using Kavior.Services.ProductAPI.Models.Dto;

namespace Kavior.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() { 
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
