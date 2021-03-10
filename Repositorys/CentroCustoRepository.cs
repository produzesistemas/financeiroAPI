using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class CentroCustoRepository<T> : ICentroCustoRepository<CentroCusto> where T : BaseEntity
    {
        private DbSet<CentroCusto> entities;
        private DbSet<IdentityUser> users;

        public CentroCustoRepository(ApplicationDbContext context)
        {
            entities = context.Set<CentroCusto>();
            users = context.Set<IdentityUser>();
        }

        public CentroCusto Get(int id)
        {
            return entities.Select(x => new CentroCusto
            {
                Id = x.Id,
                Descricao = x.Descricao,
                Ativo = x.Ativo,
                Codigo = x.Codigo,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<CentroCusto> Where(Expression<Func<CentroCusto, bool>> expression)
        {
            return entities.Select(x => new CentroCusto
            {
                Id = x.Id,
                Descricao = x.Descricao,
                Ativo = x.Ativo,
                Codigo = x.Codigo,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).Where(expression).AsQueryable();
        }
    }
}
