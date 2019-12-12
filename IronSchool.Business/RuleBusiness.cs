using IronSchool.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Business
{
    public partial class RuleBusiness
    {
        public List<Rule> GetRulesByUserId(User user)
        {
            return this.Repository.GetRulesByUserId(user);
        }

        public void DiscoverRule(string ruleDefinition)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["AutogenerateRules"]))
            {
                var count = this.Count(x => x.RuleDefinition == ruleDefinition);
                if(count == 0)
                {
                    var rule = new Rule()
                    {
                        Active = true,
                        DisplayName = ruleDefinition,
                        RuleDefinition = ruleDefinition
                    };
                    this.Insert(rule);
                }
            }
        }
    }
}
