using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("EmpresaAspNetUsers")]
    public class EmpresaAspNetUsers : BaseEntity
    {
        public string ApplicationUserId { get; set; }
        public int EmpresaId { get; set; }

        [NotMapped]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
