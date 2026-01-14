using BULK_DOWNLOAD_CFDI.Models;
using BULK_DOWNLOAD_CFDI.PaternDesign.Components;
using BULK_DOWNLOAD_CFDI.PaternDesign.DTOs;
using CFDI4._0.CFDI;
using CFDI4._0.CFDI.EsquemaCFDI3_3;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace BULK_DOWNLOAD_CFDI.PaternDesign
{
    public class Mediator
    {
        private IConfiguration _configuration;

        private CompressedXmlReader _chargeCFDIs;
        private XmlToCtasFacturaMapper _xmlToCtasFacturaMapper; 
        private FacturaSqlUploader _facturaSqlUploader;     

        public Mediator(Context context, IConfiguration configuration)
        {
            _configuration = configuration;

            _xmlToCtasFacturaMapper = new(_configuration);
            _facturaSqlUploader = new(_configuration);
            _chargeCFDIs = new(this);
        }

        //FUNCIÓN GRAL PARA VOLCAR MANIFIESTOS
        public async Task DumpCFDIs()
        {
            var records = await _chargeCFDIs.ProcesarComprimidos(_configuration["RutaCFDIs"]);
            await _facturaSqlUploader.SubirFacturasAsync(records);
        }
                
        public List<FacturaDataRecordOrderDTO> NotiFyMapper4_0(List<Comprobante> CFDIs) 
            => _xmlToCtasFacturaMapper.GetSqlDataRecords4_0(CFDIs); 
            
        public List<FacturaDataRecordOrderDTO> NotiFyMapper3_3(List<Comprobante3_3> CFDIs) 
            => _xmlToCtasFacturaMapper.GetSqlDataRecords3_3(CFDIs);
        
    }
}
