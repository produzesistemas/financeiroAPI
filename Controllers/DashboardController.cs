using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private IGenericRepository<ContasAPagar> genericRepository;
        public DashboardController(
            IGenericRepository<ContasAPagar> genericRepository
            )
        {
            this.genericRepository = genericRepository;
        }

        [HttpGet("getDashboard")]
        [Authorize()]
        public IActionResult GetDashboard()
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

                Expression<Func<ContasAPagar, bool>> p1, p2, p3, p4, p5, p6;
                var predicate = PredicateBuilder.New<ContasAPagar>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                p2 = p => p.SituacaoContaId == 1;
                predicate = predicate.And(p2);
                p3 = p => p.DataVencimento.Month == DateTime.Now.Month && p.DataVencimento.Year == DateTime.Now.Year;
                predicate = predicate.And(p3);

                var predicatePaga = PredicateBuilder.New<ContasAPagar>();
                p4 = p => p.EmpresaId == empresaId;
                predicatePaga = predicatePaga.And(p4);
                p5 = p => p.SituacaoContaId == 2;
                predicatePaga = predicatePaga.And(p5);
                p6 = p => p.DataPagamento.Value.Month == DateTime.Now.Month && p.DataPagamento.Value.Year == DateTime.Now.Year;
                predicatePaga = predicatePaga.And(p6);


                var dashboard = new Dashboard();
                dashboard.TotalAPagarMes = genericRepository.Where(predicate).Sum(x => x.ValorOriginal);
                dashboard.TotalPagoMes = genericRepository.Where(predicatePaga).Sum(x => x.ValorPago.Value);
                return new JsonResult(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento das contas: ", ex.Message));
            }
        }

    }
}
