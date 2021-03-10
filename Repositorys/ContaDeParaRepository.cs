using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class ContaDeParaRepository<T> : IContaDeParaRepository<ContaDePara> where T : BaseEntity
    {
        private DbSet<ContaDePara> entities;
        private DbSet<IdentityUser> users;

        public ContaDeParaRepository(ApplicationDbContext context)
        {
            entities = context.Set<ContaDePara>();
            users = context.Set<IdentityUser>();
        }

        public ContaDePara Get(int id)
        {
            return entities.Select(x => new ContaDePara
            {
                Id = x.Id,
                De = x.De,
                Para = x.Para,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ContaDePara> Where(Expression<Func<ContaDePara, bool>> expression)
        {
            return entities.Select(x => new ContaDePara
            {
                Id = x.Id,
                De = x.De,
                Para = x.Para,
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
