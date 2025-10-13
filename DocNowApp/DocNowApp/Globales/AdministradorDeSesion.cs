using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.Globales
{
    class AdministradorDeSesion
    {
        //Propiedades estáticas para almacenar la información de la sesión actual
        public static int idUsuario { get; set; }
        public static int idPaciente { get; set; }
        public static int idMedico { get; set; }
        public static int idAdmin { get; set; }
        public static string Rol { get; set; }
    }
}
