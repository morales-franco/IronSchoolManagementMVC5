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
    public class CourseController : BaseABMController<CourseBusiness, Course, CourseIndexVM, CourseVM, long>
    {

    }
}