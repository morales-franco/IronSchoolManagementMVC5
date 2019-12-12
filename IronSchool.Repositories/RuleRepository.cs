using IronSchool.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Repositories
{
    public partial class RuleRepository : Interfaces.IRuleRepository
    {
        public List<Rule> GetRulesByUserId(User user)
        {
            using (DBEntities connection = new DBEntities())
            {
                return connection.User.Where(x => x.UserId == user.UserId)
                    .SelectMany(x => x.Role.SelectMany(y => y.Rule))
                    .Distinct().ToList();
            }
        }
    }
}
