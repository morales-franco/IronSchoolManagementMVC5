using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Entities;
using System.Linq.Expressions;

namespace IronSchool.Repositories
{
    public partial class RoleRepository
    {
        public override void Insert(Role entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                List<long> ruleIds = entity.Rule.Select(y => y.RuleId).ToList();
                entity.Rule = connection.Rule.Where(x => ruleIds.Contains(x.RuleId)).ToList();

                List<long> userIds = entity.User.Select(y => y.UserId).ToList();
                entity.User = connection.User.Where(x => userIds.Contains(x.UserId)).ToList();

                connection.Role.Add(entity);
                connection.SaveChanges();
            }
        }

        public override void Update(Role entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                var dbEnt = connection.Role
                        .Include("Rule")
                        .Include("User")
                        .FirstOrDefault(x => x.RoleId == entity.RoleId);

                base.AssociateRelatedCollection<Rule, long>(entity, connection, dbEnt, x => x.Rule, y => y.RuleId);
                base.AssociateRelatedCollection<User, long>(entity, connection, dbEnt, x => x.User, y => y.UserId);
                connection.Entry(dbEnt).CurrentValues.SetValues(entity);
                connection.SaveChanges();
            }
        }
    }
}
