using IronSchool.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using IronSchool.Entities;
using IronSchool.Entities.Base;
using IronSchool.Repositories.Base;

namespace IronSchool.Repositories
{
    public class EntityRepositoryBase<E> : ReportRepositoryBase, IEntityRepository<E>
        where E : class, new()
    {
        public virtual E Single(Expression<Func<E, bool>> condition, string[] includeProperties = null)
        {
            E entity = null;

            using (DBEntities connection = new DBEntities())
            {
                IQueryable<E> query = connection.Set<E>();

                if (includeProperties != null)
                    foreach (string includeProperty in includeProperties)
                    {
                        query = query.Include(includeProperty);
                    }

                entity = query.SingleOrDefault(condition);
            }

            return entity;
        }

        public virtual E Get(Expression<Func<E, bool>> condition, string[] includeProperties = null)
        {
            E entity = null;

            using (DBEntities connection = new DBEntities())
            {
                IQueryable<E> query = connection.Set<E>();

                if (includeProperties != null)
                    foreach (string includeProperty in includeProperties)
                    {
                        query = query.Include(includeProperty);
                    }

                entity = query.FirstOrDefault(condition);
            }

            return entity;
        }

        public virtual E Read(object id)
        {
            E entity = null;

            using (DBEntities connection = new DBEntities())
            {
                entity = connection.Set<E>().Find(id);
            }

            return entity;
        }

        public virtual int Count(Expression<Func<E, bool>> condition = null)
        {
            int count = 0;

            using (DBEntities connection = new DBEntities())
            {
                if (condition == null)
                    count = connection.Set<E>().Count();
                else
                    count = connection.Set<E>().Count(condition);
            }

            return count;
        }

        public virtual IList<E> GetList(string storedProcedure, params KeyValuePair<string, object>[] parameters)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (parameters != null)
            {
                for (int idx = 0; idx < parameters.Length; idx++)
                {
                    storedProcedure += " @" + parameters[idx].Key;
                    if (idx < (parameters.Length - 1))
                    {
                        storedProcedure += ",";
                    }
                }

                IList<E> list = new List<E>();

                using (DBEntities connection = new DBEntities())
                {
                    list = connection.Database.SqlQuery<E>(storedProcedure, CreateSqlParameters(parameters)).ToList<E>();
                }

                return list;
            }
            else
            {
                IList<E> list = new List<E>();

                using (DBEntities connection = new DBEntities())
                {
                    list = connection.Database.SqlQuery<E>(storedProcedure).ToList<E>();
                }

                return list;
            }
        }

        public virtual IList<E> GetList(Expression<Func<E, bool>> filter = null, string[] includeProperties = null)
        {
            IList<E> list = new List<E>();

            using (DBEntities connection = new DBEntities())
            {
                connection.Configuration.LazyLoadingEnabled = false;
                connection.Configuration.ProxyCreationEnabled = false;

                IQueryable<E> query = connection.Set<E>();

                if (includeProperties != null)
                    foreach (string includeProperty in includeProperties)
                    {
                        query = query.Include(includeProperty);
                    }

                if (filter != null)
                    query = query.Where(filter);

                list = query.ToList();
            }

            return list;
        }

        public virtual void Insert(E entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                connection.Set<E>().Add(entity);
                connection.SaveChanges();
            }
        }

        public virtual void UpdateSingleEntity(E entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                   connection.Set<E>().Attach(entity);
                    connection.Entry<E>(entity).State = EntityState.Modified;
                    connection.SaveChanges();
               
            }
        }

        public virtual void Update(E entity)
        {
            UpdateSingleEntity(entity);
        }

        public virtual void Delete(E entity)
        {
            using (DBEntities connection = new DBEntities())
            {
                try
                {
                    connection.Set<E>().Attach(entity);
                    connection.Set<E>().Remove(entity);
                    connection.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is UpdateException && 
                        ex.InnerException.InnerException is SqlException && 
                        ((SqlException)ex.InnerException.InnerException).Number == 547)
                            throw new Exception("No se puede eliminar el registro por tener registros asociados.", ex);
                    else
                        throw ex;
                }                
            }
        }

        public virtual void Delete(params object[] id)
        {
            using (DBEntities connection = new DBEntities())
            {
                try
                {
                    E entity = connection.Set<E>().Find(id);
                    connection.Set<E>().Remove(entity);
                    connection.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is UpdateException &&
                        ex.InnerException.InnerException is SqlException &&
                        ((SqlException)ex.InnerException.InnerException).Number == 547)
                        throw new Exception("No se puede eliminar el registro por tener registros asociados.", ex);
                    else
                        throw ex;
                }                
            }
        }

        public virtual void DeleteMulti(Expression<Func<E, bool>> condition = null)
        {
            using (DBEntities connection = new DBEntities())
            {
                try
                {
                    connection.Set<E>().RemoveRange(connection.Set<E>().Where(condition));
                    connection.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is UpdateException &&
                        ex.InnerException.InnerException is SqlException &&
                        ((SqlException)ex.InnerException.InnerException).Number == 547)
                        throw new Exception("No se puede eliminar el registro por tener registros asociados.", ex);
                    else
                        throw ex;
                }                
            }
        }

        public virtual void Deactivate(object id)
        {
            using (DBEntities connection = new DBEntities())
            {
                E entity = connection.Set<E>().Find(id);
                ((IActivableEntity)entity).Active = false;
                connection.SaveChanges();
            }
        }

        public static void SetPropertyValue<T, R>(T target, Expression<Func<T, R>> memberLamda, R value)
        {
            var memberSelectorExpression = memberLamda.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }

        protected void HandleRelatedEntity<P, R>(P newEntity, P oldEntity, DBEntities connection, Expression<Func<P, R>> path, System.Linq.Expressions.Expression<Func<R, long>> comparison)
            where P : class, new()
            where R : class, new()
        {
            LambdaExpression lambda = path;
            var compiledLambda = lambda.Compile();
            LambdaExpression lambdaID = comparison;
            var compiledLambdaID = lambdaID.Compile();
            R newRelatedEntity = (R)compiledLambda.DynamicInvoke(newEntity);
            R dbRelatedEntity = (R)compiledLambda.DynamicInvoke(oldEntity);

            if (dbRelatedEntity != null)
            {
                if (newRelatedEntity != null)
                {
                    long newID = (long)compiledLambdaID.DynamicInvoke(newRelatedEntity);
                    long oldID = (long)compiledLambdaID.DynamicInvoke(dbRelatedEntity);
                    if (newID == oldID)
                        connection.Entry(dbRelatedEntity).CurrentValues.SetValues(newRelatedEntity);
                    else
                    {
                        // Relationship change
                        // Attach assumes that newFoo.SubFoo is an existing entity
                        connection.Set<R>().Attach(newRelatedEntity);
                        SetPropertyValue<P, R>(oldEntity, path, newRelatedEntity);
                    }
                }
                else
                    SetPropertyValue<P, R>(oldEntity, path, null);
            }
            else
            {
                if (newRelatedEntity != null) // relationship has been added                
                {
                    long newID = (long)compiledLambdaID.DynamicInvoke(newRelatedEntity);
                    if (newID > 0)
                    {
                        connection.Set<R>().Attach(newRelatedEntity);
                        connection.Entry<R>(newRelatedEntity).State = EntityState.Modified;
                        SetPropertyValue<P, R>(oldEntity, path, newRelatedEntity);
                    }
                    else
                    {
                        // Attach assumes that newFoo.SubFoo is an existing entity
                        connection.Set<R>().Add(newRelatedEntity);
                        SetPropertyValue<P, R>(oldEntity, path, newRelatedEntity);
                    }
                }
                // else -> old and new SubFoo is null -> nothing to do
            }
        }

        protected void UpdateRelatedCollection<P, R, TID>(P entity, DBEntities connection, P dbEnt, Expression<Func<P, ICollection<R>>> path, System.Linq.Expressions.Expression<Func<R, TID>> idPath)
            where P : class, new()
            where R : class, new()
        {
            UpdateRelatedCollection<P, R, TID>(entity, connection, dbEnt, path, idPath, false);
        }

        protected void UpdateRelatedCollection<P, R, TID>(P entity, DBEntities connection, P dbEnt, Expression<Func<P, ICollection<R>>> path, System.Linq.Expressions.Expression<Func<R, TID>> idPath, bool deleteChildren)
            where P : class, new()
            where R : class, new()
        {
            LambdaExpression lambda = path;
            var compiledLambda = lambda.Compile();
            LambdaExpression lambdaID = idPath;
            var compiledLambdaID = lambdaID.Compile();
            ICollection<R> newCollection = (ICollection<R>)compiledLambda.DynamicInvoke(entity);
            ICollection<R> dbCollection = (ICollection<R>)compiledLambda.DynamicInvoke(dbEnt);

            List<TID> newIDs = newCollection.Select<R, TID>(idPath.Compile()).ToList();
            List<TID> oldIDs = dbCollection.Select<R, TID>(idPath.Compile()).ToList();

            foreach (TID id in oldIDs)
            {
                if (!newIDs.Contains(id))
                {
                    R oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            oldEntity = e;
                            break;
                        }
                    }
                    dbCollection.Remove(oldEntity);
                    if (deleteChildren)
                        RemoveDeletedChild(connection, oldEntity);
                }
            }

            foreach (TID id in newIDs)
            {
                if (!oldIDs.Contains(id))
                {
                    R newEntity = connection.Set<R>().Find(id);
                    if (newEntity != null)
                        dbCollection.Add(newEntity);
                }
                else
                {
                    R oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            oldEntity = e;
                            break;
                        }
                    }
                    R newEntity = null;
                    foreach (var e in newCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                        {
                            newEntity = e;
                            break;
                        }
                    }
                    connection.Entry(oldEntity).CurrentValues.SetValues(newEntity);
                }
            }

            //add the new objects to the collection
            long idAdd = 0;
            foreach (var e in newCollection)
            {
                if (compiledLambdaID.DynamicInvoke(e).Equals(idAdd))
                    dbCollection.Add(e);
            }
        }

        protected virtual void RemoveDeletedChild<R>(DBEntities connection, R oldEntity) where R : class, new()
        {
            connection.Set<R>().Remove(oldEntity);
        }

        protected void AssociateRelatedCollection<R, TID>(E entity, DBEntities connection, E dbEnt, Expression<Func<E, ICollection<R>>> path, System.Linq.Expressions.Expression<Func<R, TID>> idPath)
            where R : class, new()
        {
            LambdaExpression lambda = path;
            var compiledLambda = lambda.Compile();
            LambdaExpression lambdaID = idPath;
            var compiledLambdaID = lambdaID.Compile();
            ICollection<R> newCollection = (ICollection<R>)compiledLambda.DynamicInvoke(entity);
            ICollection<R> dbCollection = (ICollection<R>)compiledLambda.DynamicInvoke(dbEnt);

            List<TID> newIDs = newCollection.Select<R, TID>(idPath.Compile()).ToList();
            List<TID> oldIDs = dbCollection.Select<R, TID>(idPath.Compile()).ToList();

            foreach (TID id in oldIDs)
            {
                if (!newIDs.Contains(id))
                {
                    R oldEntity = null;
                    foreach (var e in dbCollection)
                    {
                        if (compiledLambdaID.DynamicInvoke(e).Equals(id))
                            oldEntity = e;
                    }
                    dbCollection.Remove(oldEntity);
                }
            }

            foreach (TID id in newIDs)
            {
                if (!oldIDs.Contains(id))
                {
                    R newEntity = connection.Set<R>().Find(id);
                    if (newEntity != null)
                        dbCollection.Add(newEntity);
                }
            }
        }

        public virtual bool Exist(Expression<Func<E, bool>> condition, string[] includeProperties = null)
        {
            using (DBEntities connection = new DBEntities())
            {
                IQueryable<E> query = connection.Set<E>();

                if (includeProperties != null)
                    foreach (string includeProperty in includeProperties)
                    {
                        query = query.Include(includeProperty);
                    }

                return query.Any(condition);
            }

        }

    }
}
