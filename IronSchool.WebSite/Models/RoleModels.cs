using ExpressiveAnnotations.Attributes;
using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class RoleIndexVM
    {
        public long RoleId { get; set; }
        [LocalizedDisplayNameAttribute("Name")]
        public string Name { get; set; }
        [LocalizedDisplayNameAttribute("Active")]
        public string Active { get; set; }
    }

    public class RoleVM
    {
        public long RoleId { get; set; }
        public long EmpresaId { get; set; }

        [Required]
        [LocalizedDisplayNameAttribute("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [LocalizedDisplayNameAttribute("Active")]
        public bool Active { get; set; }

        public List<RoleRuleVM> Rules { get; set; }

        public List<RoleUserVM> Users { get; set; }

    }

    public class RoleRuleVM
    {
        public long RuleId { get; set; }

        [LocalizedDisplayNameAttribute("Name")]
        public string DisplayName { get; set; }

        public bool Selected { get; set; }
    }

    public class RoleUserVM
    {
        public long UserId { get; set; }

        [LocalizedDisplayNameAttribute("FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayNameAttribute("LastName")]
        public string LastName { get; set; }

        public bool Selected { get; set; }
    }
}