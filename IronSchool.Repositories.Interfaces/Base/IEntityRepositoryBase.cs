using IronSchool.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IronSchool.Repositories.Interfaces
{
    public interface IEntityRepository<E> : IReportRepository, IDisposable
           where E : class, new()
    {
        E Single(Expression<Func<E, bool>> condition, string[] includeProperties = null);

        E Get(Expression<Func<E, bool>> condition, string[] includeProperties = null);

        E Read(object id);

        int Count(Expression<Func<E, bool>> condition = null);

        bool Exist(Expression<Func<E, bool>> condition, string[] includeProperties = null);

        IList<E> GetList(string storedProcedure, params KeyValuePair<string, object>[] parameters);        

        IList<E> GetList(Expression<Func<E, bool>> filter = null, string[] includeProperties = null);

        void Insert(E entity);

        void Update(E entity);

        void UpdateSingleEntity(E entity);

        void Delete(E entity);

        void Delete(params object[] id);

        void DeleteMulti(Expression<Func<E, bool>> condition = null);

        void Deactivate(object id);
        
    }
}