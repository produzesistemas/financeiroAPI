using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> entities;

        public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
            entities = context.Set<T>();
        }

        public void Delete(T entity)
        {
            entities.Remove(entity);
            _context.SaveChanges();
        }

        public T Get(int id)
        {
            return entities.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<T> GetAll()
        {
            return entities;
        }

        public void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return entities.Where(expression).AsQueryable();
        }

    }
}
