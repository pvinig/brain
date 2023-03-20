using BRN.WebApp.MVC.Models;
using BRN.WebApp.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BRN.WebApp.MVC.Controllers
{
    public class identityController : Controller
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


            if (false) return View(userRegister);

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

            var response = await _authenticationService.Login(userLogin);

            if (false) return View(userLogin);

            return RedirectToAction("index", controllerName: "home");

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            return RedirectToAction("index", controllerName: "home");

        }

    }
}
