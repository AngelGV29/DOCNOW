using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocNowApp.Globales;

namespace DocNowApp.Login
{
    class LoginSQL
    {
       //Variables de conexión
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private string correo;
        private string contrasenia;


        //Constructor
        public LoginSQL(string correo, string contrasenia)
        {
            this.correo = correo.Trim();
            this.contrasenia = contrasenia.Trim();
            
        }

        public enum estadoLogin
        {
            Exito, CredencialesIncorrectas, Error
        }

        //Método con el que se valida que el correo y contrasenña introducidos son correctos
        /*async se coloca antes de la definición de un método
        para indicar que el método puede contener operaciones que se
        ejecutan de forma asíncrona, es decir, no bloquean el hilo principal
        mientras esperan que algo termine.
        
        await se utiliza dentro de métodos async,
        le indican al programa que debe esperar a que termine una tera asíncrona
        antes de continuarm, mientras espera, el hilo no queda bloqueado, lo que
        evita que la UI se congele*/
        
        public async Task<estadoLogin> Validacion()
        {
            //Instrucción SQL
            sentencia = $"select * from usuario where correo = @correo and contrasenia = @contrasenia";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@correo", this.correo);
                comando.Parameters.AddWithValue("@contrasenia", this.contrasenia);

                try
                {
                    //Si la conexión con la BD está cerrada, se abre
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    //Se ejecuta la instrucción SQL para validar si el correo y contraseña son correctos
                    using (SqlDataReader validar = comando.ExecuteReader())
                    {
                        //Si el lector devuelve true el estadoLogin será Exito, si devuelve false será CredencialesIncorrectas
                        return validar.Read() ? estadoLogin.Exito:estadoLogin.CredencialesIncorrectas;
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, se devolverá un estadoLogin de Error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return estadoLogin.Error;
                }
            }
        }
    }
}
