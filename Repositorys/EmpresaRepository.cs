using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class EmpresaRepository<T> : IEmpresaRepository<Empresa> where T : BaseEntity
    {
        private DbSet<Empresa> entities;
        private DbSet<IdentityUser> users;
        private DbSet<EmpresaAspNetUsers> empresaAspNetUsers;

        
        public EmpresaRepository(ApplicationDbContext context)
        {
            entities = context.Set<Empresa>();
            users = context.Set<IdentityUser>();
            empresaAspNetUsers = context.Set<EmpresaAspNetUsers>();
        }

        public Empresa Get(int id)
        {
            return entities.Select(x => new Empresa
            {
                Id = x.Id,
                Nome = x.Nome,
                Ativo = x.Ativo,
                Cnpj = x.Cnpj,
                NomeUsuarioDominio = x.NomeUsuarioDominio,
                NomeImagem = x.NomeImagem,
                CodigoFilial = x.CodigoFilial,
                ContaTransitoria = x.ContaTransitoria,
                Email = users.FirstOrDefault(q => q.Id == empresaAspNetUsers.OrderBy(c => c.Id).FirstOrDefault(a => a.EmpresaId == x.Id).ApplicationUserId).Email
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Empresa> GetAll()
        {
            return entities.Select(x => new Empresa
            {
                Id = x.Id,
                Nome = x.Nome,
                Ativo = x.Ativo,
                Cnpj = x.Cnpj,
                NomeUsuarioDominio = x.NomeUsuarioDominio,
                NomeImagem = x.NomeImagem,
                CodigoFilial = x.CodigoFilial,
                ContaTransitoria = x.ContaTransitoria,
                Email = users.FirstOrDefault(q => q.Id == empresaAspNetUsers.OrderBy(c => c.Id).FirstOrDefault(a => a.EmpresaId == x.Id).ApplicationUserId).Email
            });
        }

        public IQueryable<Empresa> Where(Expression<Func<Empresa, bool>> expression)
        {
            return entities.Select(x => new Empresa
            {
                Id = x.Id,
                Nome = x.Nome,
                Ativo = x.Ativo,
                Cnpj = x.Cnpj,
                NomeUsuarioDominio = x.NomeUsuarioDominio,
                NomeImagem = x.NomeImagem,
                CodigoFilial = x.CodigoFilial,
                ContaTransitoria = x.ContaTransitoria,
                Email = users.FirstOrDefault(q => q.Id == empresaAspNetUsers.OrderBy(c => c.Id).FirstOrDefault(a => a.EmpresaId == x.Id).ApplicationUserId).Email
            }).Where(expression).AsQueryable();
        }
    }
}
