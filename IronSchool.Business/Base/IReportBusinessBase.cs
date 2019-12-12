using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Business.Base
{
    public partial interface IReportBusinessBase
    {
        DataTable GetTable(string storedProcedure, params KeyValuePair<string, object>[] parameters);

        void FillTable(string storedProcedure, DataTable table, params KeyValuePair<string, object>[] parameters);

        IList<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : class, new();

        IList<T> GetList<T>(string storedProcedure, params KeyValuePair<string, object>[] parameters) where T : class, new();
    }
}
