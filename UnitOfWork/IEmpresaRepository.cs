using Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace UnitOfWork
{
    public interface IEmpresaRepository<T> where T : BaseEntity
    {
        T Get(int id);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        IQueryable<T> GetAll();
    }
}
