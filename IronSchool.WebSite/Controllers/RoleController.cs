using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IronSchool.Business;
using IronSchool.Entities;
using IronSchool.WebSite.Controllers.Base;
using IronSchool.WebSite.Models;

namespace IronSchool.WebSite.Controllers
{
    public class RoleController : BaseABMController<RoleBusiness, Entities.Role, Models.RoleIndexVM, Models.RoleVM, long>
    {
       
        protected override void PopulateViewBagForCreate(RoleVM model)
        {
            base.PopulateViewBagForCreate(model);

            var bizRule = new RuleBusiness();
            var rules = bizRule.GetList(x => x.Active).ToList();
            var rulesVM = AutoMapper.Mapper.Map<List<Models.RoleRuleVM>>(rules);
            model.Rules = rulesVM.OrderBy(x=> x.DisplayName).ToList();

            var bizUser = new UserBusiness();
            var users = bizUser.GetList(x => x.Active).ToList();
            var usersVM = AutoMapper.Mapper.Map<List<Models.RoleUserVM>>(users);
            model.Users = usersVM.OrderBy(x => x.LastName + x.FirstName).ToList();
        }
        protected override RoleVM EntityToModel(Role entity)
        {
            Models.RoleVM model = base.EntityToModel(entity);

            //rules
            var bizRule = new RuleBusiness();
            var rules = bizRule.GetList(x => x.Active).ToList();
            var rulesVM = AutoMapper.Mapper.Map<List<Models.RoleRuleVM>>(rules).OrderBy(x => x.DisplayName).ToList();
            foreach (var rule in rulesVM)
            {
                if (entity.Rule.FirstOrDefault(r => r.RuleId == rule.RuleId) != null)
                    rule.Selected = true;
            }
            model.Rules = rulesVM;

            //users
            var bizUser = new UserBusiness();
            var users = bizUser.GetList(x => x.Active).ToList();
            var usersVM = AutoMapper.Mapper.Map<List<Models.RoleUserVM>>(users).OrderBy(x => x.LastName + x.FirstName).ToList();
            foreach (var user in usersVM)
            {
                if (entity.User.FirstOrDefault(r => r.UserId == user.UserId) != null)
                    user.Selected = true;
            }
            model.Users = usersVM;

            return model;
        }

        protected override Role ModelToEntity(RoleVM model)
        {
            Entities.Role entity = base.ModelToEntity(model);

            //rules
            var rules = model.Rules.Where(x => x.Selected).ToList();
            entity.Rule = AutoMapper.Mapper.Map<List<Entities.Rule>>(rules);

            //users
            var users = model.Users.Where(x => x.Selected).ToList();
            entity.User = AutoMapper.Mapper.Map<List<Entities.User>>(users);
            return entity;
        }
        

        protected override Role ReadForEditOrDetail(long id)
        {
            //me traigo las reglas seleccionadas
            return this.Business.Get(x => x.RoleId == id, new String[] { "Rule", "User" });            
        }
    }
}