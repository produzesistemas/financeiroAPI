
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class ArquivoEntradaRepository<T> : IArquivoEntradaRepository<ArquivoEntrada> where T : BaseEntity
    {
        private DbSet<ArquivoEntrada> entities;
        private DbSet<IdentityUser> users;

        public ArquivoEntradaRepository(ApplicationDbContext context)
        {
            entities = context.Set<ArquivoEntrada>();
            users = context.Set<IdentityUser>();
        }

        public ArquivoEntrada Get(int id)
        {
            return entities.Select(x => new ArquivoEntrada
            {
                EmpresaId = x.EmpresaId,
                Id = x.Id,
                ColunaContaCredito = x.ColunaContaCredito,
                ColunaContaDebito = x.ColunaContaDebito,
                ColunaData = x.ColunaData,
                ColunaHistorico = x.ColunaHistorico,
                ColunaNLancamento = x.ColunaNLancamento,
                ColunaValorCredito = x.ColunaValorCredito,
                ColunaValorDebito = x.ColunaValorDebito,
                 ContaTransitoria = x.ContaTransitoria,
                  Descricao = x.Descricao,
                   IsCredito = x.IsCredito,
                    IsDebito = x.IsDebito,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ArquivoEntrada> Where(Expression<Func<ArquivoEntrada, bool>> expression)
        {
            return entities.Select(x => new ArquivoEntrada
            {
                EmpresaId = x.EmpresaId,
                Id = x.Id,
                ColunaContaCredito = x.ColunaContaCredito,
                ColunaContaDebito = x.ColunaContaDebito,
                ColunaData = x.ColunaData,
                ColunaHistorico = x.ColunaHistorico,
                ColunaNLancamento = x.ColunaNLancamento,
                ColunaValorCredito = x.ColunaValorCredito,
                ColunaValorDebito = x.ColunaValorDebito,
                ContaTransitoria = x.ContaTransitoria,
                Descricao = x.Descricao,
                IsCredito = x.IsCredito,
                IsDebito = x.IsDebito,
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
