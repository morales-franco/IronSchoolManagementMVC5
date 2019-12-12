using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IronSchool.WebSite.Controllers
{
    public class ModalController : Controller
    {
        public ActionResult CallParentWindow()
        {
            ViewBag.values = TempData["values"];
            ViewBag.callback = TempData["callback"];
            ViewBag.senderId = TempData["senderId"];
            return View();
        }
    }
}