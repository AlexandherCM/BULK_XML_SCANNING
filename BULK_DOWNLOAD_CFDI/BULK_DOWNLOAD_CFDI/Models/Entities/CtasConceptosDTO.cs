using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.Models.Entities
{   
    public class CtasConceptosDTO
    {
        [StringLength(36)]
        public string ID { get; set; }

        [StringLength(36)]
        public string FacturaID { get; set; }   

        [StringLength(8)]
        public string ClaveProductoServicio { get; set; }

        [StringLength(5)]
        public string ClaveUnidad { get; set; }
        public int Cantidad { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteFinal { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; }
    }

    [Table("CtasConceptos")]        
    public class CtasConceptos        
    {
        [Key]
        [StringLength(36)]
        public string ID { get; set; }

        [StringLength(36)]
        public string CtasFacturasID { get; set; }        
            
        [StringLength(8)]   
        public string ClaveProductoServicio { get; set; }   

        [StringLength(5)]
        public string ClaveUnidad { get; set; }
        public int Cantidad { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteFinal { get; set; }
        public decimal IVA { get; set; }
        public decimal ISR { get; set; }
        public decimal IEPS { get; set; } 
    }

}