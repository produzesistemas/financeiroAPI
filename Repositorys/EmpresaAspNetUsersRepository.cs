using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnitOfWork;

namespace Repositorys
{
    public class EmpresaAspNetUsersRepository<T> : IEmpresaAspNetUsersRepository<EmpresaAspNetUsers> where T : BaseEntity
    {
        private DbSet<IdentityUser> users;
        private DbSet<EmpresaAspNetUsers> entities;
        private readonly ApplicationDbContext _context;

        public EmpresaAspNetUsersRepository(ApplicationDbContext context)
        {
            _context = context;
            users = context.Set<IdentityUser>();
            entities = context.Set<EmpresaAspNetUsers>();
        }

        public IQueryable<EmpresaAspNetUsers> Where(Expression<Func<EmpresaAspNetUsers, bool>> expression)
        {
            return entities.Select(x => new EmpresaAspNetUsers
            {
                Id = x.Id,
                EmpresaId = x.EmpresaId,
                ApplicationUser = new ApplicationUser
                {
                    Id = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).Id,
                    Email = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).Email,
                    UserName = users.FirstOrDefault(q => q.Id == x.ApplicationUserId).UserName
                }
            }).Where(expression).AsQueryable();
        }


        public void Insert(EmpresaAspNetUsers entity)
        {
            entities.Add(entity);
            _context.SaveChanges();
        }
    }
}
