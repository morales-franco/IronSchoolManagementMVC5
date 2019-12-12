using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using IronSchool.Business;
using IronSchool.Business.Base;
using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers.Base
{
    public class BaseABMController<B, E, I, VM, TID> : BaseController
        where E : class, new()
        where I : class, new()
        where VM : class, new()
        where B : IEntityBusinessBase<E>, new()
    {
        protected B Business = new B();

        public ActionResult Index()
        {
            PopulateViewBagForIndex();
            ShowGenericMessage();

            return View();
        }

        protected virtual void ShowGenericMessage()
        {
            string msj = (string)TempData["msj"];
            if (!string.IsNullOrWhiteSpace(msj))
                ViewBag.MostrarMensajeGenerico = msj;
        }

        protected virtual void AddGenericMessage(string message)
        {
            TempData["msj"] = message;
        }

        [AuthorizeRule("Index", true)]
        public ActionResult GetList([DataSourceRequest] DataSourceRequest request, FormCollection parameters)
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
            //Type[] interfaces = typeof(E).GetInterfaces();
            //if (interfaces.Contains(typeof(IronSchool.Entities.Base.ICompanyEntity)) && 
            //    values.Count(k=>k.Key == "EmpresaId") == 0)
            //    values.Add(new KeyValuePair<string, object>("EmpresaId", UserBusiness.Current.EmpresaId));
        }

        protected virtual IEnumerable<I> GetFilteredList(List<KeyValuePair<string, object>> filter)
        {
            return this.Business.GetList<I>(filter.ToArray());
        }

        protected virtual T Map<S, T>(S source)
             where T : class, new()
        {
            if (source == null)
                return null;
            if (typeof(T) == typeof(S))
                return source as T;

            T entity = new T();
            /*AutoMapper.Mapper.CreateMap<string, DateTime>().ConvertUsing<Utils.TypeConverters.StringToDateTimeTypeConverter>();
            AutoMapper.Mapper.CreateMap<string, DateTime?>().ConvertUsing<Utils.TypeConverters.StringToNullableDateTimeTypeConverter>();
            AutoMapper.Mapper.CreateMap<DateTime, string>().ConvertUsing<Utils.TypeConverters.DateTimeToStringTypeConverter>();*/
            AutoMapper.Mapper.Map<S, T>(source, entity);
            return entity;
        }

        protected virtual E ModelToEntity(VM model)
        {
            return this.Map<VM, E>(model);
        }

        protected virtual VM EntityToModel(E entity)
        {
            return this.Map<E, VM>(entity);
        }

        protected virtual List<T> MapList<S, T>(IEnumerable<S> source)
           where T : class, new()
        {
            if (source == null)
                return null;
            if (typeof(T) == typeof(S))
                return source as List<T>;

            T entity = new T();
            return AutoMapper.Mapper.Map<List<T>>(source);
        }


        protected virtual void PopulateViewBagForEdit(VM model)
        {
            var e = new E();
            if(e is Entities.Base.IActivableEntity)
                this.AddBooleanItems();
        }
        protected virtual void PopulateViewBagForDetails(VM model)
        {
            var e = new E();
            if (e is Entities.Base.IActivableEntity)
                this.AddBooleanItems();
        }
        protected virtual void PopulateViewBagForCreate(VM model)
        {

        }

        protected virtual void PopulateViewBagForIndex()
        {
            AddBooleanFilter();
            this.GetActions();
        }       

        public virtual void GetActions()
        {
            string nombreController = this.ControllerContext.RouteData.Values["controller"].ToString();

            string accionVer = string.Format("{0}.Details", nombreController);
            string accionCrear = string.Format("{0}.Create", nombreController);
            string accionEditar = string.Format("{0}.Edit", nombreController);
            string accionEliminar = string.Format("{0}.Delete", nombreController);

            bool permiteVer = UserBusiness.Authorize(accionVer);
            bool permiteCrear = UserBusiness.Authorize(accionCrear);
            bool permiteEditar = UserBusiness.Authorize(accionEditar);
            bool permiteEliminar = UserBusiness.Authorize(accionEliminar);

            ViewBag.PermiteVer = permiteVer;
            ViewBag.PermiteCrear = permiteCrear;
            ViewBag.PermiteEditar = permiteEditar;
            ViewBag.PermiteEliminar = permiteEliminar;
        }

        public virtual ActionResult Create()
        {
            VM model = new VM();
            PopulateViewBagForCreate(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Create(VM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    E entity = ModelToEntity(model);
                    Business.Insert(entity);
                    AddGenericMessage(GenericMessage.CreateOK);
                    //model = EntityToModel(entity);
                    return ActionIndexOrCloseWindow(model);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            PopulateViewBagForCreate(model);

            return View(model);
        }

        protected string GetModelStateValidationErrors(ModelStateDictionary modelState)
        {
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            return messages;
        }

        public virtual ActionResult Edit(TID id)
        {
            E entity = ReadForEditOrDetail(id);
            VM model = EntityToModel(entity);
            PopulateViewBagForEdit(model);
            return View(model);
        }

        protected virtual E ReadForEditOrDetail(TID id)
        {
            return Business.Read(id);
        }

        [HttpPost]
        public virtual ActionResult Edit(VM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    E entity = ModelToEntity(model);
                    Business.Update(entity);
                    AddGenericMessage(GenericMessage.UpdateOK);
                    return ActionIndexOrCloseWindow(model);
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
            PopulateViewBagForEdit(model);
            return View(model);
        }

        protected virtual RedirectToRouteResult ActionIndexOrCloseWindow(VM model)
        {
            if (Request.Params["modal"] != null &&
                Request.Params["modal"].ToLower() == "true" &&
                Request.Params["callback"] != null)
            {
                TempData["values"] = model;
                TempData["callback"] = Request.Params["callback"];
                TempData["senderId"] = Request.Params["senderId"];
                return RedirectToAction("CallParentWindow", "Modal");
            }
            else
                return RedirectToAction("Index");
        }

        public virtual ViewResult Details(TID id)
        {
            E entity = ReadForEditOrDetail(id);
            VM model = EntityToModel(entity);
            PopulateViewBagForDetails(model);
            return View(model);
        }

        public virtual ActionResult Delete(object id)
        {
            E entity = Business.Read(id);
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        public virtual JsonResult DeleteConfirmed(TID id)
        {
            try
            {
                Business.Delete(id);
            }
            catch (ValidationException ex)
            {
                return Json(HandleJSONValidationException(ex), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }



        protected string HandleJSONValidationException(ValidationException ex)
        {
            string errorMessage = string.Empty;

            if (ex.KeyNames != null)
                errorMessage = string.Format(HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString(), ex.KeyNames);
            else
                errorMessage = HttpContext.GetGlobalResourceObject("Resources", ex.Message).ToString();
         
            /*
            if (val.Parameters != null)
                errorMessage = string.Format(errorMessage, val.Parameters);
                */
            return errorMessage;
        }

        protected string GetEntityValidationErrors(DbEntityValidationException ex)
        {
            List<string> errorMessages = new List<string>();
            foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
            {
                string entityName = validationResult.Entry.Entity.GetType().Name;
                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
            return string.Join("; ", errorMessages);
        }        
    }
}