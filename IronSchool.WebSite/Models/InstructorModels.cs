using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class InstructorIndexVM
    {
        public long InstructorId { get; set; }

        [LocalizedDisplayNameAttribute("FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayNameAttribute("LastName")]
        public string LastName { get; set; }

        [LocalizedDisplayNameAttribute("BirthDate")]
        public System.DateTime BirthDate { get; set; }

        [LocalizedDisplayNameAttribute("Address")]
        public string Address { get; set; }

        [LocalizedDisplayNameAttribute("Email")]
        public string ContactMail { get; set; }

        [LocalizedDisplayNameAttribute("Phone")]
        public string ContactPhone { get; set; }

        [LocalizedDisplayNameAttribute("Sex")]
        public string Sex { get; set; }

        [LocalizedDisplayNameAttribute("Salary")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
    }

    public class InstructorVM
    {
        public long InstructorId { get; set; }

        [LocalizedDisplayNameAttribute("BirthDate")]
        [Required]
        public System.DateTime BirthDate { get; set; }

        [LocalizedDisplayNameAttribute("Address")]
        [Required, StringLength(100)]
        public string Address { get; set; }

        [LocalizedDisplayNameAttribute("Email")]
        [Required, StringLength(100), EmailAddress]
        public string ContactMail { get; set; }

        [LocalizedDisplayNameAttribute("Phone")]
        [Required, StringLength(50)]
        public string ContactPhone { get; set; }

        [LocalizedDisplayNameAttribute("Sex")]
        [Required]
        public string Sex { get; set; }

        [LocalizedDisplayNameAttribute("Salary")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal Salary { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
        [LocalizedDisplayNameAttribute("User")]
        [Required]
        public string UserCompletedName { get; set; }
    }
}