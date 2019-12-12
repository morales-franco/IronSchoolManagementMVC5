using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Repositories.Interfaces.Base
{
    public partial interface IReportRepository
    {
        DataTable GetTable(string storedProcedure, params KeyValuePair<string, object>[] parameters);

        void FillTable(string storedProcedure, DataTable table, KeyValuePair<string, object>[] parameters);

        IList<T> GetList<T>(string storedProcedure, params KeyValuePair<string, object>[] parameters) where T : class, new();
    }
}
