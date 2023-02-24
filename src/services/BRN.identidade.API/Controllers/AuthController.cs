using System;
using Microsoft.AspNetCore.Identity;
using BRN.identidade.API.models;
using BRN.identidade.API.extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BRN.identidade.API.Controllers
{
    [ApiController]
    [Route("api/identidade")]

    public class AuthController : Controller
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
            if(!ModelState.IsValid) return BadRequest();

            var user = new IdentityUser
            {
                UserName = userRegistro.Email,
                Email = userRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRegistro.Senha);

            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(await GerarJwt(userRegistro.Email));
            }

            return BadRequest();
        }

        [HttpPost("autenticar")]

        public async Task<ActionResult> login(UserLoginModel userLogin)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _signInManager.PasswordSignInAsync(userName: userLogin.Email, password: userLogin.Senha,
                isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return Ok(await GerarJwt(userLogin.Email));
            }

            return BadRequest();

        }

        [Consumes("application/json")]

        public async Task<UserRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Id));
            claims.Add(new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email));
            claims.Add(new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()));
            claims.Add(new Claim(type: JwtRegisteredClaimNames.Nbf, value: ToUnixEpochData(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(type: JwtRegisteredClaimNames.Iat, value: ToUnixEpochData(DateTime.UtcNow).ToString(), ClaimValueTypes.UInteger64));
        
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(type: "role", value: userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appsettings.Issuer,
                Audience = _appsettings.Audience,
                Subject = identityClaims,
               // Expires = DateTime.UtcNow.AddHours(_appsettings.ExpirateTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(_appsettings.ExpirateTime),

            });

            var encodedToken = tokenHandler.WriteToken(token);

            var response = new UserRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appsettings.ExpirateTime).TotalSeconds,
                UserToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };

            return response;

        } 

        private static long ToUnixEpochData(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(
                year: 1970,
                month: 1,
                day: 1,
                hour: 0,
                minute: 0,
                second: 0,
                offset: TimeSpan.Zero)).TotalSeconds);
    }

}
