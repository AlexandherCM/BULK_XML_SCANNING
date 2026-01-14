using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{   
    [Table("CtasFacturas")]
    public class CtasFacturas
    {
        [Key]
        [StringLength(36)]
        public string ID { get; set; }
        public float Version { get; set; }
        public bool Emitida { get; set; }

        [StringLength(36)]
        public string UUID { get; set; }

        [StringLength(255)]
        public string FolioInterno { get; set; }

        [StringLength(255)]
        public string Folio { get; set; }

        [StringLength(255)]
        public string Serie { get; set; }

        [StringLength(255)]
        public string SerieInterna { get; set; }

        [StringLength(13)]
        public string RFC_Emisor { get; set; }

        [StringLength(255)]
        public string Nombre_Emisor { get; set; }

        [StringLength(3)]
        public string RegimenEmisor { get; set; }

        [StringLength(13)]
        public string RFC_Receptor { get; set; }

        [StringLength(255)]
        public string Nombre_Receptor { get; set; }

        [StringLength(3)]
        public string RegimenReceptor { get; set; }

        [StringLength(5)]
        public string CP_Expedicion { get; set; }

        [StringLength(1)]
        public string TipoComprobante { get; set; }

        [StringLength(2)]
        public string FormaPago { get; set; }

        [StringLength(3)]
        public string MetodoPago { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public decimal subtotal_MXN { get; set; }
        public decimal descuento_MXN { get; set; }
        public decimal total_MXN { get; set; }
        public decimal MontoPendiente { get; set; }

        [StringLength(255)]
        public string Condiciones { get; set; }
        public decimal TipoCambio { get; set; }

        [StringLength(10)]
        public string Moneda { get; set; }

        [StringLength(4)]
        public string UsoCFDI { get; set; }
        public decimal IVA_Retenido { get; set; }
        public decimal IVA_Trasladado { get; set; }
        public decimal ISR_Retenido { get; set; }
        public decimal ISR_Trasladado { get; set; }
        public decimal IEPS_Retenido { get; set; }
        public decimal IEPS_Trasladado { get; set; }
        public long FechaCobroPago { get; set; }
        public long FechaCargado { get; set; }
        public long FechaCreacion { get; set; }
        public long FechaTimbrado { get; set; }
        public bool Expirada { get; set; }
        public bool Timbrada { get; set; }
        public bool Estatus { get; set; }
        public string Descripcion { get; set; }

        // Relación uno a muchos        
        public virtual ICollection<CtasConceptos> Conceptos { get; set; }
        public virtual ICollection<CtasPagosFactura> PagosFactura { get; set; }
    }


}