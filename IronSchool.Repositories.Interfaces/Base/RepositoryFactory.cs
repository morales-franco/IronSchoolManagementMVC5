using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Configuration;

namespace IronSchool.Repositories.Interfaces
{
    public class RepositoryFactory
    {
        static UnityContainer container;
        
        private static void ConfigureContainer()
        {
            if(container == null)
            {
                container = new UnityContainer();
                UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Configure(container, "containerOne");
            }
        } 

        public static R Resolve<R>()
        {
            try
            {
                ConfigureContainer(); 
                return container.Resolve<R>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar resolver el repositorio para la interface: " + typeof(R).Name, ex);
            }
        }
    }
}
