using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS.WebSite.Controllers
{
    public class StudentController : BaseABMController<StudentBusiness, Student, StudentIndexVM, StudentVM, long>
    {
        [AuthorizeRule("Index", true)]
        public ActionResult Browser()
        {
            return View();
        }
        protected override void PopulateViewBagForCreate(StudentVM model)
        {
            base.PopulateViewBagForCreate(model);
            ViewBag.SexList = GetSexSelectListItems();
        }

        protected override void PopulateViewBagForEdit(StudentVM model)
        {
            base.PopulateViewBagForEdit(model);
            ViewBag.SexList = GetSexSelectListItems();
        }

        protected override void PopulateViewBagForDetails(StudentVM model)
        {
            base.PopulateViewBagForDetails(model);
            ViewBag.SexList = GetSexSelectListItems();
        }

        public static SelectList GetSexSelectListItems()
        {
            var sexs = new SexBusiness().GetAll()
                    .Select(x => new
                    {
                        id = x.SexId,
                        text = x.Description
                    });

            return new SelectList(sexs, "id", "text");
        }

    }
}