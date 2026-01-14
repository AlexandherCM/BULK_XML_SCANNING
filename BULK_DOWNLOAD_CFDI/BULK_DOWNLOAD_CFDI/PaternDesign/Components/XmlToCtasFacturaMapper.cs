using System.Data;
using System.Text.RegularExpressions;
using BULK_DOWNLOAD_CFDI.PaternDesign.DTOs;
using BULK_DOWNLOAD_CFDI.ToolsAndExtensions;
using CFDI4._0.CFDI;
using CFDI4._0.CFDI.EsquemaCFDI3_3;
using ComplementoPagos10;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Configuration;
#pragma warning disable CS8604
#pragma warning disable CS8602

namespace BULK_DOWNLOAD_CFDI.PaternDesign.Components
{
    public class XmlToCtasFacturaMapper : BaseComponent
    {
        private bool emitida = false;
        public XmlToCtasFacturaMapper(IConfiguration configuration) : base(configuration) { }

        public List<FacturaDataRecordOrderDTO> GetSqlDataRecords4_0(List<Comprobante> cfdis)
        {
            var metaFactura = ObtenerMetaFactura();
            var metaConcepto = ObtenerMetaConcepto();
            var metaPago = ObtenerMetaPago();
            var metaRelacionado = ObtenerMetaRelacionado();

            List<FacturaDataRecordOrderDTO> facturasDTO = new();

            foreach (var f in cfdis)
            {
                var facturaDto = new FacturaDataRecordOrderDTO
                {
                    Conceptos = new(),
                    PagosYDtosRelacionados = new()
                };

                var record = new SqlDataRecord(metaFactura);
                string facturaId = Guid.NewGuid().ToString();

                decimal tipoCambio = f.Moneda != null && !f.Moneda.Equals("MXN", StringComparison.OrdinalIgnoreCase) && f.TipoCambio > 0 ? f.TipoCambio : 1M;
                decimal subtotal_MXN = f.SubTotal * tipoCambio;
                decimal descuento_MXN = f.Descuento * tipoCambio;
                decimal total_MXN = f.Total * tipoCambio;

                DateTime fechaCreacion = DateTime.TryParse(f.Fecha, out var fc) ? fc : DateTime.Now;
                long epochCreacion = EpochHelper.CrearEpoch(fechaCreacion);
                long epochTimbrado = EpochHelper.CrearEpoch(f.TimbreFiscalDigital?.FechaTimbrado ?? fechaCreacion);
                long fechaCobroPago = 0;

                if (f.MetodoPago == "PPD" && !string.IsNullOrWhiteSpace(f.CondicionesDePago))
                {
                    var match = Regex.Match(f.CondicionesDePago, @"\d+");
                    if (match.Success && int.TryParse(match.Value, out int dias))
                        fechaCobroPago = EpochHelper.CrearEpoch(fechaCreacion.AddDays(dias));
                }

                bool expirada = fechaCobroPago > 0 && new DateTime(fechaCobroPago) < DateTime.Now;

                decimal ivaRetenido = 0, ivaTrasladado = 0;
                decimal isrRetenido = 0, isrTrasladado = 0;
                decimal iepsRetenido = 0, iepsTrasladado = 0;

                if (f.Impuestos != null)
                {
                    if (f.Impuestos?.Retenciones != null)
                    {
                        foreach (var r in f.Impuestos.Retenciones)
                        {
                            switch (r.Impuesto)
                            {
                                case "001": isrRetenido += r.Importe; break;
                                case "002": ivaRetenido += r.Importe; break;
                                case "003": iepsRetenido += r.Importe; break;
                            }
                        }
                    }

                    if (f.Impuestos?.Traslados != null)
                    {
                        foreach (var t in f.Impuestos.Traslados)
                        {
                            switch (t.Impuesto)
                            {
                                case "001": isrTrasladado += t.Importe; break;
                                case "002": ivaTrasladado += t.Importe; break;
                                case "003": iepsTrasladado += t.Importe; break;
                            }
                        }
                    }
                }

                record.SetString(0, facturaId);
                record.SetDouble(1, float.Parse(f.Version));
                record.SetBoolean(2, emitida);
                record.SetString(3, f.TimbreFiscalDigital?.UUID ?? "");
                record.SetString(4, f.Folio ?? "");
                record.SetString(5, f.Serie ?? "");
                record.SetString(6, f.Emisor?.Rfc ?? "");
                record.SetString(7, f.Emisor?.Nombre ?? "");
                record.SetString(8, f.Emisor?.RegimenFiscal ?? "");
                record.SetString(9, f.Receptor?.Rfc ?? "");
                record.SetString(10, f.Receptor?.Nombre ?? "");
                record.SetString(11, f.Receptor?.RegimenFiscalReceptor ?? "000");
                record.SetString(12, f.LugarExpedicion ?? "");
                record.SetString(13, f.TipoDeComprobante ?? "");
                record.SetString(14, f.FormaPago ?? "");
                record.SetString(15, f.MetodoPago ?? "");
                record.SetDecimal(16, f.SubTotal);
                record.SetDecimal(17, f.Descuento);
                record.SetDecimal(18, f.Total);
                record.SetDecimal(19, subtotal_MXN);
                record.SetDecimal(20, descuento_MXN);
                record.SetDecimal(21, total_MXN);
                record.SetDecimal(22, f.Total);
                record.SetString(23, f.CondicionesDePago ?? "");
                record.SetDecimal(24, tipoCambio);
                record.SetString(25, f.Moneda ?? "");
                record.SetString(26, f.Receptor?.UsoCFDI ?? "");
                record.SetDecimal(27, ivaRetenido);
                record.SetDecimal(28, ivaTrasladado);
                record.SetDecimal(29, isrRetenido);
                record.SetDecimal(30, isrTrasladado);
                record.SetDecimal(31, iepsRetenido);
                record.SetDecimal(32, iepsTrasladado);
                record.SetInt64(33, fechaCobroPago);
                record.SetInt64(34, EpochHelper.CrearEpoch(DateTime.Now));
                record.SetInt64(35, epochCreacion);
                record.SetInt64(36, epochTimbrado);
                record.SetBoolean(37, expirada);
                record.SetBoolean(38, true);
                record.SetBoolean(39, true);

                facturaDto.Factura = record;

                if (f.TipoDeComprobante == "I" && f.Conceptos != null)
                {
                    foreach (var c in f.Conceptos)
                    {
                        decimal ivaRet = 0, ivaTras = 0, isrRet = 0, isrTras = 0, iepsRet = 0, iepsTras = 0;

                        if (c.Impuestos?.Retenciones != null)
                        {
                            foreach (var r in c.Impuestos.Retenciones)
                            {
                                switch (r.Impuesto)
                                {
                                    case "001": isrRet += r.Importe; break;
                                    case "002": ivaRet += r.Importe; break;
                                    case "003": iepsRet += r.Importe; break;
                                }
                            }
                        }

                        if (c.Impuestos?.Traslados != null)
                        {
                            foreach (var t in c.Impuestos.Traslados)
                            {
                                switch (t.Impuesto)
                                {
                                    case "001": isrTras += t.Importe; break;
                                    case "002": ivaTras += t.Importe; break;
                                    case "003": iepsTras += t.Importe; break;
                                }
                            }
                        }


                        var rec = new SqlDataRecord(metaConcepto);
                        rec.SetString(0, Guid.NewGuid().ToString());
                        rec.SetString(1, facturaId);
                        rec.SetString(2, c.ClaveProdServ);
                        rec.SetString(3, c.ClaveUnidad);
                        rec.SetDecimal(4, c.Cantidad);
                        rec.SetString(5, c.Descripcion ?? "");
                        rec.SetDecimal(6, c.ValorUnitario);
                        rec.SetDecimal(7, c.Importe);
                        rec.SetDecimal(8, c.Importe);
                        rec.SetDecimal(9,  ivaRet);
                        rec.SetDecimal(10, ivaTras);
                        rec.SetDecimal(11, isrRet);
                        rec.SetDecimal(12, isrTras);
                        rec.SetDecimal(13, iepsRet);
                        rec.SetDecimal(14, iepsTras);

                        facturaDto.Conceptos.Add(rec);
                    }
                }

                if (f.TipoDeComprobante == "P" && f.complementoPagos != null)
                {
                    int noPago = 1;

                    foreach (var p in f.complementoPagos.Pago)
                    {
                        var pagoDto = new DataRecordPagosYRelacionadosDTO
                        {
                            Relacionados = new()
                        };

                        var pagoId = Guid.NewGuid().ToString();

                        decimal ivaRetP = 0, ivaTrasP = 0, isrRetP = 0, isrTrasP = 0, iepsRetP = 0, iepsTrasP = 0;

                        if (p.ImpuestosP?.RetencionesP != null)
                        {
                            foreach (var r in p.ImpuestosP.RetencionesP)
                            {
                                switch (r.ImpuestoP)
                                {
                                    case "001": isrRetP += r.ImporteP; break;
                                    case "002": ivaRetP += r.ImporteP; break;
                                    case "003": iepsRetP += r.ImporteP; break;
                                }
                            }
                        }

                        if (p.ImpuestosP?.TrasladosP != null)
                        {
                            foreach (var t in p.ImpuestosP.TrasladosP)
                            {
                                switch (t.ImpuestoP)
                                {
                                    case "001": isrTrasP += t.ImporteP; break;
                                    case "002": ivaTrasP += t.ImporteP; break;
                                    case "003": iepsTrasP += t.ImporteP; break;
                                }
                            }
                        }


                        var pagoRec = new SqlDataRecord(metaPago);
                        pagoRec.SetString(0, pagoId);
                        pagoRec.SetString(1, facturaId);
                        pagoRec.SetString(2, p.FormaDePagoP ?? "");
                        pagoRec.SetInt32(3, noPago++);
                        pagoRec.SetString(4, p.NumOperacion ?? "");
                        pagoRec.SetDecimal(5, p.Monto);
                        pagoRec.SetDecimal(6, ivaTrasP);
                        pagoRec.SetDecimal(7, ivaRetP);
                        pagoRec.SetDecimal(8, isrTrasP);
                        pagoRec.SetDecimal(9, isrRetP);
                        pagoRec.SetDecimal(10, iepsTrasP);
                        pagoRec.SetDecimal(11, iepsRetP);

                        pagoDto.Pago = pagoRec;

                        foreach (var d in p.DoctoRelacionado)
                        {
                            decimal ivaRetDR = 0, ivaTrasDR = 0, isrRetDR = 0, isrTrasDR = 0, iepsRetDR = 0, iepsTrasDR = 0;

                            if (d.ImpuestosDR?.RetencionesDR != null)
                            {
                                foreach (var r in d.ImpuestosDR.RetencionesDR)
                                {
                                    switch (r.ImpuestoDR)
                                    {
                                        case "001": isrRetDR += r.ImporteDR; break;
                                        case "002": ivaRetDR += r.ImporteDR; break;
                                        case "003": iepsRetDR += r.ImporteDR; break;
                                    }
                                }
                            }

                            if (d.ImpuestosDR?.TrasladosDR != null)
                            {
                                foreach (var t in d.ImpuestosDR.TrasladosDR)
                                {
                                    switch (t.ImpuestoDR)
                                    {
                                        case "001": isrTrasDR += t.ImporteDR; break;
                                        case "002": ivaTrasDR += t.ImporteDR; break;
                                        case "003": iepsTrasDR += t.ImporteDR; break;
                                    }
                                }
                            }

                            var rel = new SqlDataRecord(metaRelacionado);
                            rel.SetString(0, Guid.NewGuid().ToString());
                            rel.SetString(1, pagoId);
                            rel.SetString(2, d.IdDocumento);
                            rel.SetString(3, p.FormaDePagoP ?? "");
                            rel.SetInt32(4, int.TryParse(d.NumParcialidad, out var n) ? n : 1);
                            rel.SetDecimal(5, d.ImpSaldoInsoluto);
                            rel.SetDecimal(6, d.ImpSaldoAnt);
                            rel.SetDecimal(7, d.ImpPagado);
                            rel.SetDecimal(8, ivaTrasDR);
                            rel.SetDecimal(9, ivaRetDR);
                            rel.SetDecimal(10, isrTrasDR);
                            rel.SetDecimal(11, isrRetDR);
                            rel.SetDecimal(12, iepsTrasDR);
                            rel.SetDecimal(13, iepsRetDR);

                            pagoDto.Relacionados.Add(rel);
                        }

                        facturaDto.PagosYDtosRelacionados.Add(pagoDto);
                    }
                }

                facturasDTO.Add(facturaDto);
            }

            return facturasDTO;
        }

        public List<FacturaDataRecordOrderDTO> GetSqlDataRecords3_3(List<Comprobante3_3> cfdis)
        {
            var metaFactura = ObtenerMetaFactura();
            var metaConcepto = ObtenerMetaConcepto();
            var metaPago = ObtenerMetaPago();
            var metaRelacionado = ObtenerMetaRelacionado();

            List<FacturaDataRecordOrderDTO> facturasDTO = new();

            foreach (var f in cfdis)
            {
                var facturaDto = new FacturaDataRecordOrderDTO
                {
                    Conceptos = new(),
                    PagosYDtosRelacionados = new()
                };

                var record = new SqlDataRecord(metaFactura);
                string facturaId = Guid.NewGuid().ToString();

                decimal tipoCambio = f.Moneda != null && !f.Moneda.Equals("MXN", StringComparison.OrdinalIgnoreCase) && f.TipoCambio > 0 ? f.TipoCambio : 1M;
                decimal subtotal_MXN = f.SubTotal * tipoCambio;
                decimal descuento_MXN = f.Descuento * tipoCambio;
                decimal total_MXN = f.Total * tipoCambio;

                DateTime fechaCreacion = DateTime.TryParse(f.Fecha, out var fc) ? fc : DateTime.Now;
                long epochCreacion = EpochHelper.CrearEpoch(fechaCreacion);
                long epochTimbrado = EpochHelper.CrearEpoch(f.TimbreFiscalDigital?.FechaTimbrado ?? fechaCreacion);

                long fechaCobroPago = 0;
                if (f.MetodoPago == "PPD" && !string.IsNullOrWhiteSpace(f.CondicionesDePago))
                {
                    var match = Regex.Match(f.CondicionesDePago, @"\d+");
                    if (match.Success && int.TryParse(match.Value, out int dias))
                        fechaCobroPago = EpochHelper.CrearEpoch(fechaCreacion.AddDays(dias));
                }

                bool expirada = fechaCobroPago > 0 && new DateTime(fechaCobroPago) < DateTime.Now;

                decimal ivaRetenido = 0, ivaTrasladado = 0;
                decimal isrRetenido = 0, isrTrasladado = 0;
                decimal iepsRetenido = 0, iepsTrasladado = 0;

                if (f.Impuestos != null)
                {
                    if (f.Impuestos.Retenciones != null)
                    {
                        foreach (var r in f.Impuestos.Retenciones)
                        {
                            switch (r.Impuesto)
                            {
                                case "001": isrRetenido += r.Importe; break;
                                case "002": ivaRetenido += r.Importe; break;
                                case "003": iepsRetenido += r.Importe; break;
                            }
                        }
                    }

                    if (f.Impuestos.Traslados != null)
                    {
                        foreach (var t in f.Impuestos.Traslados)
                        {
                            switch (t.Impuesto)
                            {
                                case "001": isrTrasladado += t.Importe; break;
                                case "002": ivaTrasladado += t.Importe; break;
                                case "003": iepsTrasladado += t.Importe; break;
                            }
                        }
                    }
                }

                record.SetString(0, facturaId);
                record.SetDouble(1, float.Parse(f.Version));
                record.SetBoolean(2, emitida);
                record.SetString(3, f.TimbreFiscalDigital?.UUID ?? "");
                record.SetString(4, f.Folio ?? "");
                record.SetString(5, f.Serie ?? "");
                record.SetString(6, f.Emisor?.Rfc ?? "");
                record.SetString(7, f.Emisor?.Nombre ?? "");
                record.SetString(8, f.Emisor?.RegimenFiscal ?? "");
                record.SetString(9, f.Receptor?.Rfc ?? "");
                record.SetString(10, f.Receptor?.Nombre ?? "");
                record.SetString(11, "000");
                record.SetString(12, f.LugarExpedicion ?? "");
                record.SetString(13, f.TipoDeComprobante ?? "");
                record.SetString(14, f.FormaPago ?? "");
                record.SetString(15, f.MetodoPago ?? "");
                record.SetDecimal(16, f.SubTotal);
                record.SetDecimal(17, f.Descuento);
                record.SetDecimal(18, f.Total);
                record.SetDecimal(19, subtotal_MXN);
                record.SetDecimal(20, descuento_MXN);
                record.SetDecimal(21, total_MXN);
                record.SetDecimal(22, f.Total);
                record.SetString(23, f.CondicionesDePago ?? "");
                record.SetDecimal(24, tipoCambio);
                record.SetString(25, f.Moneda ?? "");
                record.SetString(26, f.Receptor?.UsoCFDI ?? "");
                record.SetDecimal(27, ivaRetenido);
                record.SetDecimal(28, ivaTrasladado);
                record.SetDecimal(29, isrRetenido);
                record.SetDecimal(30, isrTrasladado);
                record.SetDecimal(31, iepsRetenido);
                record.SetDecimal(32, iepsTrasladado);
                record.SetInt64(33, fechaCobroPago);
                record.SetInt64(34, EpochHelper.CrearEpoch(DateTime.Now));
                record.SetInt64(35, epochCreacion);
                record.SetInt64(36, epochTimbrado);
                record.SetBoolean(37, expirada);
                record.SetBoolean(38, true);
                record.SetBoolean(39, true);

                facturaDto.Factura = record;

                if (f.TipoDeComprobante == "I" && f.Conceptos != null)
                {
                    foreach (var c in f.Conceptos)
                    {
                        decimal ivaRet = 0, ivaTras = 0, isrRet = 0, isrTras = 0, iepsRet = 0, iepsTras = 0;

                        if (c.Impuestos?.Retenciones != null)
                        {
                            foreach (var r in c.Impuestos.Retenciones)
                            {
                                switch (r.Impuesto)
                                {
                                    case "001": isrRet += r.Importe; break;
                                    case "002": ivaRet += r.Importe; break;
                                    case "003": iepsRet += r.Importe; break;
                                }
                            }
                        }

                        if (c.Impuestos?.Traslados != null)
                        {
                            foreach (var t in c.Impuestos.Traslados)
                            {
                                switch (t.Impuesto)
                                {
                                    case "001": isrTras += t.Importe; break;
                                    case "002": ivaTras += t.Importe; break;
                                    case "003": iepsTras += t.Importe; break;
                                }
                            }
                        }

                        var rec = new SqlDataRecord(metaConcepto);
                        rec.SetString(0, Guid.NewGuid().ToString());
                        rec.SetString(1, facturaId);
                        rec.SetString(2, c.ClaveProdServ);
                        rec.SetString(3, c.ClaveUnidad);
                        rec.SetDecimal(4, c.Cantidad);
                        rec.SetString(5, c.Descripcion ?? "");
                        rec.SetDecimal(6, c.ValorUnitario);
                        rec.SetDecimal(7, c.Importe);
                        rec.SetDecimal(8, c.Importe);
                        rec.SetDecimal(9, ivaRet);
                        rec.SetDecimal(10, ivaTras);
                        rec.SetDecimal(11, isrRet);
                        rec.SetDecimal(12, isrTras);
                        rec.SetDecimal(13, iepsRet);
                        rec.SetDecimal(14, iepsTras);

                        facturaDto.Conceptos.Add(rec);
                    }
                }

                if (f.TipoDeComprobante == "P" && f.complementoPagos != null)
                {
                    int noPago = 1;

                    foreach (var p in f.complementoPagos.Pago)
                    {
                        var pagoDto = new DataRecordPagosYRelacionadosDTO { Relacionados = new() };
                        var pagoId = Guid.NewGuid().ToString();

                        decimal ivaRetP = 0, ivaTrasP = 0, isrRetP = 0, isrTrasP = 0, iepsRetP = 0, iepsTrasP = 0;

                        if (p.Impuestos != null)
                        {
                            foreach (var imp in p.Impuestos)
                            {
                                if (imp.Retenciones != null)
                                {
                                    foreach (var r in imp.Retenciones)
                                    {
                                        switch (r.Impuesto)
                                        {
                                            case "001": isrRetP += r.Importe; break;
                                            case "002": ivaRetP += r.Importe; break;
                                            case "003": iepsRetP += r.Importe; break;
                                        }
                                    }
                                }

                                if (imp.Traslados != null)
                                {
                                    foreach (var t in imp.Traslados)
                                    {
                                        switch (t.Impuesto)
                                        {
                                            case "001": isrTrasP += t.Importe; break;
                                            case "002": ivaTrasP += t.Importe; break;
                                            case "003": iepsTrasP += t.Importe; break;
                                        }
                                    }
                                }
                            }
                        }

                        var rec = new SqlDataRecord(metaPago);
                        rec.SetString(0, pagoId);
                        rec.SetString(1, facturaId);
                        rec.SetString(2, p.FormaDePagoP ?? "");
                        rec.SetInt32(3, noPago++);
                        rec.SetString(4, p.NumOperacion ?? "");
                        rec.SetDecimal(5, p.Monto);
                        rec.SetDecimal(6, ivaTrasP);
                        rec.SetDecimal(7, ivaRetP);
                        rec.SetDecimal(8, isrTrasP);
                        rec.SetDecimal(9, isrRetP);
                        rec.SetDecimal(10, iepsTrasP);
                        rec.SetDecimal(11, iepsRetP);

                        pagoDto.Pago = rec;

                        foreach (var d in p.DoctoRelacionado ?? Array.Empty<PagosPagoDoctoRelacionado1_0>())
                        {
                            var rel = new SqlDataRecord(metaRelacionado);
                            rel.SetString(0, Guid.NewGuid().ToString());
                            rel.SetString(1, pagoId);
                            rel.SetString(2, d.IdDocumento);
                            rel.SetString(3, p.FormaDePagoP ?? "");
                            rel.SetInt32(4, int.TryParse(d.NumParcialidad, out var n) ? n : 1);
                            rel.SetDecimal(5, d.ImpSaldoInsoluto);
                            rel.SetDecimal(6, d.ImpSaldoAnt);
                            rel.SetDecimal(7, d.ImpPagado);
                            rel.SetDecimal(8, 0);
                            rel.SetDecimal(9, 0);
                            rel.SetDecimal(10, 0);
                            rel.SetDecimal(11, 0);
                            rel.SetDecimal(12, 0);
                            rel.SetDecimal(13, 0);

                            pagoDto.Relacionados.Add(rel);
                        }

                        facturaDto.PagosYDtosRelacionados.Add(pagoDto);
                    }
                }

                facturasDTO.Add(facturaDto);
            }

            return facturasDTO;
        }


        private SqlMetaData[] ObtenerMetaFactura()
        {
            return new[]
            {
                new SqlMetaData("ID", SqlDbType.NVarChar, 36),
                new SqlMetaData("Version", SqlDbType.Float),
                new SqlMetaData("Emitida", SqlDbType.Bit),
                new SqlMetaData("UUID", SqlDbType.NVarChar, 36),
                new SqlMetaData("Folio", SqlDbType.NVarChar, 255),
                new SqlMetaData("Serie", SqlDbType.NVarChar, 255),
                new SqlMetaData("RFC_Emisor", SqlDbType.NVarChar, 13),
                new SqlMetaData("Nombre_Emisor", SqlDbType.NVarChar, 255),
                new SqlMetaData("RegimenEmisor", SqlDbType.NVarChar, 3),
                new SqlMetaData("RFC_Receptor", SqlDbType.NVarChar, 13),
                new SqlMetaData("Nombre_Receptor", SqlDbType.NVarChar, 255),
                new SqlMetaData("RegimenReceptor", SqlDbType.NVarChar, 3),
                new SqlMetaData("CP_Expedicion", SqlDbType.NVarChar, 5),
                new SqlMetaData("TipoComprobante", SqlDbType.NVarChar, 1),
                new SqlMetaData("FormaPago", SqlDbType.NVarChar, 2),
                new SqlMetaData("MetodoPago", SqlDbType.NVarChar, 3),
                new SqlMetaData("SubTotal", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("Descuento", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("Total", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("subtotal_MXN", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("descuento_MXN", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("total_MXN", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("MontoPendiente", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("Condiciones", SqlDbType.NVarChar, 255),
                new SqlMetaData("TipoCambio", SqlDbType.Decimal, 18, 6),
                new SqlMetaData("Moneda", SqlDbType.NVarChar, 10),
                new SqlMetaData("UsoCFDI", SqlDbType.NVarChar, 4),
                new SqlMetaData("IVA_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("FechaCobroPago", SqlDbType.BigInt),
                new SqlMetaData("FechaCargado", SqlDbType.BigInt),
                new SqlMetaData("FechaCreacion", SqlDbType.BigInt),
                new SqlMetaData("FechaTimbrado", SqlDbType.BigInt),
                new SqlMetaData("Expirada", SqlDbType.Bit),
                new SqlMetaData("Timbrada", SqlDbType.Bit),
                new SqlMetaData("Estatus", SqlDbType.Bit)
            };
        }

        private SqlMetaData[] ObtenerMetaConcepto()
        {
            return new[]
            {
                new SqlMetaData("ID", SqlDbType.NVarChar, 36),
                new SqlMetaData("FacturaID", SqlDbType.NVarChar, 36),
                new SqlMetaData("ClaveProductoServicio", SqlDbType.NVarChar, 8),
                new SqlMetaData("ClaveUnidad", SqlDbType.NVarChar, 3),
                new SqlMetaData("Cantidad", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("Descripcion", SqlDbType.NVarChar, -1),
                new SqlMetaData("ValorUnitario", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("Importe", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ImporteFinal", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Trasladado", SqlDbType.Decimal, 18, 2)
            };
        }

        private SqlMetaData[] ObtenerMetaPago()
        {
            return new[]
            {
                new SqlMetaData("ID", SqlDbType.NVarChar, 36),
                new SqlMetaData("FacturaPagoID", SqlDbType.NVarChar, 36),
                new SqlMetaData("FormaPago", SqlDbType.NVarChar, 2),
                new SqlMetaData("NoPago", SqlDbType.Int),
                new SqlMetaData("NumOperacion", SqlDbType.NVarChar, 100),
                new SqlMetaData("MontoTotal", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Trasladado", SqlDbType.Decimal, 18, 2)
            };
        }

        private SqlMetaData[] ObtenerMetaRelacionado()
        {
            return new[]
            {
                new SqlMetaData("ID", SqlDbType.NVarChar, 36),
                new SqlMetaData("PagoID", SqlDbType.NVarChar, 36),
                new SqlMetaData("FacturaPPD_ID", SqlDbType.NVarChar, 36),
                new SqlMetaData("FormaPago", SqlDbType.NVarChar, 2),
                new SqlMetaData("NumParcialidad", SqlDbType.Int),
                new SqlMetaData("ImpSaldoInsoluto", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ImpSaldoAnt", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ImpPagado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IVA_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("ISR_Trasladado", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Retenido", SqlDbType.Decimal, 18, 2),
                new SqlMetaData("IEPS_Trasladado", SqlDbType.Decimal, 18, 2)
            };
        }

    }

}
