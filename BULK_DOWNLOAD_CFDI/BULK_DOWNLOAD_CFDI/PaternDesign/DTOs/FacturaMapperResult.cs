using Microsoft.Data.SqlClient.Server;
#pragma warning disable CS8618

namespace BULK_DOWNLOAD_CFDI.PaternDesign.DTOs
{
    public class FacturaMapperResult
    {
        public List<SqlDataRecord> Facturas { get; set; }
        public List<SqlDataRecord> Conceptos { get; set; }
        public List<SqlDataRecord> Pagos { get; set; }
        public List<SqlDataRecord> Relacionados { get; set; }
    }

    public class FacturaDataRecordOrderDTO  
    {
        public SqlDataRecord Factura { get; set; }   
        public List<SqlDataRecord>? Conceptos { get; set; }
        public List<DataRecordPagosYRelacionadosDTO>? PagosYDtosRelacionados { get; set; }
    }
            
    public class DataRecordPagosYRelacionadosDTO
    {
        public SqlDataRecord Pago { get; set; } 
        public List<SqlDataRecord> Relacionados { get; set; }
    }

}
