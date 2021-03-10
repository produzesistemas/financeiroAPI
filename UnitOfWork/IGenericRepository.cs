using System;
using System.Linq;
using System.Linq.Expressions;
using Model;
namespace UnitOfWork
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        void Insert(T entity);
        IQueryable<T> GetAll();
        T Get(int id);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
    }
}
