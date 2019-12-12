using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IronSchool.WebSite.Models
{
    public class QualificationIndexVM
    {
        public string QualificationId { get; set; }

        [LocalizedDisplayNameAttribute("Student")]
        public string Student { get; set; }

        [LocalizedDisplayNameAttribute("Course")]
        public string Course { get; set; }

        [LocalizedDisplayNameAttribute("Qualification")]
        public string Qualification { get; set; }
    }

    public class QualificationVM
    {
        public long StudentId { get; set; }

        [LocalizedDisplayNameAttribute("Student")]
        public string StudentName { get; set; }
        public long CourseId { get; set; }

        [LocalizedDisplayNameAttribute("Course")]
        public string CourseDescription { get; set; }

        [LocalizedDisplayNameAttribute("Qualification")]
        public short? ExamQualification { get; set; }
    }
}