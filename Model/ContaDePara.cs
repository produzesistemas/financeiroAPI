using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("ContaDePara")]
    public class ContaDePara : BaseEntity
    {
        public string De { get; set; }
        public string Para { get; set; }
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
