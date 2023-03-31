using System.Linq;
using BRN.WebApp.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRN.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ErrorsInResponse(ResponseResult response)
        {
            if(response != null && response.Errors.Messages.Any())
            {
                return true;
            }

            return false;
        }

    }
}
