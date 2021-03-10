

using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class CategoriaContasAPagarPlanoContasRepository<T> : ICategoriaContasAPagarPlanoContasRepository<CategoriaContasAPagarPlanoContas> where T : BaseEntity
    {
        private DbSet<CategoriaContasAPagarPlanoContas> entities;
        private DbSet<PlanoContas> planoContas;

        public CategoriaContasAPagarPlanoContasRepository(ApplicationDbContext context)
        {
            entities = context.Set<CategoriaContasAPagarPlanoContas>();
            planoContas = context.Set<PlanoContas>();
        }
        public IQueryable<CategoriaContasAPagarPlanoContas> Where(Expression<Func<CategoriaContasAPagarPlanoContas, bool>> expression)
        {
            return entities.Select(x => new CategoriaContasAPagarPlanoContas
            {
                Id = x.Id,
                 CategoriaContasAPagarId = x.CategoriaContasAPagarId,
                  PlanoContasId = x.PlanoContasId,
                   PlanoContas = planoContas.First(p => p.Id == x.PlanoContasId)
            }).Where(expression).AsQueryable();
        }
    }
}
