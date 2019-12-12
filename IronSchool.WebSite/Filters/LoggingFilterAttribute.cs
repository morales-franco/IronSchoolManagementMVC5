using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FDFMobile.WebSite.Filters
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string model = "";
            if (filterContext.ActionArguments.ContainsKey("model"))
            {
                model = JsonConvert.SerializeObject(
                filterContext.ActionArguments["model"],
                Formatting.Indented);
            }
            //Utils.Services.Logger.Debug("Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName + "    -     JSON: " + model);
        }
    }
}