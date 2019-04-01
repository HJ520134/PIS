using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        void AddList(List<T> entity);
        void AddOrUpdate(T insertEntity, T updateEntity, bool flag);
        void AddTrans(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteList(List<T> entity);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(int id);
        T GetById(string id);
        T GetFirstOrDefault(Expression<Func<T, bool>> where);
        IQueryable<T> GetAll();
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
    }
}
