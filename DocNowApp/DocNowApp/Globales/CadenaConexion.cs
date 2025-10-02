using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.Globales
{
    internal class CadenaConexion
    {
        static public string dbNombre = "dbDocNow";
        static public string server = @"JANDOS_LAPTOP\SQLSERVER01";
        static public string contrasenia = "1234";
        static public string seguridad = "Integrated Security=True";
        static public string usuario = "sa";

        //Cadena de conexión a la base de datos
        static public string miConexion = $"Data Source={server};Initial Catalog={dbNombre};{seguridad};Encrypt=True;TrustServerCertificate=True";

    }
}
