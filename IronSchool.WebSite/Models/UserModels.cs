using ExpressiveAnnotations.Attributes;
using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class UserIndexVM
    {
        public long UserId { get; set; }
        [LocalizedDisplayNameAttribute("UserName")]
        public string UserName { get; set; }
        [LocalizedDisplayNameAttribute("Active")]
        public String Active { get; set; }
        [LocalizedDisplayNameAttribute("FirstName")]
        public string FirstName { get; set; }
        [LocalizedDisplayNameAttribute("LastName")]
        public string LastName { get; set; }
        [LocalizedDisplayNameAttribute("Email")]
        public string Email { get; set; }
    }

    public class UserVM
    {
        public long UserId { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("UserName")]
        [StringLength(255)]
        public string UserName { get; set; }

        [LocalizedDisplayNameAttribute("Active")]
        public bool Active { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("FirstName")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("LastName")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("Email")]
        [StringLength(255)]
        public string Email { get; set; }

        [RequiredIf("UserId <= 0")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de largo", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayNameAttribute("Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayNameAttribute("ConfirmPassword")]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordVM
    {
        public long UserId { get; set; }

        [LocalizedDisplayNameAttribute("UserName")]
        public string UserName { get; set; }        

        [RequiredIf("UserId <= 0")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de largo", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayNameAttribute("NewPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayNameAttribute("ConfirmPassword")]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }

    public class MyProfileVM
    {
        public long UserId { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("UserName")]
        [StringLength(255)]
        public string UserName { get; set; }        

        [Required]
        [LocalizedDisplayNameAttribute("FirstName")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("LastName")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("Email")]
        [StringLength(255)]
        public string Email { get; set; }       
    }
}