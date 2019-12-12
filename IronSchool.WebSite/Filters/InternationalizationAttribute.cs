using IronSchool.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Filters
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        public InternationalizationAttribute()
        {

        }

        public InternationalizationAttribute(string defaultValue)
        {
            Context.SetValue("Culture", CultureInfo.GetCultureInfo(defaultValue));
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(defaultValue);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                CultureInfo ci = (CultureInfo)Context.GetValue("Culture");
                if (ci != null)
                {
                    Thread.CurrentThread.CurrentUICulture = ci;
                }
            }
            catch (Exception)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            }
        }
    }
}