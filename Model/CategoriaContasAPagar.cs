
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("CategoriaContasAPagar")]
    public class CategoriaContasAPagar : BaseEntity
    {
        public string Nome { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateApplicationUserId { get; set; }
        public int EmpresaId { get; set; }
        public string ApplicationUserId { get; set; }
        [NotMapped]
        public string CriadoPor { get; set; }
        [NotMapped]
        public string AlteradoPor { get; set; }
        [NotMapped]
        public List<CategoriaContasAPagarPlanoContas> contas; 
        public CategoriaContasAPagar()
        {
            contas = new List<CategoriaContasAPagarPlanoContas>();
        }
    }
}
