using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("Fornecedor")]
    public class Fornecedor : BaseEntity
    {
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public string Cnpj { get; set; }
        public string Contato { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
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
