
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class ContaCorrenteRepository<T> : IContaCorrenteRepository<ContaCorrente> where T : BaseEntity
    {
        private DbSet<ContaCorrente> entities;
        private DbSet<IdentityUser> users;

        public ContaCorrenteRepository(ApplicationDbContext context)
        {
            entities = context.Set<ContaCorrente>();
            users = context.Set<IdentityUser>();
        }

        public ContaCorrente Get(int id)
        {
            return entities.Select(x => new ContaCorrente
            {
                Id = x.Id,
                Agencia = x.Agencia,
                Banco = x.Banco,
                Conta = x.Conta,
                BancoNumero = x.BancoNumero,
                SaldoInicial = x.SaldoInicial,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ContaCorrente> Where(Expression<Func<ContaCorrente, bool>> expression)
        {
            return entities.Select(x => new ContaCorrente
            {
                Id = x.Id,
                Agencia = x.Agencia,
                Banco = x.Banco,
                Conta = x.Conta,
                BancoNumero = x.BancoNumero,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                SaldoInicial = x.SaldoInicial,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).Where(expression).AsQueryable();
        }
    }
}
