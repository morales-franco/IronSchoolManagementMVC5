using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.Utils;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;
using static IronSchool.WebSite.Models.AccountModels;

namespace IronSchool.WebSite.Controllers
{
    public class AccountController : BaseController
    {
        [AuthorizeRule(true)]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeRule(true)]
        public ActionResult Login(LoginRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var biz = new UserBusiness();
                    var user = biz.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    this.HandleException(ex);
                }
            }
            return View(model);
        }

        [AuthorizeRule(true)]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeRule(true)]
        public ActionResult RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserBusiness buss = new UserBusiness();
                User user = buss.Get(x => x.UserName.ToUpper() == model.UserName.ToUpper());

                if (user == null)
                {
                    ModelState.AddModelError("", ResourcesHelper.GetResourceValue("UserNotExist"));
                    return View(model);
                }
                else
                {
                    if (user.Email == null || !user.Active)
                        ModelState.AddModelError("", ResourcesHelper.GetResourceValue("RequireAdminChangePassword"));
                    else
                    {
                        byte[] time = BitConverter.GetBytes(DateTime.Now.ToBinary());
                        byte[] key = Guid.NewGuid().ToByteArray();
                        string token = Convert.ToBase64String(time.Concat(key).ToArray());

                        string link = Url.Action("ResetPassword", "Account", new { id = user.UserId, code = token }, Request.Url.Scheme);
                        string mail = string.Format(ResourcesHelper.GetResourceValue("MailChangePassword"), link);

                        EmailSender.Send_NoAsync(user.Email, ResourcesHelper.GetResourceValue("RecoverPassword"), mail);

                        return RedirectToAction("PasswordSend");
                    }
                }
            }
            return View(model);
        }

        [AuthorizeRule(true)]
        public ActionResult Logout()
        {
            var biz = new UserBusiness();
            biz.Logout();
            return RedirectToAction("Login");
        }

        [AuthorizeRule(true)]
        public ActionResult PasswordSend()
        {
            return View();
        }

        [AuthorizeRule(true)]
        public ActionResult ResetPassword(long id, string code)
        {
            if (id == 0 || string.IsNullOrWhiteSpace(code))
                return View("Error");

            ResetPasswordVM model = new ResetPasswordVM();
            model.UserId = id;
            model.Code = code;

            return View(model);
        }
        
        [HttpPost]
        [AuthorizeRule(true)]
        public ActionResult ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                byte[] data = Convert.FromBase64String(model.Code);
                DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                if (when < DateTime.Now.AddHours(-24))
                    ModelState.AddModelError("", ResourcesHelper.GetResourceValue("InvalidToken"));
                else
                {
                    new UserBusiness().ResetPassword(model.UserId, model.Password);
                    return RedirectToAction("ResetPasswordSuccess");
                }
            }
            return View(model);
        }

        [AuthorizeRule(true)]
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

    }
}