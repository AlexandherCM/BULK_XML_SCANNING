using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{   
    [Table("CtasConceptos")]
    public class CtasConceptos
    {
        [Key]
        [StringLength(36)]
        public string ID { get; set; }

        [StringLength(36)]
        public string FacturaID { get; set; }

        [StringLength(8)]
        public string ClaveProductoServicio { get; set; }

        [StringLength(5)]
        public string ClaveUnidad { get; set; }
        public int Cantidad { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal IVA_Retenido { get; set; }
        public decimal IVA_Trasladado { get; set; }
        public decimal ISR_Retenido { get; set; }
        public decimal ISR_Trasladado { get; set; }
        public decimal IEPS_Retenido { get; set; }
        public decimal IEPS_Trasladado { get; set; }
        public decimal ImporteFinal { get; set; }


        [ForeignKey(nameof(FacturaID))]
        public CtasFacturas Factura { get; set; }
    }

}