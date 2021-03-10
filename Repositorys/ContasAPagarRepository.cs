using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class ContasAPagarRepository<T> : IContasAPagarRepository<ContasAPagar> where T : BaseEntity
    {
        private DbSet<ContasAPagar> entities;
        private DbSet<Fornecedor> fornecedores;
        private DbSet<IdentityUser> users;
        private DbSet<SituacaoConta> situacoes;
        private DbSet<ContaCorrente> contascorrente;
        private DbSet<CentroCusto> centros;
        private DbSet<PlanoContas> planos;
        private DbSet<CategoriaContasAPagar> categorias;
        public ContasAPagarRepository(ApplicationDbContext context)
        {
            entities = context.Set<ContasAPagar>();
            fornecedores = context.Set<Fornecedor>();
            users = context.Set<IdentityUser>();
            situacoes = context.Set<SituacaoConta>();
            contascorrente = context.Set<ContaCorrente>();
            centros = context.Set<CentroCusto>();
            planos = context.Set<PlanoContas>();
            categorias = context.Set<CategoriaContasAPagar>();

        }

        public ContasAPagar Get(int id)
        {
            return entities.Select(x => new ContasAPagar
            {
                EmpresaId = x.EmpresaId,
                Referente = x.Referente,
                CentroCustoId = x.CentroCustoId,
                DataPagamento = x.DataPagamento,
                DataVencimento = x.DataVencimento,
                FornecedorId = x.FornecedorId,
                Juros = x.Juros,
                Id = x.Id,
                Multa = x.Multa,
                ContaCorrenteId = x.ContaCorrenteId,
                PlanoContasId = x.PlanoContasId,
                SituacaoContaId = x.SituacaoContaId,
                CategoriaContasAPagarId = x.CategoriaContasAPagarId,
                ValorOriginal = x.ValorOriginal,
                ValorPago = x.ValorPago,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName,
                Fornecedor = fornecedores.First(t => t.Id == x.FornecedorId),
                SituacaoConta = situacoes.First(s => s.Id == x.SituacaoContaId),
                ContaCorrente = contascorrente.First(c => c.Id == x.ContaCorrenteId),
                CentroCusto = centros.First(c => c.Id == x.CentroCustoId),
                PlanoContas = planos.First(p => p.Id == x.PlanoContasId),
                CategoriaContasAPagar = categorias.First(p => p.Id == x.CategoriaContasAPagarId)
            }).FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ContasAPagar> Where(Expression<Func<ContasAPagar, bool>> expression)
        {
            return entities.Select(x => new ContasAPagar
            {
                EmpresaId = x.EmpresaId,
                Referente = x.Referente,
                CentroCustoId = x.CentroCustoId,
                DataPagamento = x.DataPagamento,
                DataVencimento = x.DataVencimento,
                FornecedorId = x.FornecedorId,
                Juros = x.Juros,
                Id = x.Id,
                Multa = x.Multa,
                PlanoContasId = x.PlanoContasId,
                ContaCorrenteId = x.ContaCorrenteId,
                CategoriaContasAPagarId = x.CategoriaContasAPagarId,
                SituacaoContaId = x.SituacaoContaId,
                ValorOriginal = x.ValorOriginal,
                ValorPago = x.ValorPago,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                UpdateApplicationUserId = x.UpdateApplicationUserId,
                ApplicationUserId = x.ApplicationUserId,
                CriadoPor = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName,
                AlteradoPor = users.FirstOrDefault(q => q.Id == x.UpdateApplicationUserId).UserName,
                Fornecedor = fornecedores.First(t => t.Id == x.FornecedorId),
                SituacaoConta = situacoes.First(s => s.Id == x.SituacaoContaId),
                ContaCorrente = contascorrente.First(c => c.Id == x.ContaCorrenteId),
                CentroCusto = centros.First(c => c.Id == x.CentroCustoId),
                CategoriaContasAPagar = categorias.First(p => p.Id == x.CategoriaContasAPagarId),
                PlanoContas = planos.First(p => p.Id == x.PlanoContasId)
            }).Where(expression).AsQueryable();
        }
    }
}
