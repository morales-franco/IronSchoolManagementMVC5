using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IronSchool.Utils
{
    public class ResourceManager
    {
        public static string GetResource(string resourceName)
        {
            return HttpContext.GetGlobalResourceObject("Resources", resourceName).ToString();
        }
    }
}
