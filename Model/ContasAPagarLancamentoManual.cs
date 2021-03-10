
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("ContasAPagarLancamentoManual")]
    public class ContasAPagarLancamentoManual
    {
        public string Referente { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorOriginal { get; set; }
        public DateTime DataVencimento { get; set; }
        public Fornecedor Fornecedor { get; set; }
        public PlanoContas PlanoContas { get; set; }
        public CategoriaContasAPagar CategoriaContasAPagar { get; set; }
        public CentroCusto CentroCusto { get; set; }
        public ContaCorrente ContaCorrente { get; set; }

    }
}
