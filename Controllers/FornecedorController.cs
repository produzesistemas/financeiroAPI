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
    public class FornecedorController : ControllerBase
    {
        private IGenericRepository<Fornecedor> genericRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IFornecedorRepository<Fornecedor> fornecedorRepository;
        public FornecedorController(IGenericRepository<Fornecedor> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
        IFornecedorRepository<Fornecedor> fornecedorRepository)
        {
            this.genericRepository = genericRepository;
            this.fornecedorRepository = fornecedorRepository;
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
                Expression<Func<Fornecedor, bool>> p1, p2;
                var predicate = PredicateBuilder.New<Fornecedor>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                if (filter.Name != null)
                {
                    p2 = p => p.Nome.Contains(filter.Name);
                    predicate = predicate.And(p2);
                }
                return new JsonResult(genericRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento dos fornecedores: ", ex.Message));
            }
        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public IActionResult Save(Fornecedor fornecedor)
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
                if (fornecedor.Id > decimal.Zero)
                {
                    if (genericRepository.Where(x => x.Cnpj == fornecedor.Cnpj && x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Fornecedor já cadastrado.");
                    }
                    var entity = genericRepository.Get(fornecedor.Id);
                    entity.Nome = fornecedor.Nome;
                    entity.Telefone = fornecedor.Telefone;
                    entity.Cnpj = fornecedor.Cnpj;
                    entity.Contato = fornecedor.Contato;
                    entity.Email = fornecedor.Email;
                    entity.UpdateApplicationUserId = id;
                    entity.UpdateDate = DateTime.Now;
                    genericRepository.Update(entity);
                }
                else
                {
                    if (genericRepository.Where(x => x.Cnpj == fornecedor.Cnpj && x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Fornecedor já cadastrado.");
                    }
                    fornecedor.ApplicationUserId = id;
                    fornecedor.CreateDate = DateTime.Now;
                    fornecedor.EmpresaId = empresaId;
                    genericRepository.Insert(fornecedor);
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
                return new JsonResult(fornecedorRepository.Get(id));
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
