using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.Globales
{
    internal class CadenaConexion
    {
        static public string dbNombre;
        static public string server;
        static public string usuario;
        static public string contrasenia;
        static public string miConexion;

        // Constructor estático: se ejecuta una sola vez
        static CadenaConexion()
        {
            try
            {
                // Busca .env empezando en el directorio de la app y subiendo carpetas si hace falta
                DotNetEnv.Env.TraversePath().Load(); // recomendado para .NET apps de escritorio[web:75][web:106]
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error cargando .env: " + ex.Message);
            }

            dbNombre = Environment.GetEnvironmentVariable("DOCNOW_DB_NAME");
            server = Environment.GetEnvironmentVariable("DOCNOW_DB_SERVER");
            usuario = Environment.GetEnvironmentVariable("DOCNOW_DB_USER");
            contrasenia = Environment.GetEnvironmentVariable("DOCNOW_DB_PASSWORD");

            var integrated = Environment.GetEnvironmentVariable("DOCNOW_DB_INTEGRATED_SECURITY");

            string seguridad;
            if (integrated.Equals("True", StringComparison.OrdinalIgnoreCase))
                seguridad = "Integrated Security=True";
            else
                seguridad = $"User Id={usuario};Password={contrasenia}";

            miConexion = $"Data Source={server};Initial Catalog={dbNombre};{seguridad};Encrypt=True;TrustServerCertificate=True";

            /*VERSIÓN CORRECTA
            dbNombre = Environment.GetEnvironmentVariable("DOCNOW_DB_NAME") ?? "bdDocNow";
            server = Environment.GetEnvironmentVariable("DOCNOW_DB_SERVER") ?? @"ANGELGV29\SQLEXPRESS";
            usuario = Environment.GetEnvironmentVariable("DOCNOW_DB_USER") ?? "sa";
            contrasenia = Environment.GetEnvironmentVariable("DOCNOW_DB_PASSWORD") ?? "2909";

            var integrated = Environment.GetEnvironmentVariable("DOCNOW_DB_INTEGRATED_SECURITY") ?? "True";

            string seguridad;
            if (integrated.Equals("True", StringComparison.OrdinalIgnoreCase))
                seguridad = "Integrated Security=True";
            else
                seguridad = $"User Id={usuario};Password={contrasenia}";

            miConexion = $"Data Source={server};Initial Catalog={dbNombre};{seguridad};Encrypt=True;TrustServerCertificate=True";
            */
        }



        /*
        static public string dbNombre = "bdDocNow";
        static public string server = @"ANGELGV29\SQLEXPRESS";
        static public string contrasenia = "2909";
        static public string seguridad = "Integrated Security=True";
        static public string usuario = "sa";

        //Cadena de conexión a la base de datos
        static public string miConexion = $"Data Source={server};Initial Catalog={dbNombre};{seguridad};Encrypt=True;TrustServerCertificate=True";
        */
    }
}
