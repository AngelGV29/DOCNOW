using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.NuevoPaciente
{
    class NuevoPacienteSQL
    {
        //Variables de conexión
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        private int idUsuario;
        private string alergia;
        private string medicacion;
        private DateTime ultimaVisita;

        //Constructor
        public NuevoPacienteSQL(string alergia, string medicacion, DateTime ultimaVisita)
        {
            this.idUsuario = Globales.AdministradorDeSesion.idUsuario;
            this.alergia = alergia.Trim();
            this.medicacion = medicacion.Trim();
            this.ultimaVisita = ultimaVisita;
        }

        public async Task<int> Creacion()
        {
            //Instrucción SQL
            sentencia = "insert into Paciente (idUsuario, alergia, medicacion, ultimaVisita) " +
                "values (@idUsuario, @alergia, @medicacion, @ultimaVisita)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {

                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@alergia", this.alergia);
                comando.Parameters.AddWithValue("@medicacion", this.medicacion);
                comando.Parameters.AddWithValue("@ultimaVisita", this.ultimaVisita);

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
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }
        public async Task<DataSet> ObtenerIdPaciente()
        {
            sentencia = "select * from Paciente where idUsuario = @idUsuario";
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
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
                    return new DataSet();
                }
            }
        }
    }
}
