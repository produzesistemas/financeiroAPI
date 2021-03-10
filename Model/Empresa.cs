using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Model
{
     [Table("Empresa")]
    public class Empresa : BaseEntity
    {
        public string Nome { get; set; }
        public string NomeImagem { get; set; }
        public Nullable<bool> Ativo { get; set; }
        public string Cnpj { get; set; }
        public string ContaTransitoria { get; set; }
        public int? CodigoFilial { get; set; }
        public string NomeUsuarioDominio { get; set; }

        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        public List<IdentityRole> Modulos;
        public Empresa()
        {
            Modulos = new List<IdentityRole>();
        }
    }
}
