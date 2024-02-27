using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Kavior.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _ProductService;
        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new List<ProductDto>();
            ResponseDto? response = await _ProductService.GetAllProductAsync();   
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
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
        public async Task<IActionResult> Create(ProductDto model)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? response = await _ProductService.CreateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";

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

            ResponseDto? response = await _ProductService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ProductDto model)
        {

            ResponseDto? response = await _ProductService.DeleteProductAsync(model.Id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {

            ResponseDto? response = await _ProductService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);

            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductDto model)
        {

            ResponseDto? response = await _ProductService.UpdateProductAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product updated successfully";

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

