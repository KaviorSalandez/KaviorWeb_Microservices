﻿using Kavior.Web.Models;
using Kavior.Web.Service;
using Kavior.Web.Service.IService;
using Kavior.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;

namespace Kavior.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderDto orderDto = new OrderDto();
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderDto = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(response.Result));
            }
            if(!User.IsInRole(SD.RoleAdmin) && userId != orderDto.UserId)
            {
                return NotFound();
            }
            return View(orderDto);

        }
        [HttpGet]
        public  IActionResult GetAll(string status) {
            IEnumerable<OrderDto> list;
            string userId = "";
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(x=>x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto response =  _orderService.GetAllOrder(userId).GetAwaiter().GetResult();  
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderDto>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        list = list.Where(x => x.Status == SD.Status_Approved);
                        break;
                    case "readyforpickup":
                        list = list.Where(x => x.Status == SD.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(x => x.Status == SD.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                list = new List<OrderDto>();
            }
            return Json(new {data = list});
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Complete);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
    }
}
