using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("TipoConta")]
    public class TipoConta : BaseEntity
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }
    }
}
