using IronSchool.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Repositories.Interfaces
{
    public partial interface IRuleRepository
    {
        List<Rule> GetRulesByUserId(User user);
    }
}
