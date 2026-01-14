using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{
    public class CtasDocsRelacionadosDTO
    {
        [StringLength(36)]
        public string PagoID { get; set; }

        [StringLength(36)]
        public string FacturaPPD_ID { get; set; }

        public int NumParcialidad { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal ImpPagado { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; }
    }
        
    [Table("CtasDocsRelacionados")]
    public class CtasDocsRelacionados
    {
        [StringLength(36)]
        public string PagoID { get; set; }

        [StringLength(36)]
        public string FacturaPPD_ID { get; set; }

        public int NumParcialidad { get; set; }
        public decimal ImpSaldoInsoluto { get; set; }
        public decimal ImpSaldoAnt { get; set; }
        public decimal ImpPagado { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; }

        [ForeignKey(nameof(PagoID))]
        public virtual CtasPagosFacturaDTO Pago { get; set; }

        [ForeignKey(nameof(FacturaPPD_ID))]
        public virtual CtasFacturasDTO Factura { get; set; }
    }
    
    
}