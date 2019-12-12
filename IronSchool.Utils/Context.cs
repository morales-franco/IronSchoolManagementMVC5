using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IronSchool.Utils
{
    public class Context
    {
        public static object GetValue(string key)
        {
            object value = HttpContext.Current.Session[key];

            /*if (value == null)
                throw new CurrentSessionException("No existe la clave '" + key + "' en la sesión actual.");*/

            return value;
        }

        public static void SetValue(string key, object value)
        {
            HttpContext.Current.Session.Add(key, value);
        }

        public static string ResolvePath(string path)
        {
            if (HttpContext.Current == null)
            {
                string resolvedPath = System.Web.Hosting.HostingEnvironment.MapPath(path);
                if (String.IsNullOrEmpty(resolvedPath))
                {
                    resolvedPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Context)).CodeBase);
                    resolvedPath = resolvedPath.Replace("file:\\", "");
                    resolvedPath += path.Replace("~", "").Replace("/", @"\");                    
                }
                return resolvedPath;
            }
            else
                return HttpContext.Current.Server.MapPath(path); 
        }
    }

    public class CurrentSessionException : Exception
    {
        public CurrentSessionException(string message) : base(message) { }
    }    
}
