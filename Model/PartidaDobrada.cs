using System;
namespace Model
{
    public class PartidaDobrada
    {
        public DateTime DataLancamento { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCredito { get; set; }
        public string  ContaDebito { get; set; }
        public string ContaCredito { get; set; }
        public string Historico { get; set; }
        public string CodigoHistorico { get; set; }
        public string CodigoLancamento { get; set; }
    }
}
