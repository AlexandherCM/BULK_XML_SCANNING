using Microsoft.Data.SqlClient.Server;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.PaternDesign.DTOs
{
    //DTO QUE TRANSPORTA LA INFO DE LA FACTURA
    public class FacturaDataRecordOrderDTO  
    {
        public SqlDataRecord Factura { get; set; }   
        public List<SqlDataRecord>? Conceptos { get; set; }
        public List<DataRecordPagosYRelacionadosDTO>? PagosYDtosRelacionados { get; set; }
    }
           
    //DTO QUE TRANSPORTA LA INFO QUE CONTIENE LOS PAGOS Y DOCS RELACIONADOS
    public class DataRecordPagosYRelacionadosDTO
    {
        public SqlDataRecord Pago { get; set; } 
        public List<SqlDataRecord> Relacionados { get; set; }
    }
}
