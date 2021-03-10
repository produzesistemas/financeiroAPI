using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class PlanoContasRepository<T> : IPlanoContasRepository<PlanoContas> where T : BaseEntity
    {
        private DbSet<PlanoContas> entities;
        private DbSet<TipoConta> tipos;
        private DbSet<IdentityUser> users;

        public PlanoContasRepository(ApplicationDbContext context)
        {
            entities = context.Set<PlanoContas>();
            tipos = context.Set<TipoConta>();
            users = context.Set<IdentityUser>();
        }

        public PlanoContas Get(int id)
        {
            return entities.Select(x => new PlanoContas
            {
                EmpresaId = x.EmpresaId,
                Id = x.Id,
                Descricao = x.Descricao,
                Ativo = x.Ativo,
                Classificacao = x.Classificacao,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName,
                TipoContaId = x.TipoContaId,
                TipoConta = tipos.First(t => t.Id == x.TipoContaId)
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<PlanoContas> Where(Expression<Func<PlanoContas, bool>> expression)
        {
            return entities.Select(x => new PlanoContas
            {
                 EmpresaId = x.EmpresaId,
                Id = x.Id,
                Descricao = x.Descricao,
                Ativo = x.Ativo,
                Classificacao = x.Classificacao,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.First(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.First(q => q.Id == x.UpdateApplicationUserId).UserName,
                TipoContaId = x.TipoContaId,
                TipoConta = tipos.First(t => t.Id == x.TipoContaId)
            }).Where(expression).AsQueryable();
       }
    }
}
