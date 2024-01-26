using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Kavior.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> Index()
        {
            List<CouponDto> list = new List<CouponDto>();
            ResponseDto? response = await _couponService.GetAllCouponAsync();   
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

		public async Task<IActionResult> Create()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Create(CouponDto model)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? response = await _couponService.CreateCouponAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {

            ResponseDto? response = await _couponService.GetCouponByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                CouponDto model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(CouponDto model)
        {

            ResponseDto? response = await _couponService.DeleteCouponAsync(model.CouponId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }
    }
}
