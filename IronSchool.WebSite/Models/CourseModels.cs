using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class CourseIndexVM
    {
        public long CourseId { get; set; }

        [LocalizedDisplayNameAttribute("Course")]
        public string Description { get; set; }

        [LocalizedDisplayNameAttribute("Instructor")]
        public string Instructor { get; set; }

        [LocalizedDisplayNameAttribute("StartDate")]
        public System.DateTime StartDate { get; set; }

        [LocalizedDisplayNameAttribute("FinishDate")]
        public System.DateTime FinishDate { get; set; }

        [LocalizedDisplayNameAttribute("StudentCountMax")]
        public short StudentCountMax { get; set; }

        [LocalizedDisplayNameAttribute("StudentCount")]
        public int StudentCount { get; set; }
    }

    public class CourseVM
    {
        public long CourseId { get; set; }

        [LocalizedDisplayNameAttribute("Description")]
        [Required, StringLength(255)]
        public string Description { get; set; }

        public long InstructorId { get; set; }

        [LocalizedDisplayNameAttribute("Instructor")]
        [Required]
        public string InstructorName { get; set; }

        [LocalizedDisplayNameAttribute("StartDate")]
        [Required]
        public System.DateTime StartDate { get; set; }

        [LocalizedDisplayNameAttribute("FinishDate")]
        [Required]
        public System.DateTime FinishDate { get; set; }

        [LocalizedDisplayNameAttribute("StudentCountMax")]
        [Required]
        public short StudentCountMax { get; set; }

        public List<StudentBasicIndexVM> Students { get; set; }

        public CourseVM()
        {
            Students = new List<StudentBasicIndexVM>();
        }
    }
}