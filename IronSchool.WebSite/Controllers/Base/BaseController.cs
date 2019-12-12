using IronSchool.Business.Base;
using IronSchool.WebSite.Filters;
//using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers.Base
{

    /*[CustomHandleError(ExceptionType = typeof(Exception), View = "Error")]*/
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Internationalization("es")]
    [AuthorizeRule]
    public class BaseController : Controller
    {
        protected void HandleException(Exception ex)
        {
            if (ex is ValidationException)
            {
                if ((ex as ValidationException).KeyNames != null)
                    ModelState.AddModelError(string.Empty, string.Format(HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString(), (ex as ValidationException).KeyNames));
                else
                    ModelState.AddModelError(string.Empty, HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString());
            }
            else
            {
                Utils.Services.Logger.Error("Unexpected Exception", ex);
                ModelState.AddModelError("", HttpContext.GetGlobalResourceObject("Resources", "Error").ToString());
            }
        }

        protected virtual void AddBooleanFilter()
        {
            SelectList BooleanFilter = new SelectList(new List<Object>{
                                                               new { value = true , text = HttpContext.GetGlobalResourceObject("Resources", "Yes") },
                                                               new { value = false , text = HttpContext.GetGlobalResourceObject("Resources", "No")}}, "value", "text", "");
            ViewBag.BooleanFilter = BooleanFilter;
        }

        protected virtual void AddBooleanItems()
        {
            SelectList BooleanItems = new SelectList(new List<Object>{
                                                               new { value = true , text = HttpContext.GetGlobalResourceObject("Resources", "Yes") },
                                                               new { value = false , text = HttpContext.GetGlobalResourceObject("Resources", "No")}}, "value", "text", "");
            ViewBag.BooleanItems = BooleanItems;
        }
    }
}