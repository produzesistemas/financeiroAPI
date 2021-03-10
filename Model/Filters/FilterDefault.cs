
using System;

namespace Model.Filters
{
    public class FilterDefault
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int FornecedorId { get; set; }
        public int PlanoContasId { get; set; }
        public int CentroCustoId { get; set; }
        public int CategoriaContasAPagarId { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
    }
}
