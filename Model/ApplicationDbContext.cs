using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Model
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<EmpresaAspNetUsers> EmpresaAspNetUsers { get; set; }
        public DbSet<ArquivoEntrada> ArquivoEntrada { get; set; }
        public DbSet<ContaDePara> ContaDePara { get; set; }
        public DbSet<ContaCorrente> ContaCorrente { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<PlanoContas> PlanoContas { get; set; }
        public DbSet<CentroCusto> CentroCusto { get; set; }
        public DbSet<TipoConta> TipoConta { get; set; }
        public DbSet<SituacaoConta> SituacaoConta { get; set; }

        public DbSet<ContasAPagar> ContasAPagar { get; set; }
        public DbSet<CategoriaContasAPagar> CategoriaContasAPagar { get; set; }
        public DbSet<CategoriaContasAPagarPlanoContas> CategoriaContasAPagarPlanoContas { get; set; }
    }
}
