using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers
{
    public class QualificationController : BaseABMController<StudentsInCourseBusiness, StudentsInCourse, QualificationIndexVM, QualificationVM, string>
    {
        protected override void AddInferedParameters(List<KeyValuePair<string, object>> values)
        {
            values.Add(new KeyValuePair<string, object>("InstructorUserId", UserBusiness.Current.UserId));
        }
    }
}