using IronSchool.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace IronSchool.WebSite.Helpers
{
    public static class HtmlHelpers
    {
        static IDictionary<string, object> MergeAnonymous(object obj1, object obj2)
        {
            var dict1 = new RouteValueDictionary(obj1);
            var dict2 = new RouteValueDictionary(obj2);
            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var pair in dict1.Concat(dict2))
            {
                if(!result.ContainsKey(pair.Key)) //le doy prioridad a las keys de obj1
                    result.Add(pair);
            }

            return result;
        }

        public static MvcHtmlString FilterField(this HtmlHelper html, string filedName, string labelName, int colSize, Object htmlAttributes = null, object value = null, string format = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(html.ViewContext, html.ViewDataContainer);

            var fieldContainer = new TagBuilder("div");
            fieldContainer.AddCssClass("col-md-" + colSize);
            var fg = new TagBuilder("div");
            fg.AddCssClass("form-group");
            var fgLine = new TagBuilder("div");
            fgLine.AddCssClass("fg-line");

            fgLine.InnerHtml = htmlaux.Label(labelName, new { @class = "control-label" }).ToHtmlString();

            var att = new { @class = "form-control fg-input" };

            fgLine.InnerHtml += htmlaux.TextBox(filedName, value, format, MergeAnonymous(htmlAttributes, att));

            fg.InnerHtml += fgLine.ToString();
            fieldContainer.InnerHtml = fg.ToString();
            return MvcHtmlString.Create(fieldContainer.ToString());
        }

        public static MvcHtmlString FormFieldFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int colSize, object htmlAttributes = null, string format = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);

            var fieldContainer = new TagBuilder("div");
            fieldContainer.AddCssClass("col-md-" + colSize);
            var fg = new TagBuilder("div");
            fg.AddCssClass("form-group");
            var fgLine = new TagBuilder("div");
            fgLine.AddCssClass("fg-line");

            fgLine.InnerHtml = htmlHelper.LabelFor<TModel, TProperty>(expression, new { @class = "control-label" }).ToString();
            if (expression.ReturnType.Name == "DateTime")
                fgLine.InnerHtml += htmlHelper.TextBoxFor<TModel, TProperty>(expression, format ?? "{0:yyyy-MM-dd}", MergeAnonymous(htmlAttributes, new { @class = "form-control fg-input" })).ToString();
            else
                fgLine.InnerHtml += htmlHelper.TextBoxFor<TModel, TProperty>(expression, format, MergeAnonymous(htmlAttributes, new { @class = "form-control fg-input" })).ToString();

            fg.InnerHtml += fgLine.ToString();
            fg.InnerHtml += htmlHelper.ValidationMessageFor(expression);

            fieldContainer.InnerHtml = fg.ToString();
            return MvcHtmlString.Create(fieldContainer.ToString());
        }

        public static MvcHtmlString FormSelectFieldFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, int colSize, object htmlAttributes = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);

            var fieldContainer = new TagBuilder("div");
            fieldContainer.AddCssClass("col-md-" + colSize);
            var fg = new TagBuilder("div");
            fg.AddCssClass("form-group");
            var fgLine = new TagBuilder("div");
            fgLine.AddCssClass("fg-line");

            fgLine.InnerHtml = htmlHelper.LabelFor<TModel, TProperty>(expression, new { @class = "control-label" }).ToString();

            var divSelect = new TagBuilder("div");
            divSelect.AddCssClass("select");
            divSelect.InnerHtml = htmlHelper.DropDownListFor(expression, selectList, optionLabel, MergeAnonymous(htmlAttributes, new { @class = "form-control" })).ToString();

            fgLine.InnerHtml += divSelect.ToString();

            fg.InnerHtml += fgLine.ToString();
            fg.InnerHtml += htmlHelper.ValidationMessageFor(expression);

            fieldContainer.InnerHtml = fg.ToString();
            return MvcHtmlString.Create(fieldContainer.ToString());
        }

        public static MvcHtmlString FormEntitySelectFieldFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, int colSize, string controllerName, object htmlAttributes = null, object routeValues = null, string dataUrl = "")
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);
            UrlHelper u = new UrlHelper(htmlHelper.ViewContext.Controller.ControllerContext.RequestContext);

            var fieldContainer = new TagBuilder("div");
            fieldContainer.AddCssClass("col-md-" + colSize);
            var formGroup = new TagBuilder("div");
            formGroup.AddCssClass("form-group browser-form-group");
            var inputGroup = new TagBuilder("div");
            inputGroup.AddCssClass("input-group");
            var fgLine = new TagBuilder("div");
            fgLine.AddCssClass("fg-line");

            var divSelect = new TagBuilder("div");
            divSelect.AddCssClass("select");
            if (String.IsNullOrEmpty(dataUrl))
                dataUrl = u.Action("GetSelectItems", controllerName);

            if (htmlAttributes == null)
                divSelect.InnerHtml = htmlHelper.DropDownListFor(expression, selectList, optionLabel, new { @class = "form-control", data_url = dataUrl }).ToString();
            else
                divSelect.InnerHtml = htmlHelper.DropDownListFor(expression, selectList, optionLabel, MergeAnonymous(htmlAttributes, new { @class = "form-control", data_url = dataUrl })).ToString();

            fgLine.InnerHtml += divSelect.ToString();

            inputGroup.InnerHtml += fgLine.ToString();

            bool createEnabled = UserBusiness.Authorize(string.Format("{0}.Create", controllerName));
            bool editEnabled = UserBusiness.Authorize(string.Format("{0}.Create", controllerName));

            if (createEnabled || editEnabled)
            {
                var addons = new TagBuilder("span");
                addons.AddCssClass("input-group-addon last browser-addon");

                if (createEnabled)
                {
                    var createLink = new TagBuilder("a");
                    createLink.Attributes.Add("href", "#");
                    createLink.Attributes.Add("onclick", "window.open('" + u.Action("Create", controllerName, new RouteValueDictionary(MergeAnonymous(routeValues, new { modal = true, senderId = htmlHelper.IdFor(expression), callback = "reloadSelectFromModal" }))) + "', '" + Resources.Resources.Create + "')");
                    createLink.Attributes.Add("title", Resources.Resources.Create);
                    createLink.InnerHtml = "<i class='zmdi zmdi-plus'></i>";
                    addons.InnerHtml += createLink.ToString();
                }

                if (editEnabled)
                {
                    var editLink = new TagBuilder("a");
                    editLink.Attributes.Add("href", "#");
                    editLink.Attributes.Add("onclick", "editSelectEntity('" + u.Action("Edit", controllerName, new { id = "elementId", modal = true, senderId = htmlHelper.IdFor(expression), callback = "reloadSelectFromModal" }) + "', this)");
                    editLink.Attributes.Add("title", Resources.Resources.Edit);
                    editLink.InnerHtml = "<i class='zmdi zmdi-edit'></i>";
                    if (createEnabled)
                        addons.InnerHtml += "&nbsp;";

                    addons.InnerHtml += editLink.ToString();
                   
                }

                inputGroup.InnerHtml += addons.ToString();
            }
            
            formGroup.InnerHtml = htmlHelper.LabelFor<TModel, TProperty>(expression, new { @class = "control-label" }).ToString();
            formGroup.InnerHtml += inputGroup.ToString();
            fieldContainer.InnerHtml = formGroup.ToString();
            fieldContainer.InnerHtml += htmlHelper.ValidationMessageFor(expression);
            return MvcHtmlString.Create(fieldContainer.ToString());
        }

        public static MvcHtmlString FormDisplayFieldFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int colSize, object htmlAttributes = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);

            var fieldContainer = new TagBuilder("div");
            fieldContainer.AddCssClass("col-md-" + colSize);
            var fg = new TagBuilder("div");
            fg.AddCssClass("form-group");
            var fgLine = new TagBuilder("div");
            fgLine.AddCssClass("fg-line");

            fgLine.InnerHtml = htmlHelper.LabelFor<TModel, TProperty>(expression, new { @class = "control-label" }).ToString();
            fgLine.InnerHtml += htmlHelper.DisplayFor<TModel, TProperty>(expression, null, MergeAnonymous(htmlAttributes, new { @class = "form-control fg-input" })).ToString();

            fg.InnerHtml += fgLine.ToString();
            fg.InnerHtml += htmlHelper.ValidationMessageFor(expression);

            fieldContainer.InnerHtml = fg.ToString();
            return MvcHtmlString.Create(fieldContainer.ToString());
        }
        public static MvcHtmlString Submit(this HtmlHelper html, string text)
        {
            return MvcHtmlString.Create(String.Format("<button type='submit' class='btn btn-primary waves-effect'>{0}</button>", text));
        }

        public static String ActionsColumn(this HtmlHelper htmlHelper, string pkFieldName)
        {
            UrlHelper u = new UrlHelper(htmlHelper.ViewContext.Controller.ControllerContext.RequestContext);

            string controllerName = htmlHelper.ViewContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();

            string htmlString = "";
            if(UserBusiness.Authorize(string.Format("{0}.Details", controllerName)))
                htmlString += "<a title='" + Resources.Resources.Details + "' href='" + u.Action("Details") + "?id=#= " + pkFieldName + " #'><i class='grid-action-icon zmdi-view-headline'></i></a>";

            if (UserBusiness.Authorize(string.Format("{0}.Edit", controllerName)))
                htmlString += "<a title='" + Resources.Resources.Edit + "' href='" + u.Action("Edit") + "?id=#= " + pkFieldName + " #'><i class='grid-action-icon zmdi-edit'></i></a>";

            if (UserBusiness.Authorize(string.Format("{0}.Delete", controllerName)))
                htmlString += "<a title='" + Resources.Resources.Delete + "' href='\\#' onclick='confirmDelete(\"" + u.Action("Delete") + "\", #= " + pkFieldName + " #)'><i class='grid-action-icon zmdi-delete'></i></a>";

            return htmlString;
        }

        public static int CrudPageSize(this HtmlHelper htmlHelper)
        {
            return int.Parse(ConfigurationManager.AppSettings["CRUDPageSize"]);
        }

        public static bool IsModalWindow(this HtmlHelper htmlHelper)
        {
            return (htmlHelper.ViewContext.Controller.ControllerContext.HttpContext.Request.Params["modal"] != null &&
                htmlHelper.ViewContext.Controller.ControllerContext.HttpContext.Request.Params["modal"].ToLower() == "true");
        }

        public static MvcHtmlString BackButton(this HtmlHelper html)
        {
            if (!html.IsModalWindow())
                return html.ActionLink(Resources.Resources.Back, "Index", null, new { @class = "btn btn-default waves-effect" });
            else
                return new MvcHtmlString("<a href='#' onclick='window.close()' class='btn btn-default waves-effect'>" + Resources.Resources.Back + "</a>");
        }

        public static MvcHtmlString BrowserFor<TModel, TProperty, TProperty1, TProperty2>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> idExpression, Expression<Func<TModel, TProperty1>> codeExpression, Expression<Func<TModel, TProperty2>> descriptionExpression, int colSize, string controllerName, string getByCodeAction = "GetByCode", string browserAction = "Browser", object routeValues = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);
            UrlHelper u = new UrlHelper(htmlHelper.ViewContext.Controller.ControllerContext.RequestContext);

            bool createEnabled = UserBusiness.Authorize(string.Format("{0}.Create", controllerName));

            StringBuilder sb = new StringBuilder();
            if(colSize > 0)
                sb.AppendLine(String.Format("<div class='col-md-{0}'>", colSize));
            sb.AppendLine("<div class='form-group browser-form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(descriptionExpression, new { @class = "control-label" }).ToString());
            sb.AppendLine("<div class='input-group'>");
            sb.AppendLine(htmlHelper.HiddenFor(idExpression, new { @class = "browser-id" }).ToString());
            sb.AppendLine("<div class='col-xs-3 no-left-padding'>");
            sb.AppendLine("<div class='fg-line'>");
            sb.AppendLine(htmlHelper.TextBoxFor(codeExpression, new { @class = "form-control fg-input browser-code", onblur = "browserGetByCode('" + u.Action(getByCodeAction, controllerName) + "', this)" }).ToString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class='col-xs-9 no-left-padding'>");
            sb.AppendLine("<div class='fg-line'>");
            sb.AppendLine(htmlHelper.TextBoxFor(descriptionExpression, new { @class = "form-control fg-input browser-desc", @readonly = "readonly" }).ToString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<span class='input-group-addon last browser-addon'>");
            sb.AppendLine("<a href='#' onclick='showBrowser(\"" + u.Action(browserAction, controllerName, routeValues) + "\", this)' title='" + Resources.Resources.Search + "'>");
            sb.AppendLine("<i class='zmdi zmdi-search'></i>");
            sb.AppendLine("</a>");
            if (createEnabled)
            {
                sb.AppendLine("&nbsp;");
                sb.AppendLine("<a href='#' onclick=\"window.open('" + u.Action("Create", controllerName, MergeAnonymous(routeValues, new { modal = true, senderId = htmlHelper.IdFor(idExpression), callback = "setCreatedElement" })) + "', '" + Resources.Resources.Create + "')\" title='" + Resources.Resources.Create + "'>");
                sb.AppendLine("<i class='zmdi zmdi-plus'></i>");
                sb.AppendLine("</a>");               
            }
            sb.AppendLine("</span>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(descriptionExpression).ToString());
            if (colSize > 0)
                sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString BrowserFor<TModel, TProperty, TProperty1>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> idExpression, Expression<Func<TModel, TProperty1>> descriptionExpression, int colSize, string controllerName, string browserAction = "Browser", object routeValues = null)
        {
            HtmlHelper htmlaux = new HtmlHelper(htmlHelper.ViewContext, htmlHelper.ViewDataContainer);
            UrlHelper u = new UrlHelper(htmlHelper.ViewContext.Controller.ControllerContext.RequestContext);

            bool createEnabled = UserBusiness.Authorize(string.Format("{0}.Create", controllerName));

            StringBuilder sb = new StringBuilder();
            if (colSize > 0)
                sb.AppendLine(String.Format("<div class='col-md-{0}'>", colSize));
            sb.AppendLine("<div class='form-group browser-form-group'>");
            sb.AppendLine(htmlHelper.LabelFor(descriptionExpression, new { @class = "control-label" }).ToString());
            sb.AppendLine("<div class='input-group'>");
            sb.AppendLine(htmlHelper.HiddenFor(idExpression, new { @class = "browser-id" }).ToString());
            sb.AppendLine("<div class='col-xs-12 no-left-padding'>");
            sb.AppendLine("<div class='fg-line'>");
            sb.AppendLine(htmlHelper.TextBoxFor(descriptionExpression, new { @class = "form-control fg-input browser-desc", @readonly = "readonly" }).ToString());
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<span class='input-group-addon last browser-addon'>");
            sb.AppendLine("<a href='#' onclick='showBrowser(\"" + u.Action(browserAction, controllerName, routeValues) + "\", this)' title='" + Resources.Resources.Search + "'>");
            sb.AppendLine("<i class='zmdi zmdi-search'></i>");
            sb.AppendLine("</a>");
            if (createEnabled)
            {
                sb.AppendLine("&nbsp;");
                sb.AppendLine("<a href='#' onclick=\"window.open('" + u.Action("Create", controllerName, MergeAnonymous(routeValues, new { modal = true, senderId = htmlHelper.IdFor(idExpression), callback = "setCreatedElement" })) + "', '" + Resources.Resources.Create + "')\" title='" + Resources.Resources.Create + "'>");
                sb.AppendLine("<i class='zmdi zmdi-plus'></i>");
                sb.AppendLine("</a>");
            }
            sb.AppendLine("</span>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(descriptionExpression).ToString());
            if (colSize > 0)
                sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }


        public static string Truncate(this HtmlHelper helper, string input, int length)
        {
            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + "...";
            }
        }

        public static MvcHtmlString DisplayLabelFor<TModel, TValue>(this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> html, Expression<Func<TModel, TValue>> expression)
        {
            HtmlHelper<TModel> htmlaux = new HtmlHelper<TModel>(html.ViewContext, html.ViewDataContainer);
            return htmlaux.LabelFor<TModel, TValue>(expression);
        }

        public static MvcHtmlString DisplayColumnNameFor<TModel, TClass, TProperty>(this HtmlHelper<TModel> helper, IEnumerable<TClass> model, Expression<Func<TClass, TProperty>> expression)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => Activator.CreateInstance<TClass>(), typeof(TClass), name);

            return new MvcHtmlString(metadata.PropertyName);
        }

        public static MvcHtmlString ActionLinkWithIcon(this HtmlHelper helper, String linktext, String IconClass, String actionname, String controllername = null, Object routevalues = null, Object htmlattributes = null)
        {
            String htmltag = "<i class='" + IconClass + "'>&nbsp " + linktext + "</i>";
            var tagActionLink = helper.ActionLink("[replace]", actionname, controllername, routevalues, htmlattributes).ToHtmlString();
            MvcHtmlString htmlstring = new MvcHtmlString(tagActionLink.Replace("[replace]", htmltag));
            return htmlstring;
        }

        public static MvcHtmlString ActionLinkWithParameters(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, string[] queryStringParameters, object htmlAttributes = null, string iconClass = null)
        {
            StringWriter scriptContent = new StringWriter();

            scriptContent.WriteLine("var search" + controllerName + actionName + " = '';");

            scriptContent.WriteLine("function RewriteHREF" + controllerName + actionName + "(obj) {");

            int idx = 0;

            scriptContent.WriteLine("if (search" + controllerName + actionName + " == '') {");
            scriptContent.WriteLine("search" + controllerName + actionName + " = obj.search;");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = " + "search" + controllerName + actionName + ";");
            scriptContent.WriteLine("}");

            scriptContent.WriteLine("if (obj.search.indexOf('?') != -1) {");
            scriptContent.WriteLine("obj.search = obj.search + '&';");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = obj.search + '?';");
            scriptContent.WriteLine("}");

            foreach (string key in queryStringParameters)
            {
                scriptContent.WriteLine("if ($('#" + key + "').is('select')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + " option:selected').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("if ($('#" + key + "').is('input')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + "').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("obj.search = obj.search + '" + key + "=' + value" + idx + " + '&';");

                idx++;
            }

            scriptContent.WriteLine("}");

            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.SetInnerText(scriptContent.ToString());

            var builder = new TagBuilder("i");
            if (!string.IsNullOrEmpty(iconClass))
            {
                builder.MergeAttribute("class", iconClass);
                linkText = "[replaceicon]";
            }

            MvcHtmlString actionLink = ajaxHelper.ActionLink(linkText, actionName, controllerName, ajaxOptions, htmlAttributes);

            actionLink = new MvcHtmlString(actionLink.ToHtmlString().Replace("[replaceicon]", builder.ToString(TagRenderMode.Normal)));

            MvcHtmlString response = new MvcHtmlString(ajaxHelper.ViewContext.HttpContext.Server.HtmlDecode(scriptTag.ToString(TagRenderMode.Normal)) + actionLink.ToHtmlString().Insert(3, "onclick='javascript:RewriteHREF" + controllerName + actionName + "(this);' "));

            return response;
        }

        public static MvcHtmlString ActionLinkWithParameters(this HtmlHelper HtmlHelper, string linkText, string actionName, string controllerName, string[] queryStringParameters, object htmlAttributes = null)
        {
            StringWriter scriptContent = new StringWriter();

            scriptContent.WriteLine("var search" + controllerName + actionName + " = 'empty';");

            scriptContent.WriteLine("function RewriteHREF" + controllerName + actionName + "(obj) {");

            int idx = 0;

            scriptContent.WriteLine("if (search" + controllerName + actionName + " == 'empty') {");
            scriptContent.WriteLine("search" + controllerName + actionName + " = obj.search;");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = " + "search" + controllerName + actionName + ";");
            scriptContent.WriteLine("}");

            scriptContent.WriteLine("if (obj.search.indexOf('?') != -1) {");
            scriptContent.WriteLine("obj.search = obj.search + '&';");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = obj.search + '?';");
            scriptContent.WriteLine("}");

            foreach (string key in queryStringParameters)
            {
                scriptContent.WriteLine("if ($('#" + key + "').is('select')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + " option:selected').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("if ($('#" + key + "').is('input')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + "').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("obj.search = obj.search + '" + key + "=' + value" + idx + " + '&';");

                idx++;
            }

            scriptContent.WriteLine("}");

            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.SetInnerText(scriptContent.ToString());

            MvcHtmlString actionLink = HtmlHelper.ActionLink(linkText, actionName, controllerName, htmlAttributes);

            MvcHtmlString response = new MvcHtmlString(HtmlHelper.ViewContext.HttpContext.Server.HtmlDecode(scriptTag.ToString(TagRenderMode.Normal)) + actionLink.ToHtmlString().Insert(3, "onclick='javascript:RewriteHREF" + controllerName + actionName + "(this);' "));

            return response;
        }

        public static MvcHtmlString ActionLinkWithParametersIcon(this HtmlHelper HtmlHelper, string linkText, String IconClass, string actionName, string controllerName, string[] queryStringParameters, object htmlAttributes = null)
        {
            StringWriter scriptContent = new StringWriter();

            scriptContent.WriteLine("var search" + controllerName + actionName + " = 'empty';");

            scriptContent.WriteLine("function RewriteHREF" + controllerName + actionName + "(obj) {");

            int idx = 0;

            scriptContent.WriteLine("if (search" + controllerName + actionName + " == 'empty') {");
            scriptContent.WriteLine("search" + controllerName + actionName + " = obj.search;");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = " + "search" + controllerName + actionName + ";");
            scriptContent.WriteLine("}");

            scriptContent.WriteLine("if (obj.search.indexOf('?') != -1) {");
            scriptContent.WriteLine("obj.search = obj.search + '&';");
            scriptContent.WriteLine("} else {");
            scriptContent.WriteLine("obj.search = obj.search + '?';");
            scriptContent.WriteLine("}");

            foreach (string key in queryStringParameters)
            {
                scriptContent.WriteLine("if ($('#" + key + "').is('select')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + " option:selected').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("if ($('#" + key + "').is('input')) {");
                scriptContent.WriteLine("var value" + idx + " = $('#" + key + "').val();");
                scriptContent.WriteLine("}");
                scriptContent.WriteLine("obj.search = obj.search + '" + key + "=' + value" + idx + " + '&';");

                idx++;
            }

            scriptContent.WriteLine("}");

            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.SetInnerText(scriptContent.ToString());

            String htmltag = "<i class='" + IconClass + "'>" + linkText + "</i>";
            var tagActionLink = HtmlHelper.ActionLink("[replace]", actionName, controllerName, htmlAttributes).ToHtmlString();
            MvcHtmlString actionLink = new MvcHtmlString(tagActionLink.Replace("[replace]", htmltag));

            MvcHtmlString response = new MvcHtmlString(HtmlHelper.ViewContext.HttpContext.Server.HtmlDecode(scriptTag.ToString(TagRenderMode.Normal)) + actionLink.ToHtmlString().Insert(3, "onclick='javascript:RewriteHREF" + controllerName + actionName + "(this);' "));

            return response;
        }

        public static MvcHtmlString DeleteLink(this HtmlHelper helper, String linktext, string itemID, Object routevalues = null, Object htmlattributes = null)
        {
            MvcHtmlString htmlstring = new MvcHtmlString("<a onclick='validateDelete('@Url.Action('ValidateDelete')', '" + itemID + "')'>" + linktext + "</a>");
            return htmlstring;
        }
        
        public static MvcHtmlString CreateMultiDatepicker(this HtmlHelper HtmlHelper, string divId, int monthPosition = 5, bool disabled = false)
        {
            HtmlString htmlstring = new HtmlString("<div id='" + divId + "'></div>");
            StringWriter scriptContent = new StringWriter();
            scriptContent.WriteLine("$(function(){$(" + divId + ").multiDatesPicker({");
            scriptContent.WriteLine("closeText: '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Close") + "',");
            scriptContent.WriteLine("prevText: '<" + ResourcesHelper.GetResourceValue(HtmlHelper, "Prev") + "',");
            scriptContent.WriteLine("nextText: '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Next") + ">',");
            scriptContent.WriteLine("currentText: '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Today") + "',");
            scriptContent.WriteLine("monthNames: ['" + ResourcesHelper.GetResourceValue(HtmlHelper, "Enero") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Febrero") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Marzo") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Abril") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Mayo") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Junio") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Julio") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Agosto") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Septiembre") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Octubre") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Noviembre") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Diciembre") + "'],");
            scriptContent.WriteLine("monthNamesShort: ['" + ResourcesHelper.GetResourceValue(HtmlHelper, "Enero").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Febrero").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Marzo").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Abril").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Mayo").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Junio").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Julio").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Agosto").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Septiembre").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Octubre").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Noviembre").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Diciembre").Substring(0, 3) + "'],");
            scriptContent.WriteLine("dayNames: ['" + ResourcesHelper.GetResourceValue(HtmlHelper, "Domingo") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Lunes") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Martes") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Miercoles") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Jueves") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Viernes") + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Sabado") + "'],");
            scriptContent.WriteLine("dayNamesShort: ['" + ResourcesHelper.GetResourceValue(HtmlHelper, "Domingo").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Lunes").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Martes").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Miercoles").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Jueves").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Viernes").Substring(0, 3) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Sabado").Substring(0, 3) + "'],");
            scriptContent.WriteLine("dayNamesMin: ['" + ResourcesHelper.GetResourceValue(HtmlHelper, "Domingo").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Lunes").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Martes").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Miercoles").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Jueves").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Viernes").Substring(0, 2) + "', '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Sabado").Substring(0, 2) + "'],");
            scriptContent.WriteLine("weekHeader: '" + ResourcesHelper.GetResourceValue(HtmlHelper, "Wk") + "',");
            scriptContent.WriteLine("dateFormat: 'dd/mm/yy',");
            scriptContent.WriteLine("firstDay: 1,");
            scriptContent.WriteLine("numberOfMonths: [2, 3],");
            scriptContent.WriteLine("showWeek: true,");
            scriptContent.WriteLine("selectWeek: true,");
            if (disabled)
            {
                scriptContent.WriteLine("disabled: true,");
            }
            scriptContent.WriteLine("defaultDate: '-" + monthPosition + "m'");
            scriptContent.WriteLine("}).prop('readonly'); eventos();});");
            TagBuilder scriptTag = new TagBuilder("script");
            scriptTag.MergeAttribute("type", "text/javascript");
            scriptTag.SetInnerText(scriptContent.ToString());
            MvcHtmlString response = new MvcHtmlString(HtmlHelper.ViewContext.HttpContext.Server.HtmlDecode(scriptTag.ToString(TagRenderMode.Normal)) + htmlstring);
            return response;
        }
                
    }
}