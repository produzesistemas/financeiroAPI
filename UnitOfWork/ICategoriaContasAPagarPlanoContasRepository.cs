using Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace UnitOfWork
{
    public interface ICategoriaContasAPagarPlanoContasRepository<T> where T : BaseEntity
    {
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
    }
}
