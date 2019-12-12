using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using IronSchool.Business.Base;
using IronSchool.WebSite.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers.Base
{
    public class BaseReportController<B, I> : BaseController
        where I : class, new()
        where B : IReportBusinessBase, new()
    {
        protected B Business = new B();

        public ActionResult Index()
        {
            PopulateViewBagForIndex();
            return View();
        }

        protected virtual void PopulateViewBagForIndex()
        {

        }

        [AuthorizeRule("Index", true)]
        public virtual ActionResult GetList([DataSourceRequest] DataSourceRequest request, FormCollection parameters)
        {
            var par = ParseIndexFilter(parameters, true);

            var datasource = GetFilteredList(par);

            return Json(datasource.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        protected virtual List<KeyValuePair<string, object>> ParseIndexFilter(FormCollection parameters, bool skipGridParameters)
        {
            List<KeyValuePair<string, object>> values = new List<KeyValuePair<string, object>>();
            int gridParametersCount = 5;
            int paramIndex = 0;
            foreach (string parameter in parameters)
            {
                if (skipGridParameters && paramIndex >= gridParametersCount)
                {
                    if (!string.IsNullOrEmpty(HttpContext.Request.Params[parameter]))
                    {
                        values.Add(new KeyValuePair<string, object>(parameter, HttpContext.Request.Params[parameter]));
                    }
                    else
                    {
                        values.Add(new KeyValuePair<string, object>(parameter, null));
                    }
                }
                paramIndex++;
            }
            AddInferedParameters(values);
            return values;
        }

        protected virtual void AddInferedParameters(List<KeyValuePair<string, object>> values)
        {
            /*
            Type[] interfaces = typeof(E).GetInterfaces();
            if (interfaces.Contains(typeof(IronSchool.Entities.Base.ICompanyEntity)) &&
                values.Count(k => k.Key == "EmpresaId") == 0)
                values.Add(new KeyValuePair<string, object>("EmpresaId", UserBusiness.Current.EmpresaId));
                */
        }

        protected virtual IEnumerable<I> GetFilteredList(List<KeyValuePair<string, object>> filter)
        {
            return this.Business.GetList<I>(filter.ToArray());
        }
    }
}