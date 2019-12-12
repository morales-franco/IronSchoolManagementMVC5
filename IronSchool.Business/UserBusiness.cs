using IronSchool.Business.Base;
using IronSchool.Entities;
using IronSchool.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Business
{
    public partial class UserBusiness
    {
        public static User Current
        {
            get {
                return Utils.Context.GetValue("CurrentUser") as User;
            }
            set
            {
                Utils.Context.SetValue("CurrentUser", value);
            }
        }

        public bool IsAdministrator()
        {
            var userId = Current.UserId;
            RoleBusiness roleBuzz = new RoleBusiness();
            return roleBuzz.Exist(r => r.User.Any(u => u.UserId == userId) && r.RoleId == (long)RoleHelper.Role.Administrator, new string[] { "User" });
        }

        public static bool Authorize(string ruleDefinition)
        {
            var count = Current.Rules.Count(x => x.RuleDefinition == ruleDefinition);
            if (count > 0)
                return true;
            else
            {
                using (var biz = new RuleBusiness())
                {
                    biz.DiscoverRule(ruleDefinition);
                }               
                return false;
            }
        }

        public User Login(string userName, string password)
        {
            string passwordHash = Utils.Encryption.Encrypt(password);
            var user = this.Get(x => x.UserName == userName && x.PasswordHash == passwordHash);
            if (user == null)
                throw new ValidationException("InvalidUserNameOrPassword");
            if (!user.Active)
                throw new ValidationException("UserIsDisabled");

            var ruleBuzz = new RuleBusiness();

            UserBusiness.Current = user;
            UserBusiness.Current.Rules = ruleBuzz.GetRulesByUserId(UserBusiness.Current);

            return user;
        }


        public void Logout()
        {
            UserBusiness.Current = null;
        }

        public void ResetPassword(long userId, string password)
        {
            string passwordHash = Utils.Encryption.Encrypt(password);
            var user = this.Read(userId);
            user.PasswordHash = passwordHash;
            this.Update(user);
        }

        public void RefreshApplicationUser()
        {
            var user = this.Get(x => x.UserId == UserBusiness.Current.UserId);
            UserBusiness.Current = user;
            var biz = new RuleBusiness();
            UserBusiness.Current.Rules = biz.GetRulesByUserId(user);
        }


        public override void Insert(User entity)
        {
            ValidateCreateOrUpdate(entity);
            base.Insert(entity);
        }

        public override void Update(User entity)
        {
            ValidateCreateOrUpdate(entity);
            base.Update(entity);
        }

        public void ValidateCreateOrUpdate(User entity)
        {
            if (this.Repository.Count(x => x.UserName == entity.UserName && x.UserId != entity.UserId) > 0)
                throw new ValidationException("ValidationSameUserName");
        }
    }
}
