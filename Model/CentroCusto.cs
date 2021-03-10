using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("CentroCusto")]
    public class CentroCusto : BaseEntity
    {
        public string Descricao { get; set; }
        public string Codigo { get; set; }
        public bool Ativo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateApplicationUserId { get; set; }
        public int EmpresaId { get; set; }
        public string ApplicationUserId { get; set; }

        [NotMapped]
        public string CriadoPor { get; set; }
        [NotMapped]
        public string AlteradoPor { get; set; }
    }
}
