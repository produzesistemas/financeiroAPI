using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System;
using System.Linq;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoContaController : ControllerBase
    {
        private IGenericRepository<TipoConta> genericRepository;

        public TipoContaController(IGenericRepository<TipoConta> genericRepository)
        {
            this.genericRepository = genericRepository;
        }

        [HttpGet()]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                return new JsonResult(genericRepository.GetAll().ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }
    }
}
