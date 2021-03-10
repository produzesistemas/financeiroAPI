using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Model
{
     [Table("ArquivoEntrada")]
    public class ArquivoEntrada : BaseEntity
    {
        public string Descricao { get; set; }
        public int ColunaData { get; set; }
        public int ColunaHistorico { get; set; }
        public int? ColunaValorDebito { get; set; }
        public int? ColunaContaDebito { get; set; }
        public int? ColunaValorCredito { get; set; }
        public int? ColunaContaCredito { get; set; }
        public int ColunaNLancamento { get; set; }
        public string ContaTransitoria { get; set; }
        public bool IsDebito { get; set; }
        public bool IsCredito { get; set; }
        public bool HasMapeamento { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateApplicationUserId { get; set; }
        public string ApplicationUserId { get; set; }
        public int EmpresaId { get; set; }

        [NotMapped]
        public string CriadoPor { get; set; }
        [NotMapped]
        public string AlteradoPor { get; set; }
    }
}
