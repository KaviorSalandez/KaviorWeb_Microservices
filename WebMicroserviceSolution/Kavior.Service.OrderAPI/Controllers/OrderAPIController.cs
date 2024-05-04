﻿using AutoMapper;
using Kavior.Service.OrderAPI.Models;
using Kavior.Service.OrderAPI.Models.Dto;
using Kavior.Service.OrderAPI.Utility;
using Kavior.Services.OrderAPI.Data;
using Kavior.Services.OrderAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Kavior.Service.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        private IProductService _productService;
        private IConfiguration _configuration;
        public OrderAPIController(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderDto orderDto = _mapper.Map<OrderDto>(cartDto.CartHeader);
                orderDto.Id = 0;
                orderDto.OrderTime = DateTime.Now;
                orderDto.Status = SD.Status_Pending;
                orderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
                foreach (var item in orderDto.OrderDetails)
                {
                    item.Id = 0;
                }
                Order orderCreated = _context.Orders.Add(_mapper.Map<Order>(orderDto)).Entity;
                await _context.SaveChangesAsync();

                orderDto.Id = orderCreated.Id;
                _response.Result = orderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }



        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                //StripeConfiguration.ApiKey = "sk_test_51O3t6VAd9rGlskqFOsUcH6SxJT4ivrmT7ANZuTq8B94HOb0b6uQYytujbju44NC3UXjUZffm0zQuLqQ3p5pgxP0C00QyDhg5xl";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl  = stripeRequestDto?.CancelUrl,
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountsObj = new List<Stripe.Checkout.SessionDiscountOptions>()
                {
                    new Stripe.Checkout.SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.Order.CouponCode
                    }
                };


                foreach (var item in stripeRequestDto.Order.OrderDetails)
                {
                    var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20,99 ->2099
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,

                            },
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.Order.Discount > 0)
                {
                    options.Discounts = discountsObj;
                }

                var service = new Stripe.Checkout.SessionService();
                var session =  service.Create(options); // session này sẽ chứa các thuộc tính cần thiết cho url

                stripeRequestDto.StripeSessionUrl = session.Url;
                // Url này là cần thiết vì dựa vào nó, ứng dụng web của bạn sẽ biết chuyển hướng đến đâu để thu được khoản thanh toán

                // chúng ta sẽ nhận được session Id ở đâu đó
                // Tốt nhất là lưu trữ dữ liệu đó trong cơ sở dữ liệu theo cách đó trong tương lai
                // khi chúng ta phải làm việc với cùng một phiên
                // như hoàn lại tiền hoặc theo dõi nếu thanh toán thành công
                // Chúng tôi có thể thực hiện tất cả những điều đó nếu chúng tôi có ID phiên đã được bắt đầu.
                Order order = _context.Orders.First(x => x.Id == stripeRequestDto.Order.Id);
                order.StripeSessionId = session.Id;
                _context.SaveChanges();
                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }


        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderId)
        {
            try
            {
                Order order = _context.Orders.First(x => x.Id == orderId);


                var service = new Stripe.Checkout.SessionService();
                var session = service.Get(order.StripeSessionId); // session này sẽ chứa các thuộc tính cần thiết cho url


                var paymentIntentService = new Stripe.PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                if(paymentIntent.Status== "succeeded")
                {
                    // then payment was successful
                    order.PaymentIntentId = paymentIntent.Id;
                    order.Status = SD.Status_Approved;
                    _context.SaveChanges();

                    _response.Result = _mapper.Map<OrderDto>(order);
                }

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}