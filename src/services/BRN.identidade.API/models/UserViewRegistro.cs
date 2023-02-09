﻿using System.ComponentModel.DataAnnotations;

namespace BRN.identidade.API.models
{
    public class UserViewRegistro
    {
        [Required(ErrorMessage = "o campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]

        public string Email { get; set; }

        [Required(ErrorMessage = "o campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6 )]

        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "As senhas não conferem")]

        public string SenhaConfirmacao { get; set; }
    }
}
