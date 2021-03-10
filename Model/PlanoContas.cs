using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("PlanoContas")]
    public class PlanoContas : BaseEntity
    {
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public string Classificacao { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateApplicationUserId { get; set; }
        public int EmpresaId { get; set; }
        public int TipoContaId { get; set; }
        public string ApplicationUserId { get; set; }

        [NotMapped]
        public string CriadoPor { get; set; }
        [NotMapped]
        public string AlteradoPor { get; set; }
        [NotMapped]
        public TipoConta TipoConta { get; set; }
    }
}
