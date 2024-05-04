using AutoMapper;
using Kavior.Services.CouponAPI.Data;
using Kavior.Services.CouponAPI.Models;
using Kavior.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kavior.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private  ResponseDto _response;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _context.Coupons.ToList();
             
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                var obj = _context.Coupons.FirstOrDefault(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                var obj = _context.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                if(obj == null)
                {
                    _response.IsSuccess =false;
                    _response.Message = "Code is invalid";
                }
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }



        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Post( [FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Add(obj);
                _context.SaveChanges();

                // create coupon in stripe
                var options = new Stripe.CouponCreateOptions
                {
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode,
                    AmountOff = (long) (couponDto.DiscountAmount * 100)
                };
                var service = new Stripe.CouponService();
                service.Create(options);


                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]

        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Update(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]

        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _context.Coupons.First(x=>x.CouponId==id);
                _context.Coupons.Remove(obj);
                _context.SaveChanges();

             
                var service = new Stripe.CouponService();
                service.Delete(obj.CouponCode);



            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
