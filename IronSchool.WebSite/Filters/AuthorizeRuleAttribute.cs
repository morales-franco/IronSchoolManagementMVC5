using IronSchool.Business;
using IronSchool.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IronSchool.WebSite.Filters
{
    public class AuthorizeRuleAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string rule = string.Empty;
        private bool ignore = false;
        private bool appendControllerName = false;


        public AuthorizeRuleAttribute()
        {

        }

        public AuthorizeRuleAttribute(string rule)
        {
            this.rule = rule;
        }
        public AuthorizeRuleAttribute(string rule, bool appendControllerName)
        {
            this.rule = rule;
            this.appendControllerName = appendControllerName;
        }

        public AuthorizeRuleAttribute(bool ignore)
        {
            this.ignore = ignore;           
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!ignore)
            {
                string ruleDefinition = string.Empty;

                if (string.IsNullOrEmpty(rule))
                    ruleDefinition = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName;
                else
                {
                    ruleDefinition = this.rule;
                    if (appendControllerName)
                        ruleDefinition = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + ruleDefinition;
                }

                var entityID = filterContext.RouteData.Values["id"];

                User usuario = null;

                if (UserBusiness.Current != null)
                    usuario = UserBusiness.Current;

                if (usuario != null)
                {
                        //Si no estoy editando mi perfil y no estoy autorizado a esa regla
                        if (!((filterContext.ActionDescriptor.ActionName == "MyProfile" || filterContext.ActionDescriptor.ActionName == "ResetMyPassword")
                                && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "User") &&
                            !UserBusiness.Authorize(ruleDefinition))
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login"}));
                        }
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = filterContext.HttpContext.Request.Url.PathAndQuery }));
                }
            }
        }
    }
}