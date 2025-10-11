using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.NuevoMedico
{
    public class Consultorio
    {
        public int idConsultorio { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
    }
    class NuevoMedicoSQL
    {
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        private int idUsuario;
        private string cedula;
        private string especialidad;

        public NuevoMedicoSQL()
        {
            this.idUsuario = 0;
            this.cedula = "";
            this.especialidad = "";
        }
        public NuevoMedicoSQL(string cedula, string especialidad)
        {
            this.idUsuario = Globales.AdministradorDeSesion.idUsuario;
            this.cedula = cedula.Trim();
            this.especialidad = especialidad.Trim();
        }

        public async Task<List<Consultorio>> ObtenerConsultorios()
        {
            sentencia = "select * from Consultorio";
            List<Consultorio> consultoriosDisponibles = new List<Consultorio>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    using (SqlDataReader lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            consultoriosDisponibles.Add(new Consultorio
                            {
                                idConsultorio = lector.GetInt32(0),
                                nombre = lector.GetString(1),
                                direccion = lector.GetString(2),
                                telefono = lector.GetString(3)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, se devolverá un estadoLogin de Error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    
                }
                return consultoriosDisponibles;
            }
        }

        public async Task<int> Creacion()
        {
            //Instrucción SQL
            sentencia = "insert into Medico (idUsuario, cedula, especialidad) " +
                "values (@idUsuario, @cedula, @especialidad)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {

                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@cedula", this.cedula);
                comando.Parameters.AddWithValue("@especialidad", this.especialidad);

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
        public async Task<DataSet> ObtenerIdMedico()
        {
            sentencia = "select * from Medico where idUsuario = @idUsuario";
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
        public async Task<int> VinculacionMedicoConsultorio(List<Consultorio> ConsultoriosSeleccionados)
        {
            //Instrucción SQL
            sentencia = "insert into MedicoConsultorio (idMedico, idConsultorio) " +
                "values (@idMedico, @idConsultorio";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                foreach (var consultorio in ConsultoriosSeleccionados)
                {
                    comando.Parameters.AddWithValue("@idMedico", Globales.AdministradorDeSesion.idMedico);
                    comando.Parameters.AddWithValue("@idConsultorio", consultorio);

                    try
                    {
                        //Si la conexión con la BD está cerrada, se abre
                        if (conexion.State != System.Data.ConnectionState.Open)
                        {
                            conexion.Open();
                        }
                        //Se ejecuta la instrucción SQL para validar si el correo y contraseña son correctos

                        comando.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //Si surge una excepción, se devolverá un estadoLogin de Error
                        await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                        return -1;
                    }
                }
                return 1;
                
            }
        }
    }
}
