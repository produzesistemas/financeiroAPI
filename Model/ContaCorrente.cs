using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("ContaCorrente")]
    public class ContaCorrente : BaseEntity
    {
        public string Agencia { get; set; }
        public string Banco { get; set; }
        public string Conta { get; set; }
        public int BancoNumero { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateApplicationUserId { get; set; }
        public int EmpresaId { get; set; }
        public string ApplicationUserId { get; set; }
        public decimal SaldoInicial { get; set; }

        [NotMapped]
        public string CriadoPor { get; set; }
        [NotMapped]
        public string AlteradoPor { get; set; }
    }
}
