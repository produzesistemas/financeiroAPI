using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class FornecedorRepository<T> : IFornecedorRepository<Fornecedor> where T : BaseEntity
    {
        private DbSet<Fornecedor> entities;
        private DbSet<IdentityUser> users;

        public FornecedorRepository(ApplicationDbContext context)
        {
            entities = context.Set<Fornecedor>();
            users = context.Set<IdentityUser>();
        }

        public Fornecedor Get(int id)
        {
            return entities.Select(x => new Fornecedor
            {
                Id = x.Id,
                Nome = x.Nome,
                Ativo = x.Ativo,
                Cnpj = x.Cnpj,
                Contato = x.Contato,
                Email = x.Email,
                Telefone = x.Telefone,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Fornecedor> Where(Expression<Func<Fornecedor, bool>> expression)
        {
            return entities.Select(x => new Fornecedor
            {
                Id = x.Id,
                Nome = x.Nome,
                Ativo = x.Ativo,
                Cnpj = x.Cnpj,
                Contato = x.Contato,
                Email = x.Email,
                Telefone = x.Telefone,
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
