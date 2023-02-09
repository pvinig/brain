using Microsoft.AspNetCore.Identity;
using BRN.identidade.API.models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BRN.identidade.API.Controllers
{
    [Route("api/identidade")]

    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("nova-conta")]

        public async Task<ActionResult> Registrar(UserViewRegistro usuarioRegistro)
        {
            if(!ModelState.IsValid) return BadRequest();

            var user = new IdentityUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("autenticar")]

        public async Task<ActionResult> login(UserViewLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(userName: usuarioLogin.Email, password: usuarioLogin.Senha,
                isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded) return Ok();

            return BadRequest();

        }



    }
}
