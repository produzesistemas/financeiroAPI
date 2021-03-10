
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace UnitOfWork
{
    public interface IArquivoEntradaRepository<T> where T : BaseEntity
    {
        T Get(int id);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
    }
}
