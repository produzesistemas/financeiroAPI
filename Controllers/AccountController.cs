using financeiroAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository;
        private IGenericRepository<Empresa> empresaRepository;

        public AccountController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration Configuration,
            IGenericRepository<EmpresaAspNetUsers> empresaAspNetUsersRepository,
            IGenericRepository<Empresa> empresaRepository,
            IWebHostEnvironment environment)
        {
            //this._hostEnvironment = environment;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.configuration = Configuration;
            this.empresaAspNetUsersRepository = empresaAspNetUsersRepository;
            this.empresaRepository = empresaRepository;
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
                var user = userManager.FindByEmailAsync(loginUser.Email);

                var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user.Result);
                var claims = claimsPrincipal.Claims.ToList();
                var permissions = claims.Where(c => c.Type.Contains("role")).Select(c => c.Value).ToList();
                if (permissions.Where(x => x.Contains("Master")).Any())
                {
                    return BadRequest("Acesso negado! Usuário é Master!");
                }

                var applicationUserDTO = new ApplicationUserDTO();
                var empresa = empresaRepository.Get(empresaAspNetUsersRepository.Where(x => x.ApplicationUserId == user.Result.Id).FirstOrDefault().EmpresaId);
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

    }
}
