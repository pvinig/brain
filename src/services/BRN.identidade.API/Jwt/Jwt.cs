using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BRN.identidade.API.extensions;

namespace BRN.identidade.API.Jwt
{
    

        public class JwtBuilder<TIdentityUser, TIdentityRole, TKey>
            where TIdentityUser : IdentityUser<TKey>
            where TIdentityRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            private UserManager<TIdentityUser> _userManager;
            private RoleManager<TIdentityRole> _roleManager;
            private AppSettings _appJwtSettings;
            private string _email;

            public JwtBuilder(UserManager<TIdentityUser> userManager, AppSettings appJwtSettings, string email)
            {
                _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
                _appJwtSettings = appJwtSettings ?? throw new ArgumentException(nameof(appJwtSettings));
                _email = string.IsNullOrEmpty(email) ? throw new ArgumentException(nameof(email)) : email;
            }

            public async Task<string> GenerateAccessToken()
            {
                var identityClaims = new ClaimsIdentity();

                var user = await _userManager.FindByEmailAsync(_email);
                var userRoles = await _userManager.GetRolesAsync(user);

                identityClaims.AddClaims(await _userManager.GetClaimsAsync(user));
                identityClaims.AddClaims(userRoles.Select(s => new Claim("role", s)));

                foreach (var userRole in userRoles)
                {
                    var role = await _roleManager.FindByNameAsync(userRole);
                    identityClaims.AddClaims(await _roleManager.GetClaimsAsync(role));
                }

                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, value: user.Id.ToString()));
                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Email, user.Email));
                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appJwtSettings.Secret);
                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = _appJwtSettings.Issuer,
                    Audience = _appJwtSettings.Audience,
                    Subject = identityClaims,
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddHours(_appJwtSettings.ExpirateTime),
                    IssuedAt = DateTime.UtcNow,
                    TokenType = "at+jwt",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                });

                return tokenHandler.WriteToken(token);
            }

            public async Task<string> GenerateRefreshToken()
            {
                var jti = Guid.NewGuid().ToString();
                var identityClaims = new ClaimsIdentity();

                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Email, _email));
                identityClaims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, jti));

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appJwtSettings.Secret);
                var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = _appJwtSettings.Issuer,
                    Audience = _appJwtSettings.Audience,
                    Subject = identityClaims,
                    Expires = DateTime.UtcNow.AddDays(30),
                    NotBefore = DateTime.UtcNow,
                    IssuedAt = DateTime.UtcNow,
                    TokenType = "rt+jwt",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                });

                await UpdateLastGeneratedClaim(jti);

                return tokenHandler.WriteToken(token);
            }

            private async Task UpdateLastGeneratedClaim(string jti)
            {
                var user = await _userManager.FindByEmailAsync(_email);
                var claims = await _userManager.GetClaimsAsync(user);

                var newLastRtClaim = new Claim("LastRefreshToken", jti);

                var claimLastRt = claims.FirstOrDefault(f => f.Type == "LastRefreshToken");

                if (claimLastRt != null)
                    await _userManager.ReplaceClaimAsync(user, claimLastRt, newLastRtClaim);
                else
                    await _userManager.AddClaimAsync(user, newLastRtClaim);
            }

            private static long ToUnixEpochDate(DateTime date)
                => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                    .TotalSeconds);
        }

        public class JwtBuilder<TIdentityUser, TIdentityRole> : JwtBuilder<TIdentityUser, TIdentityRole, string>
            where TIdentityUser : IdentityUser<string>
            where TIdentityRole : IdentityRole<string>
        {
            public JwtBuilder(UserManager<TIdentityUser> userManager, AppSettings appJwtSettings, string email) : base(userManager, appJwtSettings, email)
            {
            }
        }

        public sealed class JwtBuilder : JwtBuilder<IdentityUser, IdentityRole>
        {
            public JwtBuilder(UserManager<IdentityUser> userManager, AppSettings appJwtSettings, string email) : base(userManager, appJwtSettings, email)
            {
            }
        }
    
}
