using System.ComponentModel.DataAnnotations;

namespace BRN.identidade.API.models
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "field {0} is required")]
        [EmailAddress(ErrorMessage = "field {0} is in invalid format")]

        public string Email { get; set; }

        [Required(ErrorMessage = "o campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]

        public string Senha { get; set; }

    }
}
