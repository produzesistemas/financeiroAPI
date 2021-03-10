using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Filters;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaContasAPagarController : ControllerBase
    {
        private IGenericRepository<CategoriaContasAPagar> genericRepository;
        private ICategoriaContasAPagarRepository<CategoriaContasAPagar> categoriaContasAPagarRepository;
        private IGenericRepository<CategoriaContasAPagarPlanoContas> categoriaContasAPagarPlanoContasRepository;
        private ICategoriaContasAPagarPlanoContasRepository<CategoriaContasAPagarPlanoContas> planoContasRepository;
        public CategoriaContasAPagarController(IGenericRepository<CategoriaContasAPagar> genericRepository,
        ICategoriaContasAPagarRepository<CategoriaContasAPagar> categoriaContasAPagarRepository,
        IGenericRepository<CategoriaContasAPagarPlanoContas> categoriaContasAPagarPlanoContasRepository,
        ICategoriaContasAPagarPlanoContasRepository<CategoriaContasAPagarPlanoContas> planoContasRepository)
        {
            this.genericRepository = genericRepository;
            this.categoriaContasAPagarRepository = categoriaContasAPagarRepository;
            this.categoriaContasAPagarPlanoContasRepository = categoriaContasAPagarPlanoContasRepository;
            this.planoContasRepository = planoContasRepository;
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

                Expression<Func<CategoriaContasAPagar, bool>> p1, p2;
                var predicate = PredicateBuilder.New<CategoriaContasAPagar>();
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
                return BadRequest(string.Concat("Falha no carregamento das categorias: ", ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize()]
        public IActionResult Get(int id)
        {
            try
            {
                return new JsonResult(categoriaContasAPagarRepository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest("Categoria não encontrada!" + ex.Message);
            }
        }

        [HttpGet("getPlanoContas/{id}")]
        [Authorize()]
        public IActionResult GetPlanoContas(int id)
        {
            try
            {
                return new JsonResult(planoContasRepository.Where(x => x.CategoriaContasAPagarId == id).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest("Categoria não encontrada!" + ex.Message);
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
        [Route("save")]
        [Authorize()]
        public IActionResult Save(CategoriaContasAPagar categoriaContasAPagar)
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
                if (categoriaContasAPagar.Id > decimal.Zero)
                {
                    var entityBase = categoriaContasAPagarRepository.Get(categoriaContasAPagar.Id);
                    entityBase.Nome = categoriaContasAPagar.Nome;
                    entityBase.UpdateApplicationUserId = id;
                    entityBase.UpdateDate = DateTime.Now;
                    genericRepository.Update(entityBase);
                    var toDelete = entityBase.contas.Except(categoriaContasAPagar.contas, new EqualityComparer()).ToList();
                    var toInsert = categoriaContasAPagar.contas.Except(entityBase.contas, new EqualityComparer()).ToList();
                    toDelete.ForEach(x =>
                    {
                        categoriaContasAPagarPlanoContasRepository.Delete(x);
                    });
                    toInsert.ForEach(x =>
                    {
                        x.CategoriaContasAPagarId = entityBase.Id;
                        categoriaContasAPagarPlanoContasRepository.Insert(x);
                    });

                }
                else
                {
                    if (genericRepository.Where(x => x.Nome == categoriaContasAPagar.Nome && x.EmpresaId == empresaId).Any())
                    {
                        return BadRequest("Categoria já cadastrada com esse nome.");
                    }
                    categoriaContasAPagar.ApplicationUserId = id;
                    categoriaContasAPagar.CreateDate = DateTime.Now;
                    categoriaContasAPagar.EmpresaId = empresaId;
                    genericRepository.Insert(categoriaContasAPagar);
                    categoriaContasAPagar.contas.ForEach(conta =>
                    {
                        categoriaContasAPagarPlanoContasRepository.Insert(new CategoriaContasAPagarPlanoContas()
                        { 
                            CategoriaContasAPagarId = categoriaContasAPagar.Id,
                             PlanoContasId = conta.PlanoContasId
                        });
                    });

                }
                return new OkResult();

            }
            catch (Exception ex)
            {
                return BadRequest("Falha na conversão do arquivo - " + ex.Message);
            }
        }

        class EqualityComparer : IEqualityComparer<CategoriaContasAPagarPlanoContas>
        {
            public bool Equals(CategoriaContasAPagarPlanoContas x, CategoriaContasAPagarPlanoContas y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;
                if (x == null || y == null)
                    return false;
                return x.CategoriaContasAPagarId == y.CategoriaContasAPagarId && x.PlanoContasId == y.PlanoContasId;
            }

            public int GetHashCode(CategoriaContasAPagarPlanoContas obj)
            {
                return obj.Id.GetHashCode();
            }
        }


    }
}
