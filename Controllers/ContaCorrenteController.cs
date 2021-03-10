using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaCorrenteController : ControllerBase
    {
        private IGenericRepository<ContaCorrente> genericRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IContaCorrenteRepository<ContaCorrente> contaCorrenteRepository;
        public ContaCorrenteController(IGenericRepository<ContaCorrente> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
        IContaCorrenteRepository<ContaCorrente> contaCorrenteRepository)
        {
            this.genericRepository = genericRepository;
            this.contaCorrenteRepository = contaCorrenteRepository;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;

        }
        [HttpPost()]
        [Route("filter")]
        [Authorize()]
        public IActionResult GetByFilter(FilterDefault filter)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                Expression<Func<ContaCorrente, bool>> p1, p2;
                var predicate = PredicateBuilder.New<ContaCorrente>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (filter.Name != null)
                {
                    p2 = p => p.Banco.Contains(filter.Name);
                    predicate = predicate.And(p2);
                }
                return new JsonResult(genericRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento das contas: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(ContaCorrente contaCorrente)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                if (contaCorrente.Id > decimal.Zero)
                {
                    var contaBase = genericRepository.Get(contaCorrente.Id);
                    contaBase.Agencia = contaCorrente.Agencia;
                    contaBase.Banco = contaCorrente.Banco;
                    contaBase.Conta = contaCorrente.Conta;
                    contaBase.BancoNumero = contaCorrente.BancoNumero;
                    contaBase.SaldoInicial = contaCorrente.SaldoInicial;
                    contaBase.UpdateApplicationUserId = id;
                    contaBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(contaBase);
                }
                else
                {
                    if (genericRepository.Where(x => x.Agencia == contaCorrente.Agencia 
                    && x.Banco == contaCorrente.Banco
                    && x.BancoNumero == contaCorrente.BancoNumero
                    && x.Conta == contaCorrente.Conta
                    && x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Conta corrente já cadastrada.");
                    }
                    contaCorrente.ApplicationUserId = id;
                    contaCorrente.CreateDate = DateTime.Now;
                    contaCorrente.EmpresaId = empresaId;
                    genericRepository.Insert(contaCorrente);
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no save do arquivo - " + ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize()]
        public IActionResult Get(int id)
        {
            try
            {
                return new JsonResult(contaCorrenteRepository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest("Conta não encontrada!" + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize()]
        public IActionResult Delete(int id)
        {
            try
            {
                var entityBase = genericRepository.Get(id);
                genericRepository.Delete(entityBase);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
