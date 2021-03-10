using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanoContasController : ControllerBase
    {
        private IGenericRepository<PlanoContas> genericRepository;
        private IGenericRepository<TipoConta> tipoContaRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IPlanoContasRepository<PlanoContas> planocontasRepository;
        private static readonly Encoding LocalEncoding = Encoding.GetEncoding("iso-8859-1");
        public PlanoContasController(IGenericRepository<PlanoContas> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
        IPlanoContasRepository<PlanoContas> planocontasRepository,
        IGenericRepository<TipoConta> tipoContaRepository)
        {
            this.genericRepository = genericRepository;
            this.planocontasRepository = planocontasRepository;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;
            this.tipoContaRepository = tipoContaRepository;
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

                Expression<Func<PlanoContas, bool>> p1, p2;
                var predicate = PredicateBuilder.New<PlanoContas>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (filter.Name != null)
                {
                    p2 = p => p.Descricao.Contains(filter.Name);
                    predicate = predicate.And(p2);
                }
                return new JsonResult(planocontasRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento dos fornecedores: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(PlanoContas planoContas)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                if (planoContas.Id > decimal.Zero)
                {
                    var entity = genericRepository.Get(planoContas.Id);
                    entity.Descricao = planoContas.Descricao;
                    entity.Classificacao = planoContas.Classificacao;
                    entity.UpdateApplicationUserId = id;
                    entity.UpdateDate = DateTime.Now;
                    entity.TipoContaId = planoContas.TipoContaId;
                    genericRepository.Update(entity);
                }
                else
                {
                    planoContas.ApplicationUserId = id;
                    planoContas.CreateDate = DateTime.Now;
                    planoContas.Ativo = true;
                    planoContas.EmpresaId = empresaAspNetUsersRepository.Where(x => x.ApplicationUserId == id).FirstOrDefault().Id;
                    genericRepository.Insert(planoContas);
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
                return new JsonResult(planocontasRepository.Get(id));
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

        [HttpPost()]
        [Route("importarDominio")]
        [Authorize()]
        public async Task<IActionResult> ImportarDominio()
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
                var lst = new List<string>();
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Form.Files[0].CopyToAsync(memoryStream);
                    string[] lines = LocalEncoding.GetString(memoryStream.ToArray()).Split("\r\n");
                    foreach (string line in lines)
                    {
                        if (!line.Equals(""))
                        {
                            var classificacao = line.Substring(0, 27);
                            var descricao = line.Substring(27, 39);
                            var tipo = line.Substring(67, 1);
                            if ((tipo.Equals("S")) || (tipo.Equals("A")))
                            {
                                genericRepository.Insert(new PlanoContas()
                                {
                                    ApplicationUserId = id,
                                    Ativo = true,
                                    Classificacao = classificacao,
                                    CreateDate = DateTime.Now,
                                    Descricao = descricao,
                                    EmpresaId = empresaId,
                                    TipoContaId = tipoContaRepository.Where(x => x.Sigla == tipo).FirstOrDefault().Id
                                });
                            }
                        }
                    }
                }
                return new JsonResult(lst);
            }
            catch (Exception ex)
            {
                return BadRequest("Falha na conversão do arquivo - " + ex.Message);
            }

        }
    }
}
