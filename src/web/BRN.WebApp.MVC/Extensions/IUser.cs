using System.Security.Claims;

namespace BRN.WebApp.MVC.Extensions
{
    public interface IUser
    {
        string Name { get; }

        Guid GetUserId();   

        string GetUserEmail();

        string GetUserToken();

        bool IsLoggedIn();

        bool HasRole(string role);

        IEnumerable<Claim> GetClaims();

        HttpContext GetHttpContext();

    }

    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AspNetUser(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Name => _contextAccessor.HttpContext.User.Identity.Name;

        public Guid GetUserId()
        {
            return IsLoggedIn() ? Guid.Parse(_contextAccessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }

        public string GetUserEmail()
        {
            return IsLoggedIn() ? _contextAccessor.HttpContext.User.GetUserEmail() : "";
        }

        public string GetUserToken()
        {
            return IsLoggedIn() ? _contextAccessor.HttpContext.User.GetUserToken() : "";
        }

        public bool IsLoggedIn()
        {
            return _contextAccessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool HasRole(string role)
        {
            return _contextAccessor.HttpContext.User.IsInRole(role);
        }

        public IEnumerable<Claim> GetClaims()
        {
            return _contextAccessor.HttpContext.User.Claims;
        }

        public HttpContext GetHttpContext()
        {
            return _contextAccessor.HttpContext;
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.FindFirst(type: "sub");
            return claim?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if(principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.FindFirst(type: "email");
            return claim?.Value;
        }

        public static string GetUserToken(this ClaimsPrincipal principal)
        {
            if(principal == null) 
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.FindFirst(type: "JWT");
            return claim?.Value;
        }
        

    }
}
