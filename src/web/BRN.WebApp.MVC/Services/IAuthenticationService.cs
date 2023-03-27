using System.Threading.Tasks;
using BRN.WebApp.MVC.Models;

namespace BRN.WebApp.MVC.Services
{
    public interface IAuthenticationService
    {
        Task<UserLoginResponse> Login(UserLogin userLogin);

        Task<UserLoginResponse> Register(UserRegister userRegister);

    }
}
