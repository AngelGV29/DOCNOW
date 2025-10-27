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
    //Clase utilizada para almacenar la información de los consultorios
    public class Consultorio
    {
        public int idConsultorio { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
    }

    //Clase utilizada para almacenar la información de las especialidades
    public class Especialidad
    {
        public int idEspecialidad { get; set; }
        public string especialidad { get; set; }
    }
    class NuevoMedicoSQL
    {
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private int idUsuario;
        private string numCedula;
        private string especialidad;

        //Constructor vacío utilizado para que el objeto pueda utilizar los métodos que devuelven los consultorios y especialidades disponibles
        public NuevoMedicoSQL()
        {
            this.idUsuario = 0;
            this.numCedula = "";
            this.especialidad = "";
        }
        //Constructor utilizado para crear el médico
        public NuevoMedicoSQL(string numCedula, string especialidad)
        {
            this.idUsuario = Globales.AdministradorDeSesion.idUsuario;
            this.numCedula = numCedula.Trim();
            this.especialidad = especialidad;
        }


        //Método que devuelve las especialidades disponibles
        public async Task<List<Especialidad>> ObtenerEspecialidades()
        {
            sentencia = "select * from EspecialidadMedico";
            List<Especialidad> EspecialidadesDisponibles = new List<Especialidad>();
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
                            //Crea instancias de la clase Especialidad y las almacena en una lista que es retornada
                            EspecialidadesDisponibles.Add(new Especialidad
                            {
                                idEspecialidad = lector.GetInt32(0),
                                especialidad = lector.IsDBNull(1) ? "" : lector.GetString(1),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return EspecialidadesDisponibles;
            }
        }

        //Método que devuelve los consultorios disponibles
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
                            //Crea instancias de la clase Consultorio y las almacena en una lista que es retornada
                            consultoriosDisponibles.Add(new Consultorio
                            {
                                idConsultorio = lector.GetInt32(0),
                                nombre = lector.IsDBNull(1) ? "" : lector.GetString(1),
                                telefono = lector.IsDBNull(2) ? "" : lector.GetString(2),
                                //Dirección es un campo compuesto por calle, número interior, número exterior, colonia y código postal
                                direccion = lector.IsDBNull(3) ? "" : lector.GetString(3) + ", " +
                                            (lector.IsDBNull(4) ? "" : lector.GetString(4) + ", #") +
                                            (lector.IsDBNull(5) ? "" : lector.GetString(5) + ", ") +
                                            (lector.IsDBNull(6) ? "" : lector.GetString(6) + ", C.P. ") +
                                            (lector.IsDBNull(7) ? "" : lector.GetString(7))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return consultoriosDisponibles;
            }
        }

        //Método que crea el médico en la base de datos
        public async Task<int> Creacion()
        {
            //Instrucción SQL
            sentencia = "insert into Medico (idUsuario, numCedula, idEspecialidad) " +
                "values (@idUsuario, @numCedula, (select idEspecialidad from EspecialidadMedico where nombreEspecialidad = @especialidad))";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {

                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@numCedula", this.numCedula);
                comando.Parameters.AddWithValue("@especialidad", this.especialidad);

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

        //Método que le asigna el rol de médico al usuario
        public async Task<int> AsignacionRol()
        {
            //Instrucción SQL
            sentencia = "update Usuario set rol=@rol where idUsuario=@idUsuario";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {

                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@rol", "MEDICO");

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

        //Método que devuelve el ID del médico recien creado
        public async Task<DataSet> ObtenerIdMedico()
        {
            //Instrucción SQL
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
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return new DataSet();
                }
            }
        }

        //Método que crea registros en la tabla MedicoConsultorio
        public async Task<int> VinculacionMedicoConsultorio(int idConsultorio)
        {
            //Instrucción SQL
            sentencia = "insert into MedicoConsultorio (idMedico, idConsultorio) " +
                "values (@idMedico, @idConsultorio)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", Globales.AdministradorDeSesion.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", idConsultorio);
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
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                    return -1;
                }
                
            }
        }
    }
}
