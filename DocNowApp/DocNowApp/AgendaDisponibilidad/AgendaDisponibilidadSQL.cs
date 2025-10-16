using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendaDisponibilidad
{
    internal class AgendaDisponibilidadSQL
    {
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private int idMedico;
        private int idConsultorio;
        private int idMedicoConsultorio;
        private bool trabajaLunes;
        private bool trabajaMartes;
        private bool trabajaMiercoles;
        private bool trabajaJueves;
        private bool trabajaViernes;
        private bool trabajaSabado;
        private bool trabajaDomingo;
        private TimeSpan horaInicioJornada;
        private TimeSpan horaFinJornada;
        private TimeSpan horaInicioDescanso;
        private TimeSpan horaFinDescanso;
        private int duracionSlot;
        private string estado;

        //Constructor vacío utilizado para que el objeto pueda utilizar los métodos que devuelven los consultorios y especialidades disponibles
        /*public NuevoMedicoSQL()
        {
            this.idUsuario = 0;
            this.numCedula = "";
            this.especialidad = "";
        }*/
        //Constructor utilizado para crear el médico
        public AgendaDisponibilidadSQL(bool trabajaLunes, bool trabajaMartes, bool trabajaMiercoles, bool trabajaJueves, bool trabajaViernes,
            bool trabajaSabado, bool trabajaDomingo, TimeSpan horaInicioJornada, TimeSpan horaFinJornada, TimeSpan horaInicioDescanso,
            TimeSpan horaFinDescanso, int duracionSLot, string estado)
        {
            this.trabajaLunes = trabajaLunes;
            this.trabajaMartes = trabajaMartes;
            this.trabajaMiercoles = trabajaMiercoles;
            this.trabajaJueves = trabajaJueves;
            this.trabajaViernes = trabajaViernes;
            this.trabajaSabado = trabajaSabado;
            this.trabajaDomingo = trabajaDomingo;
            this.horaInicioJornada = horaInicioJornada;
            this.horaFinJornada = horaFinJornada;
            this.horaInicioDescanso = horaInicioDescanso;
            this.horaFinDescanso = horaFinDescanso;
            this.duracionSlot = duracionSLot;
            this.estado = estado.Trim();
        }

        public async Task<DataSet> ValidarExistenciaAgenda()
        {
            //Instrucción SQL
            sentencia = "select * from AgendaDisponibilidad where idMedicoConsultorio = @idMedicoConsultorio";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedicoConsultorio", this.idMedicoConsultorio);
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

        public async Task<int> Creacion()
        {
            //Instrucción SQL
            sentencia = "insert into AgendaDisponibilidad (idMedicoConsultorio, trabajaLunes, trabajaMartes, trabajaMiercoles, trabajaJueves, " +
                "trabajaViernes, trabajaSabado, trabajaDomingo, horaInicioJornada, horaFinJornada, horaInicioDescanso, horaFinDescanso, duracionSlot, estado) " +
                "values (@idMedicoConsultorio, @trabajaLunes, @trabajaMartes, @trabajaMiercoles, @trabajaJueves, @trabajaViernes, @trabajaSabado, @trabajaDomingo, " +
                "@horaInicioJornada, @horaFinJornada, @horaInicioDescanso, @horaFinDescanso, @duracionSlot, @estado)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedicoConsultorio", this.idMedicoConsultorio);
                comando.Parameters.AddWithValue("@trabajaLunes", this.trabajaLunes);
                comando.Parameters.AddWithValue("@trabajaMartes", this.trabajaMartes);
                comando.Parameters.AddWithValue("@trabajaMiercoles", this.trabajaMiercoles);
                comando.Parameters.AddWithValue("@trabajaJueves", this.trabajaJueves);
                comando.Parameters.AddWithValue("@trabajaViernes", this.trabajaViernes);
                comando.Parameters.AddWithValue("@trabajaSabado", this.trabajaSabado);
                comando.Parameters.AddWithValue("@trabajaDomingo", this.trabajaDomingo);
                comando.Parameters.AddWithValue("@horaInicioJornada", this.horaInicioJornada);
                comando.Parameters.AddWithValue("@horaFinJornada", this.horaFinJornada);
                comando.Parameters.AddWithValue("@horaInicioDescanso", this.horaInicioDescanso);
                comando.Parameters.AddWithValue("@horaFinDescanso", this.horaFinDescanso);
                comando.Parameters.AddWithValue("@duracionSlot", this.duracionSlot);
                comando.Parameters.AddWithValue("@estado", this.estado);

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
