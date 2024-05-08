﻿using Kavior.Web.Models;

namespace Kavior.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto); 
        Task<ResponseDto> ValidateStripeSession(int orderId); 
        Task<ResponseDto> GetAllOrder(string? userId); 
        Task<ResponseDto> GetOrder(int orderId); 
        Task<ResponseDto> UpdateOrderStatus(int orderId, string newStatus); 
    }
}
