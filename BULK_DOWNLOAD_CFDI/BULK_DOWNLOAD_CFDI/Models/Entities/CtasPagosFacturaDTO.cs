using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{   
    public class CtasPagosFacturaDTO
    {
        public string ID { get; set; }  
        public string FacturaPagoID { get; set; }

        [StringLength(2)]
        public string FormaPago { get; set; }
        public int NoPago { get; set; }

        [StringLength(100)]
        public string NumOperacion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; }

        public virtual ICollection<CtasDocsRelacionadosDTO> DocsRelacionados { get; set; }
    }   

    [Table("CtasPagosFactura")]
    public class CtasPagosFactura
    {
        [Key]
        [StringLength(36)]
        [Column(Order = 0)]
        public string ID { get; set; }

        [StringLength(36)]  
        public string CtasFacturaPagoID { get; set; }   

        [StringLength(2)]
        public string FormaPago { get; set; }
        public int NoPago { get; set; }

        [StringLength(100)]
        public string NumOperacion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; }    
    }
    
    
}