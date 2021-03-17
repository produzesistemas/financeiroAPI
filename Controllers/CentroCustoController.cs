using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CentroCustoController : ControllerBase
    {
        private IGenericRepository<CentroCusto> genericRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private ICentroCustoRepository<CentroCusto> centrocustoRepository;
        public CentroCustoController(IGenericRepository<CentroCusto> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
        ICentroCustoRepository<CentroCusto> centrocustoRepository)
        {
            this.genericRepository = genericRepository;
            this.centrocustoRepository = centrocustoRepository;
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
                Expression<Func<CentroCusto, bool>> p1, p2;
                var predicate = PredicateBuilder.New<CentroCusto>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (filter.Name != null)
                {
                    p2 = p => p.Descricao.Contains(filter.Name);
                    predicate = predicate.And(p2);
                }
                return new JsonResult(genericRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento do centro de custo: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(CentroCusto entity)
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
                if (entity.Id > decimal.Zero)
                {
                    var entityBase = genericRepository.Get(entity.Id);
                    entityBase.Descricao = entity.Descricao;
                    entityBase.Codigo = entity.Codigo;
                    entityBase.UpdateApplicationUserId = id;
                    entityBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(entityBase);
                }
                else
                {
                    if (genericRepository.Where(x => x.Codigo == entity.Codigo && x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Centro de custo com esse código já cadastrado.");
                    }
                    entity.ApplicationUserId = id;
                    entity.CreateDate = DateTime.Now;
                    entity.Ativo = true;
                    entity.EmpresaId = empresaId;
                    genericRepository.Insert(entity);
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
                return new JsonResult(centrocustoRepository.Get(id));
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
