using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.NuevoUsuario
{
    class NuevoUsuarioSQL
    {
        //Variables de conexión
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private string nombre;
        private string apellidoPaterno;
        private string apellidoMaterno;
        private string correo;
        private string contrasenia;
        private string telefono;
        private DateTime fechaNac;
        private string sexo;
        private string rol;
        private DateTime fechaCreacion;
        private DateTime ultimoInicioSesion;


        //Constructor
        public NuevoUsuarioSQL(string nombre, string apellidoPaterno, string apellidoMaterno, string correo, string contrasenia, string telefono,
            DateTime fechaNac, string sexo, string rol, DateTime fechaCreacion, DateTime ultimoInicioSesion)
        {
            this.nombre = nombre;
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
            this.correo = correo.Trim();
            this.contrasenia = contrasenia.Trim();
            this.telefono = telefono;
            this.fechaNac = fechaNac;
            this.sexo = sexo;
            this.rol = rol;
            this.fechaCreacion = fechaCreacion;
            this.ultimoInicioSesion = ultimoInicioSesion;

        }
    }
}
