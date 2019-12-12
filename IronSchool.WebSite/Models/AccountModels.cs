using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class AccountModels
    {
        public class LoginRegisterViewModel
        {
            [Required]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }           
        }

        public class RecoverPasswordViewModel
        {
            [Required]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }            
        }

        public class ResetPasswordVM
        {
            [Required]
            public long UserId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de largo.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [LocalizedDisplayNameAttribute("NewPassword")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [LocalizedDisplayNameAttribute("PasswordConfirmation")]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
            public bool CodeFromMail { get; set; }

            public ResetPasswordVM()
            {
                CodeFromMail = true;
            }
        }

        public class EmpresaSelectorViewModel
        {
            public long? EmpresaID { get; set; }
        }
    }
}