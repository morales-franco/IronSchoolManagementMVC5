using IronSchool.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronSchool.Entities.Sex;

namespace IronSchool.Business
{
    public class SexBusiness
    {
        public IList<Sex> GetAll()
        {
            var entities = new List<Sex>()
            {
                new Sex()
                {
                    SexId = "M",
                    Description = "Masculino"
                },
                new Sex()
                {
                    SexId = "F",
                    Description = "Femenino"
                },
            };

            return entities;
        }
    }
}
