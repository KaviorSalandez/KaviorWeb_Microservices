using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Kavior.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Kavior.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService,IOrderService orderService)
        {
            _cartService = cartService; 
            _orderService = orderService;   
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View( await LoadCartDtoBasedOnLoggedInUser());
        }
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;

            ResponseDto? response = await _orderService.CreateOrder(cart);
            OrderDto orderDto = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                // get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new StripeRequestDto()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderDto.Id,
                    CancelUrl = domain + "cart/Checkout",
                    Order = orderDto,
                };

                // about stripe have 4 four parameter
                // 1. SessionId: Id của session checkout
                // 2. URL: url to go to page which user can enter a credit card
                // 3. Back/Cancel Url: go back to cart page to continue shoppping
                // 4. Approve Url: If you select "Pay" it will redirect to some confirmation page on your website

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDto stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));
                // Đặt URL chuyển hướng tới phiên thanh toán Stripe
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);// the status Code 303 basically denotes that there is a redirect to another page.
            }
            return View();
        }
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderDto order = JsonConvert.DeserializeObject<OrderDto>(Convert.ToString(response.Result));
                if(order.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to some error page based on status
            return View(orderId);
        }


        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdsync(userId); 
            if(response != null && response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart update successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Apply coupon successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = ""; 
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Remove coupon successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            ResponseDto? response = await _cartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

    }
}
