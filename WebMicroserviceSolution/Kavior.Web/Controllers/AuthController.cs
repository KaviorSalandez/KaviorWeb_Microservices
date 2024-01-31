using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Kavior.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Kavior.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;

        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            ResponseDto responseDto = await _authService.LoginAsync(model);
            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                // before we set token, we will call our sign in user and we will pass the obj.
                await SignInUser(loginResponseDto);

                _tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(model);
            }
        }


        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);
            //And from that token, now we want to extract all the claims that we added.
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                                    jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Name,
                                  jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                                  jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            //That will sign in the user using the Dotnet identity.
        }




        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem {Text = SD.RoleAdmin, Value= SD.RoleAdmin},
                new SelectListItem {Text = SD.RoleCustomer, Value= SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDto model)
        {
            ResponseDto result = await _authService.RegisterAsync(model);
            ResponseDto assignRole;
            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(model.Role))
                {
                    model.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(model);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successfull";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem {Text = SD.RoleAdmin, Value= SD.RoleAdmin},
                new SelectListItem {Text = SD.RoleCustomer, Value= SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            TempData["error"] = result.Message;
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index","Home");
        }
    }
}
