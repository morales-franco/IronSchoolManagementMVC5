using IronSchool.Repositories.Interfaces;
using IronSchool.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Business.Base
{
    public partial class ReportBusinessBase<R> : IReportBusinessBase
        where R : IReportRepository
    {
        protected R Repository { get; set; }
        protected string StoredProcedureName { get; set; }

        public ReportBusinessBase(string storedProcedureName)
        {
            Repository = RepositoryFactory.Resolve<R>();
            this.StoredProcedureName = storedProcedureName;
        }

        public virtual DataTable GetTable(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            return this.Repository.GetTable(storedProcedure, parameters);
        }

        public virtual IList<T> GetList<T>(string storedProcedure, params KeyValuePair<string, object>[] parameters) where T : class, new()
        {
            return this.Repository.GetList<T>(storedProcedure, parameters);
        }

        public virtual IList<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : class, new()
        {
            return this.Repository.GetList<T>(StoredProcedureName, parameters);
        }
        
        public virtual void FillTable(string storedProcedure, DataTable table, params KeyValuePair<string, object>[] parameters)
        {
            this.Repository.FillTable(storedProcedure, table, parameters);
        }
    }
}
