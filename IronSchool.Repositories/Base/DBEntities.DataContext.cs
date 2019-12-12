using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Repositories
{
    public partial class DBEntities
    {
        
        public void InitializeContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Database.Connection.StateChange += new System.Data.StateChangeEventHandler(Connection_StateChange);
        }

        void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Open)
            {
                var conn = sender as SqlConnection;
                if (conn != null)
                {
                    SetAuditContext(conn);
                }
            }
        }

        protected void SetAuditContext(SqlConnection connection)
        {
            //string userID = GetUserName();

            //if (userID.Length <= 0)
            //{
            //    userID = "AnnonymusUser";
            //}

            //// Create local temporary context table 
            //var cmd = new SqlCommand();
            //IDbDataParameter param = cmd.CreateParameter();
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "spSetAuditUserName";

            //param.ParameterName = "UserName";
            //param.DbType = DbType.String;
            //param.Value = userID;
            //cmd.Parameters.Add(param);
            //cmd.Connection = connection;

            //cmd.ExecuteNonQuery();
        }

        private string GetUserName()
        {
            try
            {
                Entities.User user = IronSchool.Utils.Context.GetValue("CurrentUser") as Entities.User;
                if (user != null)
                {
                    return user.UserName;
                }
                return string.Empty;
            }
            catch (NullReferenceException)
            {
                //Si algun otro objeto llega a ser null, devuelve.
            }
            return string.Empty;
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}
