using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.Globales
{
    internal class CadenaConexion
    {
        static public string dbNombre = "bdDocNow";
        static public string server = @"ANGELGV29\SQLEXPRESS";
        static public string contrasenia = "2909";
        static public string seguridad = "Integrated Security=True";
        static public string usuario = "sa";

        //Cadena de conexión a la base de datos
        static public string miConexion = $"Data Source={server};Initial Catalog={dbNombre};{seguridad};Encrypt=True;TrustServerCertificate=True";

    }
}
