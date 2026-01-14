using System.IO.Compression;
using BULK_DOWNLOAD_CFDI.PaternDesign.DTOs;
using CFDI4._0.CFDI;
using CFDI4._0.CFDI.EsquemaCFDI3_3;
using CFDI4._0.ToolsXML.ProcessCFDI;
using Microsoft.Data.SqlClient.Server;
#pragma warning disable CS8618
#pragma warning disable CS0108

namespace BULK_DOWNLOAD_CFDI.PaternDesign.Components
{
    public class CompressedXmlReader : BaseComponent
    {
        private readonly ScanXML _scan;
        private readonly Mediator _mediator;

        private readonly List<Comprobante> _comprobante4_0List;
        private readonly List<Comprobante3_3> _comprobante3_3List;

        public CompressedXmlReader(Mediator mediator)
        {
            _scan = new ScanXML();
            _mediator = mediator;
            _comprobante4_0List = new();
            _comprobante3_3List = new();
        }

        public async Task<List<FacturaDataRecordOrderDTO>> ProcesarComprimidos(string rutaGz)
        {
            List<FacturaDataRecordOrderDTO> result = new();

            using var archivoGz = File.OpenRead(rutaGz);
            using var zipArchive = new ZipArchive(archivoGz, ZipArchiveMode.Read);

            foreach (var entry in zipArchive.Entries)
            {
                if (!entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) continue;

                using var stream = entry.Open();
                using var reader = new StreamReader(stream);
                string contenidoXml = await reader.ReadToEndAsync();

                var version = _scan.GetVersionFromXml(contenidoXml);

                if (version == 3.3f)
                {
                    var comprobante33 = _scan.ObtenerDatosCFDI3_3();
                    if (comprobante33 != null)
                        _comprobante3_3List.Add(comprobante33);
                }
                else if (version == 4.0f)
                {
                    var comprobante40 = _scan.ObtenerDatosCFDI4_0();
                    if (comprobante40 != null)
                        _comprobante4_0List.Add(comprobante40);
                }
            }

            if (_comprobante4_0List.Any())
                result.AddRange(_mediator.NotiFyMapper4_0(_comprobante4_0List));

            if (_comprobante3_3List.Any())
                result.AddRange(_mediator.NotiFyMapper3_3(_comprobante3_3List));

            return result;
        }
    }



}
