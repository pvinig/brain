using BRN.identidade.API.extensions;
using BRN.identidade.API.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BRN.identidade.API.Jwt;

namespace BRN.identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]

    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appsettings;

        public AuthController(SignInManager<IdentityUser> signInManager, 
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appsettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appsettings = appsettings.Value;
        }

        [HttpPost("nova-conta")]

        public async Task<ActionResult> Registrar(UserRegistroModel userRegistro)
        {
            if(!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = userRegistro.Email,
                Email = userRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRegistro.Senha);
            var jwtBuilder = new JwtBuilder(_userManager, _appsettings, userRegistro.Email);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return CustomResponse(new
                {
                    access_token = await jwtBuilder.GenerateAccessToken()
                });
            }
            foreach(var error in result.Errors) 
            {
                AddError(error.Description);
            }
            return CustomResponse();
        }


        [HttpPost("autenticar")]

        public async Task<ActionResult> login(UserLoginModel userLogin)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(userName: 
                userLogin.Email,
                password: userLogin.Password,
                isPersistent: false,
                lockoutOnFailure: true  );

            var jwtBuilder = new JwtBuilder(_userManager, _appsettings, userLogin.Email);

            if (result.Succeeded)
            {
                return CustomResponse(
                new {
                        access_token = await jwtBuilder.GenerateAccessToken(),
                        refresh_token = await jwtBuilder.GenerateRefreshToken()

                     });
            }       

            if(result.IsLockedOut)
            {
                AddError("too many errors");
            }

            AddError("Invalid user or password !!");
            return CustomResponse();
        }
    }
}
