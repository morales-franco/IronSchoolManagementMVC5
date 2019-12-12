using System.Web;
using System.Web.Optimization;

namespace IronSchool.WebSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            
            bundles.Add(new StyleBundle("~/Content/ma").Include(
                      "~/Content/ma/animate.css",
                      "~/Content/ma/sweetalert2.css",
                      "~/Content/ma/material-design-iconic-font.css",
                      "~/Content/ma/jquery.mCustomScrollbar.css",
                      "~/Content/ma/app_1.css",
                      "~/Content/ma/app_2.css"));

            bundles.Add(new ScriptBundle("~/bundles/ma").Include(
                      "~/Scripts/ma/jquery.mCustomScrollbar.concat.js",
                      "~/Scripts/ma/waves.js",
                      "~/Scripts/ma/bootstrap-growl.js",
                      "~/Scripts/ma/sweetalert2.js",
                      "~/Scripts/ma/jquery.mask.js",
                      "~/Scripts/ma/app.js"));

            bundles.Add(new StyleBundle("~/bundles/kendoStyles").Include(
                      "~/Content/styles/kendo.common.css",
                      "~/Content/styles/kendo.material.css",
                      "~/Content/styles/kendo.material.mobile.css"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                      "~/Scripts/kendo/kendo.all.js",
                      "~/Scripts/kendo/kendo.aspnetmvc.js",
                      "~/Scripts/kendo/messages/kendo.messages.es-AR.js"));
        }
    }
}
