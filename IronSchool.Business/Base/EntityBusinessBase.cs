using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronSchool.Entities;
using IronSchool.Repositories.Interfaces;
using System.Reflection;
using System.Web;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data;
using IronSchool.Entities.Base;

namespace IronSchool.Business.Base
{
    public abstract class EntityBusinessBase<E, R> : IEntityBusinessBase<E>
        where R : IEntityRepository<E>
        where E : class, new()
    {
        protected R Repository { get; set; }

        public EntityBusinessBase()
        {
            Repository = RepositoryFactory.Resolve<R>();
        }

        public virtual string GetIndexStoredProcedureName()
        {
            return "Index" + typeof(E).Name;
        }

        public virtual E Single(Expression<Func<E, bool>> condition, string[] includeProperties = null)
        {
            return Repository.Single(condition, includeProperties);
        }

        public virtual E Get(Expression<Func<E, bool>> condition, string[] includeProperties = null)
        {
            return Repository.Get(condition, includeProperties);
        }

        public virtual E Read(object id)
        {
            return Repository.Read(id);
        }

        public virtual int Count(Expression<Func<E, bool>> condition = null)
        {
            return Repository.Count(condition);
        }

        public virtual bool Exist(Expression<Func<E, bool>> condition = null, string[] includeProperties = null)
        {
            return Repository.Exist(condition, includeProperties);
        }

        public virtual DataTable GetTable(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            return this.Repository.GetTable(storedProcedure, parameters);
        }

        public virtual void FillTable(string storedProcedure, DataTable table, params KeyValuePair<string, object>[] parameters)
        {
            this.Repository.FillTable(storedProcedure, table, parameters);
        }

        public virtual IList<E> GetList(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            return this.Repository.GetList(GetIndexStoredProcedureName(), parameters);
        }

        public virtual IList<T> GetList<T>(string storedProcedure, params KeyValuePair<string, object>[] parameters) where T : class, new()
        {
            if (typeof(T) == typeof(E))
                return this.Repository.GetList(storedProcedure, parameters) as IList<T>;
            else
                return this.Repository.GetList<T>(storedProcedure, parameters);
        }

        public virtual IList<T> GetList<T>(params KeyValuePair<string, object>[] parameters) where T : class, new()
        {
            if(typeof(T) == typeof(E))
                return this.Repository.GetList(GetIndexStoredProcedureName(), parameters) as IList<T>;
            else
                return this.Repository.GetList<T>(GetIndexStoredProcedureName(), parameters);
        }

        public virtual IList<E> GetList(Expression<Func<E, bool>> filter = null, string[] includeProperties = null)
        {
            return this.Repository.GetList(filter, includeProperties);
        }

        public virtual void Insert(E entity)
        {
            AuditChange(entity);            
            SetActive(entity);
            //SetCompany(entity);
            Repository.Insert(entity);
        }       

        protected virtual void AuditChange(E entity)
        {
            if (entity is IAuditableEntity)
            {
                (entity as IAuditableEntity).AuditDate = DateTime.Now;/*
                (entity as IAuditableEntity).AudTerminal = "HARDCODED";
                (entity as IAuditableEntity).AudUser = "HARDCODED";*/
            }
        }

        //public void SetCompany(E entity)
        //{
        //    if (entity is ICompanyEntity)
        //    {
        //        (entity as ICompanyEntity).EmpresaId = UserBusiness.Current.EmpresaId;
        //    }
        //}

        protected virtual void SetActive(E entity)
        { 
            if (entity is IActivableEntity)
            {
                (entity as IActivableEntity).Active = true;
            }
        }

        public virtual void Update(E entity)
        {
            AuditChange(entity);
            //SetCompany(entity);
            Repository.Update(entity);
        }

        public virtual void Delete(E entity)
        {
            if (entity is IActivableEntity)
            {
                ((IActivableEntity)entity).Active = false;
                AuditChange(entity);
                Repository.Update(entity);
            }
            else
                Repository.Delete(entity);
        }

        public virtual void Delete(params object[] id)
        {
            Type[] interfaces = typeof(E).GetInterfaces();
            if (interfaces.Contains(typeof(IActivableEntity)))
            {
                foreach(object objectID in id) {
                    Repository.Deactivate(objectID);
                }
            }
            else
                Repository.Delete(id);
        }

        public virtual void DeleteMulti(Expression<Func<E, bool>> condition = null)
        {
            Repository.DeleteMulti(condition);
        }

        public void Dispose()
        {
            Repository.Dispose();
            Repository = default(R);
        }
    }
}
