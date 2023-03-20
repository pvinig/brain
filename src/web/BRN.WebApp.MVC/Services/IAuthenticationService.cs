using System.Threading.Tasks;
using BRN.WebApp.MVC.Models;

namespace BRN.WebApp.MVC.Services
{
    public interface IAuthenticationService
    {
        Task<string> Login(UserLogin userLogin);

        Task<string> Register(UserRegister userRegister);

    }
}
