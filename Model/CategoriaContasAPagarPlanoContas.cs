using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("CategoriaContasAPagarPlanoContas")]
    public class CategoriaContasAPagarPlanoContas : BaseEntity
    {
        public int CategoriaContasAPagarId { get; set; }
        public int PlanoContasId { get; set; }

        [NotMapped]
        public PlanoContas PlanoContas { get; set; }
    }
}
