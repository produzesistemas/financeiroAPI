
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace UnitOfWork
{
    public interface IEmpresaAspNetUsersRepository<T> where T : BaseEntity
    {
        void Insert(T entity);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
    }
}
