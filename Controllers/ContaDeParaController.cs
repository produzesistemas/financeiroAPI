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
    public class ContaDeParaController : ControllerBase
    {
        private IGenericRepository<ContaDePara> genericRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IContaDeParaRepository<ContaDePara> contaDeParaRepository;
        public ContaDeParaController(IGenericRepository<ContaDePara> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
        IContaDeParaRepository<ContaDePara> contaDeParaRepository)
        {
            this.genericRepository = genericRepository;
            this.contaDeParaRepository = contaDeParaRepository;
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
                Expression<Func<ContaDePara, bool>> p1, p2;
                var predicate = PredicateBuilder.New<ContaDePara>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (!filter.Name.Equals(""))
                {
                    p2 = p => p.De.Contains(filter.Name);
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
        public IActionResult Save(ContaDePara contaDePara)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                if (contaDePara.Id > decimal.Zero)
                {
                    var contaDeParaBase = genericRepository.Get(contaDePara.Id);
                    contaDeParaBase.De = contaDePara.De;
                    contaDeParaBase.Para = contaDePara.Para;
                    contaDeParaBase.UpdateApplicationUserId = id;
                    contaDeParaBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(contaDeParaBase);
                }
                else
                {
                    
                    contaDePara.ApplicationUserId = id;
                    contaDePara.CreateDate = DateTime.Now;
                    contaDePara.EmpresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                    genericRepository.Insert(contaDePara);
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
                return new JsonResult(contaDeParaRepository.Get(id));
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

