using financeiroAPI.Security;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using UnitOfWork;

namespace financeiroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private IConfiguration configuration;
        //private IWebHostEnvironment _hostEnvironment;
        private IEmpresaAspNetUsersRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IGenericRepository<Empresa> empresaRepository;
        private IGenericRepository<EmpresaAspNetUsers> genericRepository;

        public AccountController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration Configuration,
            IEmpresaAspNetUsersRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
            IGenericRepository<Empresa> empresaRepository,
            IGenericRepository<EmpresaAspNetUsers> genericRepository,
            IWebHostEnvironment environment)
        {
            //this._hostEnvironment = environment;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.configuration = Configuration;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;
            this.empresaRepository = empresaRepository;
            this.genericRepository = genericRepository;
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
                if ((id == null) || (empresaId == 0))
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                Expression<Func<EmpresaAspNetUsers, bool>> p1;
                var predicate = PredicateBuilder.New<EmpresaAspNetUsers>();
                p1 = p => p.EmpresaId == empresaId;
                predicate = predicate.And(p1);
                return new JsonResult(empresaAspNetUsersRepository.Where(predicate).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(string.Concat("Falha no carregamento dos usuários: ", ex.Message));
            }
        }

        [HttpPost()]
        [AllowAnonymous]
        [Route("registerMaster")]
        public async Task<IActionResult> RegisterMaster(LoginUser loginUser)
        {
            try
            {
                var user = new IdentityUser
                {
                    UserName = loginUser.Email,
                    Email = loginUser.Email
                };
                var result = await userManager.CreateAsync(user, loginUser.Secret);
                    if (result.Succeeded)
                    {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Role, "Master"));
                    await userManager.AddClaimsAsync(user, claims);

                } else
                {
                    return BadRequest("Falha na criação do usuário!");
                }
                return new JsonResult(user);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }

        }

        [HttpPost()]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            try
            {
                    var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Secret, false, false);

                    if (!result.Succeeded)
                    {
                        return BadRequest("Acesso negado! Login inválido!");
                    }
                    var user = userManager.FindByEmailAsync(loginUser.Email);

                    var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user.Result);
                    var claims = claimsPrincipal.Claims.ToList();
                    var permissions = claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();
                    if (permissions.Where(x => x.Contains("Empresa")).Any())
                    {
                        return BadRequest("Acesso negado! Usuário não é Master!");
                    }

                    var applicationUserDTO = new ApplicationUserDTO();
                    applicationUserDTO.Token = TokenService.GenerateToken(user.Result, configuration, new Empresa() {  Id = 0 }, permissions);
                    applicationUserDTO.Email = user.Result.Email;
                    applicationUserDTO.UserName = user.Result.UserName;
                    return new JsonResult(applicationUserDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no login! " + ex.Message);
            }

        }


        [HttpPost()]
        [AllowAnonymous]
        [Route("loginEmpresa")]
        public async Task<IActionResult> LoginEmpresa(LoginUser loginUser)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Secret, false, false);

                if (!result.Succeeded)
                {
                    return BadRequest("Acesso negado! Login inválido!");
                }
                var user = userManager.FindByNameAsync(loginUser.Email);

                var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user.Result);
                var claims = claimsPrincipal.Claims.ToList();
                var permissions = claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();
                if (permissions.Where(x => x.Contains("Master")).Any())
                {
                    return BadRequest("Acesso negado! Usuário é Master!");
                }

                var applicationUserDTO = new ApplicationUserDTO();
                var empresa = empresaRepository.Get(genericRepository.Where(x => x.ApplicationUserId == user.Result.Id).FirstOrDefault().EmpresaId);
                applicationUserDTO.Token = TokenService.GenerateToken(user.Result, configuration, empresa, permissions);
                applicationUserDTO.Email = user.Result.Email;
                applicationUserDTO.UserName = user.Result.UserName;
                applicationUserDTO.NomeEmpresa = string.Concat(empresa.Id.ToString(), " - ", empresa.Nome);
                return new JsonResult(applicationUserDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Falha no login! " + ex.Message);
            }

        }

        [HttpGet()]
        [Route("getRoles")]
        public IActionResult GetRoles()
        {
            return new JsonResult(roleManager.Roles.Where(x => x.Id != "1" && x.Id != "2").ToList());
        }

        [HttpPost()]
        [Route("changePassword")]
        [Authorize()]
        public async Task<IActionResult> changePassword(ApplicationUserDTO usermodel)
        {
            ClaimsPrincipal currentUser = this.User;
            var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
            if (id == null)
            {
                return BadRequest("Identificação do usuário não encontrada.");
            }
            var user = await userManager.FindByIdAsync(id);
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, usermodel.Secret);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Não foi possível alterar a senha.");
            }
            return Ok();
        }

        [HttpPost()]
        [Route("register")]
        [Authorize()]
        public async Task<IActionResult> Register(LoginUser loginUser)
        {
            try
            {
                ClaimsPrincipal currentUser = this.User;
                var id = currentUser.Claims.FirstOrDefault(z => z.Type.Contains("primarysid")).Value;
                if (id == null)
                {
                    return BadRequest("Identificação do usuário não encontrada.");
                }
                var empresaId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(z => z.Type.Contains("sid")).Value);
                var claimscurrentUser = currentUser.Claims.ToList();
                var permissions = claimscurrentUser.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();
                var user = new IdentityUser
                {
                    UserName = loginUser.Email.Split("@")[0],
                    Email = loginUser.Email
                };
                var result = await userManager.CreateAsync(user, "Fin@nceiro2021");
                if (result.Succeeded)
                {
                    List<Claim> claims = new List<Claim>();
                    permissions.ForEach(permission =>
                    {
                        if (!permission.Equals("CEO"))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, permission));
                        }
                    });
                    await userManager.AddClaimsAsync(user, claims);
                    var empresaAspNetUsers = new EmpresaAspNetUsers()
                    {
                        EmpresaId = empresaId,
                        ApplicationUserId = user.Id
                    };
                    empresaAspNetUsersRepository.Insert(empresaAspNetUsers);
                    sendEmail(user, "Fin@nceiro2021");
                }
                else
                {
                    return BadRequest("Falha na criação do usuário!");
                }
                return new JsonResult(user);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }

        }

        private void sendEmail(IdentityUser user, string secret)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(configuration["FromEmail"].ToString());
                mail.To.Add(user.Email);
                mail.Subject = "Seu cadastrado foi efetuado com sucesso.";
                mail.Body = "" +
                    "<div> Sr. " + user.UserName + "</div>" +
                    "<div></div>" +
                    "<div>Seu cadastro foi efetuado com sucesso.</div>" +
                     "<div>Login: " + user.Email + "</div>" +
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

    }
}
