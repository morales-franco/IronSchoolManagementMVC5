using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Entities.Base
{
    public interface IActivableEntity
    {
        bool Active { get; set; }
    }
}
