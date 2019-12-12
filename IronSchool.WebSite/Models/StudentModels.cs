using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class StudentBasicIndexVM
    {
        public long StudentId { get; set; }

        [LocalizedDisplayNameAttribute("FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayNameAttribute("LastName")]
        public string LastName { get; set; }
    }

    public class StudentIndexVM : StudentBasicIndexVM
    {
        [LocalizedDisplayNameAttribute("BirthDate")]
        public System.DateTime BirthDate { get; set; }

        [LocalizedDisplayNameAttribute("Address")]
        public string Address { get; set; }

        [LocalizedDisplayNameAttribute("Phone")]
        public string ContactPhone { get; set; }

        [LocalizedDisplayNameAttribute("Sex")]
        public string Sex { get; set; }

        [LocalizedDisplayNameAttribute("Email")]
        public string ContactMail { get; set; }
    }

    public class StudentVM
    {
        public long StudentId { get; set; }

        [LocalizedDisplayNameAttribute("FirstName")]
        [Required, StringLength(100)]
        public string FirstName { get; set; }

        [LocalizedDisplayNameAttribute("LastName")]
        [Required, StringLength(100)]
        public string LastName { get; set; }

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
        [StringLength(50)]
        public string ContactPhone { get; set; }

        [LocalizedDisplayNameAttribute("Sex")]
        [Required]
        public string Sex { get; set; }

        public StudentVM()
        {
            BirthDate = DateTime.Now;
        }
    }

    
}