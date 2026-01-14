using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{   
    [Table("CtasPagosFactura")]
    public class CtasPagosFactura
    {
        [Key]
        [StringLength(36)]
        public string ID { get; set; }

        [StringLength(36)]
        public string FacturaPagoID { get; set; }

        [StringLength(2)]
        public string FormaPago { get; set; }
        public int NoPago { get; set; }

        [StringLength(100)]
        public string NumOperacion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal IVA_Retenido { get; set; }
        public decimal IVA_Trasladado { get; set; }
        public decimal ISR_Retenido { get; set; }
        public decimal ISR_Trasladado { get; set; }
        public decimal IEPS_Retenido { get; set; }
        public decimal IEPS_Trasladado { get; set; }


        [ForeignKey(nameof(FacturaPagoID))]
        public CtasFacturas Factura { get; set; }
        public virtual ICollection<CtasDocsRelacionados> DocsRelacionados { get; set; }
    }


}