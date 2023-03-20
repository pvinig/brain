using System.ComponentModel;
using System.ComponentModel.DataAnnotations; 

namespace BRN.WebApp.MVC.Models
{
    public class UserRegister
    {
        [Required(ErrorMessage = "Field {0} is required")]
        [EmailAddress(ErrorMessage = "field {0} is in invalid format" )]
        public string Email { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(100, ErrorMessage = "field {0} need be between {2} and {1} characters", MinimumLength = 6)]

        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "email or password invalid")]

        public string PasswordConfirm { get; set; }
    }

    public class UserLogin
    {
        [Required(ErrorMessage = "field {0} is required")]
        [EmailAddress(ErrorMessage = "field {0} is in invalid format")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Field {0} is required")]
        [StringLength(100, ErrorMessage = "field {0} need be between {2} and {1} characters", MinimumLength = 6)]

        public string Password { get; set; }

    }
}
