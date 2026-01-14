using Microsoft.Extensions.Configuration;

#pragma warning disable CS8600
#pragma warning disable CS8604

namespace BULK_DOWNLOAD_CFDI.ToolsAndExtensions
{
    public static class ConsoleTools
    {
        public static List<string> _logs = new List<string>();

        private static string _lines = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" +
                                       " " +
                                       "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -";

        public static bool CreateFile(string ruta)
        {
            bool flag = false;
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
                flag = true;
            }

            return flag;
        }

        public static void MoveFile(string origen, string destino)
        {
            if (File.Exists(origen))
            {
                // Crea la carpeta destino si no existe
                string directorioDestino = Path.GetDirectoryName(destino);
                if (!Directory.Exists(directorioDestino))
                    Directory.CreateDirectory(directorioDestino);

                File.Move(origen, destino);
                Console.WriteLine($"Se movio el archivo en la ruta {origen} a erroneos.");
            }
            else
            {
                Console.WriteLine($"El archivo en la ruta: {origen} no existe.");
            }
        }

        public static void RemoveFile(string route)
            => File.Delete(route);


        public static void CreateLog(IConfiguration configuration)
        {
            string rutaLogs = configuration["RutaLogs"];
            string dateFormat = configuration["dataCode:dateFormat"];

            var fechaHoy = DateTime.Now.ToString(dateFormat);
            var carpetaRuta = Path.Combine(rutaLogs, fechaHoy);

            CreateFile(carpetaRuta);

            string nombreArchivo = $"log_{DateTime.Now:HHmmss}.log";
            string rutaArchivo = Path.Combine(carpetaRuta, nombreArchivo);

            using (StreamWriter sw = new StreamWriter(rutaArchivo, append: false))
            {
                int contadorLineas = 0;

                for (int i = 0; i < _logs.Count; i++)
                {
                    string lineaActual = _logs[i];
                    sw.WriteLine(lineaActual);
                    contadorLineas++;

                    bool esAdjunto = lineaActual.Contains("Archivo adjunto");
                    bool siguienteEsAdjunto = i + 1 < _logs.Count && _logs[i + 1].Contains("Archivo adjunto");

                    if (esAdjunto && siguienteEsAdjunto)
                    {
                        // Si está dentro de un bloque de adjuntos, no hagas nada
                        continue;
                    }
                    else if (esAdjunto && !siguienteEsAdjunto)
                    {
                        // Si termina el bloque de adjuntos, agrega salto y reinicia contador
                        sw.WriteLine();
                        contadorLineas = 0;
                        continue;
                    }

                    // Si no es adjunto y se llegó a 5 líneas normales, agregar salto
                    if (contadorLineas == 5)
                    {
                        sw.WriteLine();
                        contadorLineas = 0;
                    }
                }
            }

            Console.WriteLine($"Log guardado en: {rutaArchivo}");
        }



        public static void ShowDataConsole(string[] messages, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            // - - - - - - - - - - - - - - - - - - - - - - - - - 
            CreateLinesWithLineBreak();
            foreach (var message in messages)
            {
                Console.WriteLine(message);
                _logs.Add($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] {message}");
            }
            CreateLines();
            // - - - - - - - - - - - - - - - - - - - - - - - - - 
            Console.ResetColor();
        }

        public static void CreateLines()
            => Console.WriteLine(_lines);

        public static void MessageWithLineBreak(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            // - - - - - - - - - - - - - - - - - - - - - - - - - 
            Console.WriteLine($"\n{message}");
            _logs.Add($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] {message}");
            // - - - - - - - - - - - - - - - - - - - - - - - - - 
            Console.ResetColor();
        }

        public static void CreateLinesWithLineBreak()
            => Console.WriteLine($"\n{_lines}");


        public static async Task ShowSpinner(CancellationToken token)
        {
            var spinner = new[] { '|', '/', '-', '\\' };
            int i = 0;

            while (!token.IsCancellationRequested)
            {
                Console.Write(spinner[i++ % spinner.Length]);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                await Task.Delay(100, token).ContinueWith(_ => { }); // evita excepciones si se cancela
            }
        }

        public static async Task ShowPulsingBar(CancellationToken token)
        {
            int maxLength = 20;
            int step = 1;
            int currentLength = 0;
            bool increasing = true;

            var colors = new[]
            {
                ConsoleColor.DarkGray,
                ConsoleColor.Gray,
                ConsoleColor.White,
                ConsoleColor.Gray,
                ConsoleColor.DarkGray
            };
            int colorIndex = 0;

            while (!token.IsCancellationRequested)
            {
                string barra = new string('█', currentLength).PadRight(maxLength, ' ');

                Console.ForegroundColor = colors[colorIndex];
                Console.Write("\r");
                Console.Write($"[{barra}]");

                await Task.Delay(70, token).ContinueWith(_ => { });

                // Primero crecer o decrecer
                if (increasing)
                {
                    currentLength += step;
                    if (currentLength > maxLength) // OJO: ahora es ">"
                    {
                        currentLength = maxLength;
                        increasing = false;
                    }
                }
                else
                {
                    currentLength -= step;
                    if (currentLength < 0) // OJO: ahora es "<"
                    {
                        currentLength = 0;
                        increasing = true;
                    }
                }

                colorIndex = (colorIndex + 1) % colors.Length;
            }

            Console.ResetColor();
        }



    }
}
