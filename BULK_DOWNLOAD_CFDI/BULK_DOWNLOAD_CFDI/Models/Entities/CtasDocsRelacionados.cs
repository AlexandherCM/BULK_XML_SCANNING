using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{
    [Table("CtasDocsRelacionados")]
    public class CtasDocsRelacionados
    {
        [Key]
        [StringLength(36)]
        public string ID { get; set; }

        [StringLength(36)]
        public string PagoID { get; set; }

        [StringLength(36)]
        public string FacturaPPD_UUID { get; set; }

        [StringLength(36)]
        public string FacturaPPD_ID { get; set; }

        [StringLength(2)]
        public string FormaPago { get; set; }

        public int NumParcialidad { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal ImpPagado { get; set; }
        public decimal IVA_Retenido { get; set; }
        public decimal IVA_Trasladado { get; set; }
        public decimal ISR_Retenido { get; set; }
        public decimal ISR_Trasladado { get; set; }
        public decimal IEPS_Retenido { get; set; }
        public decimal IEPS_Trasladado { get; set; }
        public bool Contemplado { get; set; }

        [ForeignKey(nameof(PagoID))]
        public CtasPagosFactura Pago { get; set; }
    }


}