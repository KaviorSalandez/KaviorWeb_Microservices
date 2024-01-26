using AutoMapper;
using Kavior.Services.CouponAPI.Models;
using Kavior.Services.CouponAPI.Models.Dto;

namespace Kavior.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() { 
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingConfig;
        }
    }
}
