using BRN.WebApp.MVC.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IAuthenticationService = BRN.WebApp.MVC.Services.IAuthenticationService;

namespace BRN.WebApp.MVC.Controllers
{
    public class identityController : MainController
    {
        private readonly IAuthenticationService _authenticationService;

        public identityController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("new-account")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("new-account")]
        public async Task<IActionResult> Register(UserRegister userRegister)
        {
            if (!ModelState.IsValid) return View(userRegister);
            var response = await _authenticationService.Register(userRegister);

            if(ErrorsInResponse(response.ResponseResult)) return View(userRegister);

            await RealizeLogin(response);

            return RedirectToAction("index",controllerName: "home");

        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid) return View(userLogin);

            var response  = await _authenticationService.Login(userLogin);

            if (ErrorsInResponse(response.ResponseResult)) return View(userLogin);

            await RealizeLogin(response);

            return RedirectToAction("index", controllerName: "home");

        }

        private async Task RealizeLogin(UserLoginResponse response)
        {
            var token = takeToken(response.access_token);

            var claims = new List<Claim>(); 
            claims.Add(new Claim(type: "JWT", value: response.access_token));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authPropeerties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true,    
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authPropeerties );
        }
        private static JwtSecurityToken takeToken(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(jwtToken) as JwtSecurityToken;
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            return RedirectToAction("index", controllerName: "home");

        }

    }
}
