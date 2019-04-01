using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Infrastructure
{
    public abstract class RepositoryBase<T> where T : class
    {

        private SPPContext dataContext;
        private readonly IDbSet<T> dbset;
        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = DataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected SPPContext DataContext
        {
            get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
        }
        public virtual T Add(T entity)
        {
            return dbset.Add(entity);
        }
        public virtual void AddList(List<T> entity)
        {
            foreach (var item in entity)
            {
                dbset.Add(item);
            }           
        }


        public virtual void AddTrans(T entity)
        {
            using (var trans = dataContext.Database.BeginTransaction())
            {
                try
                {
                    dbset.Add(entity);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public virtual void Update(T entity)
        {
            dbset.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            dbset.Remove(entity);
        }
        public virtual void DeleteList(List<T> entity)
        {
            foreach (var item in entity)
            {
                dbset.Remove(item);
            }
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbset.Remove(obj);
        }

        public virtual void AddOrUpdate(T insertEntity, T updateEntity, bool flag)
        {
            if (flag)
            {
                dbset.Add(insertEntity);
            }
            else
            {
                dbset.Attach(updateEntity);
                dataContext.Entry(updateEntity).State = EntityState.Modified;
            }
        }
        public virtual T GetById(int id)
        {
            return dbset.Find(id);
        }
        public virtual T GetById(string id)
        {
            return dbset.Find(id);
        }
        public virtual IQueryable<T> GetAll()
        {
            return dbset;
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).FirstOrDefault<T>();
        }
    }
}
