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

namespace IronSchool.WebSite.Controllers
{
    public class InstructorController : BaseABMController<InstructorBusiness, Instructor, InstructorIndexVM, InstructorVM, long>
    {
        [AuthorizeRule("Index", true)]
        public ActionResult Browser()
        {
            return View();
        }

        protected override void PopulateViewBagForCreate(InstructorVM model)
        {
            base.PopulateViewBagForCreate(model);
            ViewBag.SexList = GetSexSelectListItems();
        }

        protected override void PopulateViewBagForEdit(InstructorVM model)
        {
            base.PopulateViewBagForEdit(model);
            ViewBag.SexList = GetSexSelectListItems();
        }

        protected override void PopulateViewBagForDetails(InstructorVM model)
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