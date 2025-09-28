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
        public async Task<DataSet> ValidarCorreo()
        {
            sentencia = "select * from Usuario where correo = @correo";
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
                    //Si surge una excepción, se devolverá un estadoLogin de Error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return null;
                }
            }
        }
        public async Task<int> Creacion()
        {
            this.idUsuario = await GenerarId();
            //Instrucción SQL
            sentencia = "insert into Usuario (idUsuario, nombre, apellidoPaterno, apellidoMaterno, correo, contrasenia, telefono, fechaNac, sexo, rol, fechaCreacion, ultimoInicioSesion) " +
                "values (@idUsuario, @nombre, @apellidoPaterno, @apellidoMaterno, @correo, @contrasenia, @telefono, @fechaNac, @sexo, @rol, @fechaCreacion, @ultimoInicioSesion)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@nombre", this.nombre);
                comando.Parameters.AddWithValue("@apellidoPaterno", this.apellidoPaterno);
                comando.Parameters.AddWithValue("@apellidoMaterno", this.apellidoMaterno);
                comando.Parameters.AddWithValue("@correo", this.correo);
                comando.Parameters.AddWithValue("@contrasenia", this.contrasenia);
                comando.Parameters.AddWithValue("@telefono", this.telefono);
                comando.Parameters.AddWithValue("@fechaNac", this.fechaNac);
                comando.Parameters.AddWithValue("@sexo", this.sexo);
                comando.Parameters.AddWithValue("@rol", this.rol);
                comando.Parameters.AddWithValue("@fechaCreacion", this.fechaCreacion);
                comando.Parameters.AddWithValue("@ultimoInicioSesion", this.ultimoInicioSesion);

                try
                {
                    //Si la conexión con la BD está cerrada, se abre
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    //Se ejecuta la instrucción SQL para validar si el correo y contraseña son correctos
                    
                    return comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, se devolverá un estadoLogin de Error
                    //await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }
        public async Task<int> GenerarId()
        {
            sentencia = "select max(idUsuario) + 1 as id FROM Usuario";
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
            }
            try
            {
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    conexion.Open();
                }
                DataSet datos = new DataSet();
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                adaptador.Fill(datos, "Tabla");
                if (datos.Tables["Tabla"].Rows.Count > 0)
                {
                    return Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["id"].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                //Si surge una excepción, se devolverá un estadoLogin de Error
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                return -1;
            }
        }
    }
}
