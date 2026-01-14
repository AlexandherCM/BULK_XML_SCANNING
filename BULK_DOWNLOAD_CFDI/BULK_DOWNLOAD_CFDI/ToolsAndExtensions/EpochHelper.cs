using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BULK_DOWNLOAD_CFDI.ToolsAndExtensions
{
    public static class EpochHelper
    {
        public static long CrearEpoch(DateTime fecha)
        {
            DateTime fechaInicial = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            int epoch = (int)Math.Round((fecha.ToUniversalTime() - fechaInicial).TotalSeconds, 0);

            return epoch;
        }

        public static DateTime ObtenerFecha(long epoch)
        {
            DateTime fechaInicial = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            fechaInicial = fechaInicial.AddSeconds(epoch).ToLocalTime();

            return fechaInicial;
        }

        public static string ObtenerMesYAnio(DateTime fecha)
        {
            string nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fecha.Month);
            nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1);

            // Formatear la cadena "Mes del Año"
            string resultado = $"{nombreMes} del {fecha.Year}";
            return resultado;
        }
    }
}
