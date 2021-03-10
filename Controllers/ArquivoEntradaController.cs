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
    public class ArquivoEntradaController : ControllerBase
    {
        private IGenericRepository<ArquivoEntrada> genericRepository;
        private IArquivoEntradaRepository<ArquivoEntrada> arquivoEntradaRepository;
        private IGenericRepository<Empresa> empresaRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        public ArquivoEntradaController(IGenericRepository<ArquivoEntrada> genericRepository,
            IArquivoEntradaRepository<ArquivoEntrada> arquivoEntradaRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
            IGenericRepository<Empresa> empresaRepository)
        {
            this.genericRepository = genericRepository;
            this.arquivoEntradaRepository = arquivoEntradaRepository;
            this.empresaRepository = empresaRepository;
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

                Expression<Func<ArquivoEntrada, bool>> p1, p2;
                var predicate = PredicateBuilder.New<ArquivoEntrada>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (filter.Name != null)
                {
                    p2 = p => p.Descricao.Contains(filter.Name);
                    predicate = predicate.And(p2);
                }
                return new JsonResult(arquivoEntradaRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento dos arquivos: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(ArquivoEntrada arquivoEntrada)
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

                if (arquivoEntrada.Id > decimal.Zero)
                {
                    var arquivoEntradaBase = genericRepository.Get(arquivoEntrada.Id);
                    arquivoEntradaBase.Descricao = arquivoEntrada.Descricao;
                    arquivoEntradaBase.ColunaContaCredito = arquivoEntrada.ColunaContaCredito;
                    arquivoEntradaBase.ColunaContaDebito = arquivoEntrada.ColunaContaDebito;
                    arquivoEntradaBase.ColunaData = arquivoEntrada.ColunaData;
                    arquivoEntradaBase.ColunaHistorico = arquivoEntrada.ColunaHistorico;
                    arquivoEntradaBase.ColunaNLancamento = arquivoEntrada.ColunaNLancamento;
                    arquivoEntradaBase.ColunaValorCredito = arquivoEntrada.ColunaValorCredito;
                    arquivoEntradaBase.ColunaValorDebito = arquivoEntrada.ColunaValorDebito;
                    arquivoEntradaBase.ContaTransitoria = arquivoEntrada.ContaTransitoria;
                    arquivoEntradaBase.IsCredito = arquivoEntrada.IsCredito;
                    arquivoEntradaBase.IsDebito = arquivoEntrada.IsDebito;
                    arquivoEntradaBase.UpdateApplicationUserId = id;
                    arquivoEntradaBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(arquivoEntradaBase);
                }
                else
                {
                    arquivoEntrada.EmpresaId = empresaId;
                    arquivoEntrada.CreateDate = DateTime.Now;
                    arquivoEntrada.ApplicationUserId = id;
                    genericRepository.Insert(arquivoEntrada);

                }

                return new OkResult();

            }
            catch (Exception ex)
            {
                return BadRequest("Falha no save do arquivo - " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize()]
        public IActionResult Get(int id)
        {
            try
            {
                return new JsonResult(arquivoEntradaRepository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest("Arquivo não encontrado!" + ex.Message);
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
