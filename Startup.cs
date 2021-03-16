using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Model;
using Repositorys;
using System.Text;
using UnitOfWork;

namespace financeiroAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("connectiondatabase")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                         .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IEmpresaRepository<>), typeof(EmpresaRepository<>));
            services.AddScoped(typeof(IContaDeParaRepository<>), typeof(ContaDeParaRepository<>));
            services.AddScoped(typeof(IContaCorrenteRepository<>), typeof(ContaCorrenteRepository<>));
            services.AddScoped(typeof(IArquivoEntradaRepository<>), typeof(ArquivoEntradaRepository<>));
            services.AddScoped(typeof(IFornecedorRepository<>), typeof(FornecedorRepository<>));
            services.AddScoped(typeof(IPlanoContasRepository<>), typeof(PlanoContasRepository<>));
            services.AddScoped(typeof(ICentroCustoRepository<>), typeof(CentroCustoRepository<>));
            services.AddScoped(typeof(IContasAPagarRepository<>), typeof(ContasAPagarRepository<>));
            services.AddScoped(typeof(ICategoriaContasAPagarRepository<>), typeof(CategoriaContasAPagarRepository<>));
            services.AddScoped(typeof(ICategoriaContasAPagarPlanoContasRepository<>), typeof(CategoriaContasAPagarPlanoContasRepository<>));
            services.AddScoped(typeof(IEmpresaAspNetUsersRepository<>), typeof(EmpresaAspNetUsersRepository<>));
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            var key = Encoding.ASCII.GetBytes(Configuration["secretJwt"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
