using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public string NomeImagem { get; set; }

        [NotMapped]
        public string Token { get; set; }

        [NotMapped]
        public List<string> Permissions { get; set; }

        public ApplicationUser()
        {
            this.Permissions = new List<string>();
        }

        public static explicit operator ApplicationUser(Task<IdentityUser> v)
        {
            throw new NotImplementedException();
        }
    }
}