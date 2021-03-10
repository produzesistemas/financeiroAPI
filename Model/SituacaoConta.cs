
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("SituacaoConta")]
    public class SituacaoConta : BaseEntity
    {
        public string Nome { get; set; }
    }
}
