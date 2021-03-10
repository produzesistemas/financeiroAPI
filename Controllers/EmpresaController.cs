using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Filters;
using Newtonsoft.Json;
using UnitOfWork;
namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private IGenericRepository<Empresa> genericRepository;
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IEmpresaRepository<Empresa> empresaRepository;
        private readonly UserManager<IdentityUser> userManager;
        private IConfiguration configuration;
        private IWebHostEnvironment _hostEnvironment;

        public EmpresaController(IGenericRepository<Empresa> genericRepository,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
           IEmpresaRepository<Empresa> empresaRepository,
           UserManager<IdentityUser> userManager,
           IWebHostEnvironment environment,
           IConfiguration Configuration)
        {
            this.genericRepository = genericRepository;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;
            this.empresaRepository = empresaRepository;
            this.userManager = userManager;
            this.configuration = Configuration;
            this._hostEnvironment = environment;
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
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }

                return new JsonResult(empresaRepository.GetAll().ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento empresas: ", ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                return new JsonResult(empresaRepository.Get(id));
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no carregamento das Empresas - " + ex.Message);
            }

        }

        [HttpPost()]
        [Route("save")]
        [Authorize()]
        public async Task<IActionResult> Save(Empresa empresa)
        {
            try
            {
                if (empresa.Id > decimal.Zero)
                {
                    var empresaBase = genericRepository.Get(empresa.Id);
                    empresaBase.Nome = empresa.Nome;
                    empresaBase.Cnpj = empresa.Cnpj;
                    empresaBase.CodigoFilial = empresa.CodigoFilial;
                    empresaBase.NomeUsuarioDominio = empresa.NomeUsuarioDominio;
                    empresaBase.ContaTransitoria = empresa.ContaTransitoria;
                    genericRepository.Update(empresaBase);
                } else
                {
                    var exist = await userManager.FindByEmailAsync(empresa.Email);
                    if (exist != null)
                    {
                        return BadRequest("Email já usado para outra Empresa!");
                    }
                    var user = new IdentityUser()
                    {
                        Email = empresa.Email,
                        UserName = empresa.Email
                    };
                    var result = await userManager.CreateAsync(user, "Fin@nceiro2021");
                    if (result.Succeeded)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Role, "Empresa"));
                        empresa.Modulos.ForEach(modulo =>
                        {
                            claims.Add(new Claim(ClaimTypes.Role, modulo.Name));
                        });
                        await userManager.AddClaimsAsync(user, claims);
                        empresa.Ativo = true;
                        genericRepository.Insert(empresa);
                        var empresaAspNetUsers = new EmpresaAspNetUsers()
                        {
                            EmpresaId = empresa.Id,
                            ApplicationUserId = user.Id
                        };
                        empresaAspNetUsersRepository.Insert(empresaAspNetUsers);
                        sendEmail(empresa, user, "Fin@nceiro2021");
                    }
                    else
                    {
                        return BadRequest(result.Errors.FirstOrDefault().Description);
                    }
                    
                }
               
                return new OkResult();

            }
            catch (Exception ex)
            {
                return BadRequest("Falha no save das Empresas - " + ex.Message);
            }
        }

        private void sendEmail(Empresa empresa, IdentityUser user, string secret)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(configuration["FromEmail"].ToString());
                mail.To.Add(empresa.Email);
                mail.Subject = "Seu cadastrado foi efetuado com sucesso.";
                mail.Body = "" +
                    "<div> Sr. " + empresa.Nome + "</div>" +
                    "<div></div>" +
                    "<div>Seu cadastro foi efetuado com sucesso.</div>" +
                     "<div>Login: " + empresa.Email + "</div>" +
                     "<div>Senha: " + secret + "</div>";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(configuration["STMPEmail"].ToString(), Convert.ToInt32(configuration["PortEmail"].ToString()));
                smtp.Credentials = new System.Net.NetworkCredential(configuration["UserEmail"].ToString(), configuration["PassEmail"].ToString());
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientException ex)
            {
                throw ex;
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet()]
        [Route("getByUser")]
        [Authorize()]
        public IActionResult GetByUser()
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                if (id == null)
                {
                    return BadRequest("Usuário não encontrado! Efetue o login.");
                }
                return new JsonResult(empresaRepository.Get(empresaAspNetUsersRepository.Where(x => x.ApplicationUserId == id).FirstOrDefault().Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro no servidor: {ex}");
            }
        }

        [HttpPost()]
        [Route("update")]
        [Authorize()]
        public IActionResult Update()
        {
            try
            {
                var empresa = JsonConvert.DeserializeObject<Empresa>(Convert.ToString(Request.Form["empresa"]));
                var pathToSave = string.Concat(_hostEnvironment.ContentRootPath, configuration["pathFileEmpresa"]);
                var fileDelete = pathToSave;
                var empresaBase = genericRepository.Get(empresa.Id);
                if (Request.Form.Files.Count() > decimal.Zero)
                {
                    var extension = Path.GetExtension(Request.Form.Files[0].FileName);
                    var fileName = string.Concat(Guid.NewGuid().ToString(), extension);
                    using (var stream = new FileStream(Path.Combine(pathToSave, fileName), FileMode.Create))
                    {
                        Request.Form.Files[0].CopyTo(stream);
                    }
                    fileDelete = string.Concat(fileDelete, empresaBase.NomeImagem);
                    empresaBase.NomeImagem = fileName;
                }
                    empresaBase.Nome = empresa.Nome;
                    empresaBase.CodigoFilial = empresa.CodigoFilial;
                    empresaBase.NomeUsuarioDominio = empresa.NomeUsuarioDominio;
                    empresaBase.ContaTransitoria = empresa.ContaTransitoria;
                    genericRepository.Update(empresaBase);
                if (System.IO.File.Exists(fileDelete))
                {
                    System.IO.File.Delete(fileDelete);
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no save das Empresas - " + ex.Message);
            }
        }
    }
}
