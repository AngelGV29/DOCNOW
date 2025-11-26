using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocNowApp.Login.LoginSQL;

namespace DocNowApp.NuevoUsuario
{
    class NuevoUsuarioSQL
    {
        //Variables de conexión
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private int idUsuario;
        private string nombre;
        private string apellidoPaterno;
        private string apellidoMaterno;
        private string correo;
        private string contrasenia;
        private string telefono;
        private DateTime fechaNac;
        private char sexo;
        private string rol;
        private DateTime fechaCreacion;
        private DateTime ultimoInicioSesion;


        //Constructor
        public NuevoUsuarioSQL(string nombre, string apellidoPaterno, string apellidoMaterno, string correo, string telefono, string contrasenia,
            DateTime fechaNac, char sexo, string rol, DateTime fechaCreacion, DateTime ultimoInicioSesion)
        {
            this.nombre = nombre.Trim();
            this.apellidoPaterno = apellidoPaterno.Trim();
            this.apellidoMaterno = apellidoMaterno.Trim();
            this.correo = correo.Trim();
            this.telefono = telefono.Trim();
            this.contrasenia = contrasenia.Trim();
            this.fechaNac = fechaNac;
            this.sexo = sexo;
            this.rol = rol;
            this.fechaCreacion = fechaCreacion;
            this.ultimoInicioSesion = ultimoInicioSesion;

        }

        //Método que valida si el correo ya está registrado
        public async Task<DataSet> ValidarCorreo()
        {
            //Instrucción SQL
            sentencia = "select correo from Usuario where correo = @correo";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@correo", this.correo);
                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    DataSet datos = new DataSet();
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                    adaptador.Fill(datos, "Tabla");
                    return datos;
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return new DataSet();
                }
            }
        }

        //Método que devuelve la información del usuario para obtener su ID
        public async Task<DataSet> ObtenerIdUsuario()
        {
            //Instrucción SQL
            sentencia = "select idUsuario from Usuario where correo = @correo";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@correo", this.correo);
                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    DataSet datos = new DataSet();
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                    adaptador.Fill(datos, "Tabla");
                    return datos;
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return new DataSet();
                }
            }
        }

        //Método que crea el usuario en la base de datos
        public async Task<int> Creacion()
        {
            //Instrucción SQL
            sentencia = "insert into Usuario (nombre, apellPaterno, apellMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, fechaCreacion, ultimoLogin) " +
                "values (@nombre, @apellPaterno, @apellMaterno, @correo, @contrasenia, @telefono, @fechaNac, @sexo, @rol, @fechaCreacion, @ultimaModSesion)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@nombre", this.nombre);
                comando.Parameters.AddWithValue("@apellPaterno", this.apellidoPaterno);
                comando.Parameters.AddWithValue("@apellMaterno", this.apellidoMaterno);
                comando.Parameters.AddWithValue("@correo", this.correo);
                comando.Parameters.AddWithValue("@contrasenia", this.contrasenia);
                comando.Parameters.AddWithValue("@telefono", this.telefono);
                comando.Parameters.AddWithValue("@fechaNac", this.fechaNac);
                comando.Parameters.AddWithValue("@sexo", this.sexo);
                comando.Parameters.AddWithValue("@rol", this.rol);
                comando.Parameters.AddWithValue("@fechaCreacion", this.fechaCreacion);
                comando.Parameters.AddWithValue("@ultimaModSesion", this.ultimoInicioSesion);

                try
                {
                    //Si la conexión con la BD está cerrada, se abre
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    return comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }
    }
}
