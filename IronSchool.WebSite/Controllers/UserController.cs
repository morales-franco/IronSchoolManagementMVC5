using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Filters;
using IronSchool.WebSite.Models;

namespace IronSchool.WebSite.Controllers
{
    public class UserController : BaseABMController<UserBusiness, Entities.User, Models.UserIndexVM, Models.UserVM, long>
    {
        protected override User ModelToEntity(UserVM model)
        {
            User e = base.ModelToEntity(model);
            if(model.UserId <= 0) //is a create
                e.PasswordHash = Utils.Encryption.Encrypt(model.Password);
            else //preserve passwordhash
            {
                var dbU = this.Business.Get(x => x.UserId == model.UserId);
                e.PasswordHash = dbU.PasswordHash;
            }
            return e;
        }

        public override void GetActions()
        {
            base.GetActions();
            ViewBag.ResetPassword = UserBusiness.Authorize("User.ResetPassword");
        }
        
        public ActionResult ResetPassword(long id)
        {
            var user = this.Business.Read(id);
            var model = new ResetPasswordVM()
            {
                UserId = user.UserId,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Business.ResetPassword(model.UserId, model.Password);
                    AddGenericMessage(GenericMessage.UpdateOK);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            return View(model);
        }

        public ActionResult MyProfile()
        {
            long id = UserBusiness.Current.UserId;
            User entity = ReadForEditOrDetail(id);
            MyProfileVM model = AutoMapper.Mapper.Map<MyProfileVM>(entity);
            ShowGenericMessage();
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult MyProfile(MyProfileVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User entity = AutoMapper.Mapper.Map<User>(model);
                    
                    //preserve password and active
                    var dbU = this.Business.Get(x => x.UserId == model.UserId);
                    entity.PasswordHash = dbU.PasswordHash;
                    entity.Active = true;

                    Business.Update(entity);
                    Business.RefreshApplicationUser();
                    AddGenericMessage(GenericMessage.UpdateOK);
                    return RedirectToAction("MyProfile");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }
            return View(model);
        }

        public ActionResult ResetMyPassword()
        {
            long id = UserBusiness.Current.UserId;
            var user = this.Business.Read(id);
            var model = new ResetPasswordVM()
            {
                UserId = user.UserId,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetMyPassword(ResetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Business.ResetPassword(model.UserId, model.Password);
                    AddGenericMessage(GenericMessage.UpdateOK);
                    return RedirectToAction("MyProfile");
                }
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
            }

            return View(model);
        }

        [AuthorizeRule("Index", true)]
        public JsonResult GetSelectItems()
        {
            return Json(UserController.GetSelectListItems(), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRule("Index", true)]
        public ActionResult Browser()
        {
            return View();
        }

        [AuthorizeRule("Details", true)]
        public JsonResult GetByCode(string code)
        {
            var item = this.Business.Get(x => x.UserName == code);

            if (item != null)
            {
                var result = new
                {
                    id = item.UserId,
                    code = item.UserName,
                    description = $"{item.LastName} {item.FirstName}",
                    email = item.Email
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        public static SelectList GetSelectListItems()
        {
            var items = new UserBusiness().GetList(x=> x.Active)
                    .OrderBy(x => x.FirstName + x.LastName)
                    .Select(x=> new {
                        id = x.UserId,
                        text = x.FirstName + " " + x.LastName});

            return new SelectList(items, "id", "text");
        }


    }
}