
using Model;

namespace UnitOfWork
{
    public interface ICategoriaContasAPagarRepository<T> where T : BaseEntity
    {
        T Get(int id);
    }
}
