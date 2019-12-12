using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Helpers
{
    public static class ResourcesHelper
    {
        public static string GetResourceValue(this HtmlHelper helper, string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return helper.ViewContext.HttpContext.GetGlobalResourceObject("Resources", key).ToString();
        }

        public static string GetResourceValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return HttpContext.GetGlobalResourceObject("Resources", key).ToString();
        }

        public static string GetDayOfWeekHelper(int day)
        {
            string resource ="Sin definir";

            switch (day)
            {
                case 0:
                    resource ="Domingo";
                    break;

                case 1:
                    resource ="Lunes";
                    break;

               case 2:
                    resource ="Martes";
                    break;
               case 3:
                    resource ="Miercoles";
                    break;

               case 4:
                    resource ="Jueves";
                    break;

               case 5:
                    resource ="Viernes";
                    break;

               case 6:
                    resource ="Sabado";
                    break;
            }

            return System.Web.HttpContext.GetGlobalResourceObject("Resources", resource).ToString();
        }
    }
}