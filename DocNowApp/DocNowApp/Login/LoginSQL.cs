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

        /*enum es utilizado para definir un conjunto de constantes con nombres bajo un
        mismo tipo. En este caso se utiliza para representar el conjunto de estados posibles
        para un inicio de sesión. Internamente, cada estado es répresentado con un número, que
        empieza en 0 y va aumentando de uno en uno (de manera similar a un arreglo con los índices)*/
        public enum estadoLogin
        {
            Exito,
            CredencialesIncorrectas,
            Error
        }

        /*async se coloca antes de la definición de un método
        para indicar que el método puede contener operaciones que se
        ejecutan de forma asíncrona, es decir, no bloquean el hilo principal
        mientras esperan que algo termine.
        
        await se utiliza dentro de métodos async,
        le indican al programa que debe esperar a que termine una tarea asíncrona
        antes de continuar, mientras espera, el hilo no queda bloqueado, lo que
        evita que la UI se congele*/

        /*Cuando se trata de un método asíncrono se utiliza Task<tipo_de_dato_a_retornar>
        a menos que sea un método void que no devuelve nada*/

        //Método con el que se valida que el correo y contrasenña introducidos son correctos
        public async Task<estadoLogin> Validacion()
        {
            //Instrucción SQL
            sentencia = $"select * from NuevoUsuario where correo = @correo and contrasenia = @contrasenia";

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
