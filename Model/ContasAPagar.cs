using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("ContasAPagar")]
    public class ContasAPagar : BaseEntity
    {
        public string Referente { get; set; }
        public int SituacaoContaId { get; set; }
        public int FornecedorId { get; set; }
        public int? CentroCustoId { get; set; }
        public int? PlanoContasId { get; set; }
        public int? CategoriaContasAPagarId { get; set; }
        public int? ContaCorrenteId { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal? ValorPago { get; set; }
        public decimal? Juros { get; set; }
        public decimal? Multa { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
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
        public SituacaoConta SituacaoConta { get; set; }
        [NotMapped]
        public Fornecedor Fornecedor { get; set; }
        [NotMapped]
        public CentroCusto CentroCusto { get; set; }
        [NotMapped]
        public PlanoContas PlanoContas { get; set; }
        [NotMapped]
        public ContaCorrente ContaCorrente { get; set; }
        [NotMapped]
        public CategoriaContasAPagar CategoriaContasAPagar { get; set; }
    }
}
