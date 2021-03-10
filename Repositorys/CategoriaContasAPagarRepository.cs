using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Linq;
using UnitOfWork;

namespace Repositorys
{
    public class CategoriaContasAPagarRepository<T> : ICategoriaContasAPagarRepository<CategoriaContasAPagar> where T : BaseEntity
    {
        private DbSet<CategoriaContasAPagar> entities;
        private DbSet<CategoriaContasAPagarPlanoContas> contas;
        private DbSet<IdentityUser> users;
        private DbSet<PlanoContas> planoContas;

        public CategoriaContasAPagarRepository(ApplicationDbContext context)
        {
            entities = context.Set<CategoriaContasAPagar>();
            contas = context.Set<CategoriaContasAPagarPlanoContas>();
            users = context.Set<IdentityUser>();
            planoContas = context.Set<PlanoContas>();
        }
        public CategoriaContasAPagar Get(int id)
        {
            return entities.Select(x => new CategoriaContasAPagar
            {
                EmpresaId = x.EmpresaId,
                Id = x.Id,
                Nome = x.Nome,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName,
                contas = contas.Where(c => c.CategoriaContasAPagarId == x.Id)
                .Select(w => new CategoriaContasAPagarPlanoContas {
                CategoriaContasAPagarId = w.CategoriaContasAPagarId,
                Id = w.Id,
                PlanoContasId = w.PlanoContasId,
                PlanoContas = planoContas.FirstOrDefault(q => q.Id == w.PlanoContasId)
                }).ToList()
            }).FirstOrDefault(x => x.Id == id);
        }
    }
}
