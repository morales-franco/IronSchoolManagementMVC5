using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronSchool.Repositories.Interfaces.Base;

namespace IronSchool.Repositories.Base
{
    public partial class ReportRepositoryBase: IReportRepository        
    {
        public virtual DataTable GetTable(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            DataTable table = new DataTable();

            FillTable(storedProcedure, table, parameters);

            return table;
        }

        public virtual void FillTable(string storedProcedure, DataTable table, params KeyValuePair<string, object>[] parameters)
        {
            using (DBEntities connection = new DBEntities())
            {
                using (var sqlConnection = new SqlConnection(connection.Database.Connection.ConnectionString))
                {
                    SqlCommand command = sqlConnection.CreateCommand();
                    command.CommandText = storedProcedure;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(CreateSqlParameters(parameters));
                    }

                    sqlConnection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(table);
                }
            }
        }


        public virtual IList<T> GetList<T>(string storedProcedure, params KeyValuePair<string, object>[] parameters) where T : class, new()
        {
            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; idx++)
                {
                    storedProcedure += String.Format(" @{0}=@{0}", parameters[idx].Key);
                    if (idx < (parameters.Length - 1))
                    {
                        storedProcedure += ",";
                    }
                }

                IList<T> list = new List<T>();

                using (DBEntities connection = new DBEntities())
                {
                    list = connection.Database.SqlQuery<T>(storedProcedure, CreateSqlParameters(parameters)).ToList<T>();
                }

                return list;
            }
            else
            {
                IList<T> list = new List<T>();

                using (DBEntities connection = new DBEntities())
                {
                    list = connection.Database.SqlQuery<T>(storedProcedure).ToList<T>();
                }

                return list;
            }
        }
        

        protected virtual SqlParameter[] CreateSqlParameters(KeyValuePair<string, object>[] parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; idx++)
                {
                    SqlParameter sqlParameter = new SqlParameter("@" + parameters[idx].Key, DBNull.Value);

                    if (parameters[idx].Value != null)
                    {
                        if (parameters[idx].Value.ToString() == "true" || parameters[idx].Value.ToString() == "false")
                            sqlParameter.Value = (Convert.ToBoolean(parameters[idx].Value) ? 1 : 0);
                        else
                            sqlParameter.Value = parameters[idx].Value;
                    }

                    sqlParameters.Add(sqlParameter);
                }
            }
            return sqlParameters.ToArray();
        }

        public virtual void Dispose()
        {
        }
    }
}
