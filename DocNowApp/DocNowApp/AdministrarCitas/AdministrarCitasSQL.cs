using DocNowApp.AgendarCita;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AdministrarCitas
{
    internal class AdministrarCitasSQL
    {
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private int idCita;
        private int idPaciente;
        private int idMedico;
        private int idConsultorio;
        private int idMotivo;
        private DateTime fechaCita;
        private int idDia;
        private TimeSpan horaInicio;
        private TimeSpan horaFin;
        private string estadoCita;
        private string notas;
        private DateTime fechaCreacion;
        private DateTime fechaModificacion;


        public AdministrarCitasSQL(CitaDto _citaDto)
        {
            idCita = _citaDto.IdCita;
            idPaciente = _citaDto.IdPaciente;
            idMedico = _citaDto.IdMedico;
            idConsultorio = _citaDto.IdConsultorio;
            idMotivo = _citaDto.IdMotivo;
            fechaCita = _citaDto.FechaCita;
            horaInicio = _citaDto.HoraInicio;
            horaFin = _citaDto.HoraFin;
            estadoCita = _citaDto.EstadoCita;
            notas = _citaDto.Notas;
        }

        public AdministrarCitasSQL(int idMedico)
        {
            this.idMedico = idMedico;
        }

        public AdministrarCitasSQL(int idMedico, int idConsultorio)
        {
            this.idMedico = idMedico;
            this.idConsultorio = idConsultorio;
        }

        public async Task<List<CitaDto>> ObtenerCitasAgendadas()
        {
            sentencia = "select c.idCita, c.idPaciente, c.idMedico, c.idConsultorio, c.idMotivo, c.fechaCita, c.horaInicio, c.horaFin, c.estadoCita, c.notas, " +
                "uPac.nombre + ' ' + uPac.apellPaterno + ' ' + uPac.apellMaterno as NombrePaciente, " +
                "p.alergia as AlergiasPaciente, " +
                "p.medicacion as MedicacionPaciente, " +
                "cons.nombre as NombreConsultorio, " +
                "mc.descripcion as DescripcionMotivo " +
                "from Cita c " +
                "inner join Paciente p on c.idPaciente = p.idPaciente " +
                "inner join Usuario uPac on p.idUsuario = uPac.idUsuario " +
                "inner join Consultorio cons on c.idConsultorio = cons.idConsultorio " +
                "inner join MotivoConsulta mc on c.idMotivo = mc.idMotivo " +
                "where c.idMedico = @idMedico " +
                "order by c.fechaCita asc, c.horaInicio";
            List<CitaDto> citasAgendadas = new List<CitaDto>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
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
                            //Crea instancias de la clase CitaDto y las almacena en una lista que es retornada
                            citasAgendadas.Add(new CitaDto
                            {
                                IdCita = lector.GetInt32(0),
                                IdPaciente = lector.GetInt32(1),
                                IdMedico = lector.GetInt32(2),
                                IdConsultorio = lector.GetInt32(3),
                                IdMotivo = lector.GetInt32(4),
                                FechaCita = lector.GetDateTime(5),
                                HoraInicio = lector.GetTimeSpan(6),
                                HoraFin = lector.GetTimeSpan(7),
                                EstadoCita = lector.GetString(8),
                                Notas = lector.IsDBNull(9) ? "Sin notas" : lector.GetString(9),
                                NombrePaciente = lector.GetString(10),
                                AlergiaPaciente = lector.IsDBNull(11) ? "Sin alergias registradas" : lector.GetString(10),
                                MedicacionPaciente = lector.GetString(12),
                                NombreConsultorio = lector.GetString(13),
                                DescripcionMotivo = lector.GetString(14),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return citasAgendadas;
            }
        }

        public async Task<List<string>> ObtenerConsultorios()
        {
            sentencia = "select con.nombre from Consultorio con inner join MedicoConsultorio mc" +
                " on con.idConsultorio = mc.idConsultorio where mc.idMedico = @idMedico";
            List<string> consultorios = new List<string>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
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
                            consultorios.Add(lector.GetString(0));
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return consultorios;
            }
        }

        public async Task<List<string>> ObtenerMotivosConsulta()
        {
            sentencia = "select descripcion from MotivoConsulta where idMedico = @idMedico";
            List<string> motivosConsulta = new List<string>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
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
                            motivosConsulta.Add(lector.GetString(0));

                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return motivosConsulta;
            }
        }

        public async Task<int> CancelarCita()
        {
            sentencia = "update Cita set estadoCita = @estadoCita, notas = @notas, " +
                "fechaModificacion = @fechaModificacion where idCita = @idCita";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("idCita", this.idCita);
                comando.Parameters.AddWithValue("@estadoCita", "CANCELADA");
                comando.Parameters.AddWithValue("@notas", this.notas);
                comando.Parameters.AddWithValue("@fechaModificacion", DateTime.Now);

                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    return comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Error al modificar: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }

        public async Task<int> CompletarCita()
        {
            sentencia = "update Cita set estadoCita = @estadoCita, notas = @notas, " +
                "fechaModificacion = @fechaModificacion where idCita = @idCita";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("idCita", this.idCita);
                comando.Parameters.AddWithValue("@estadoCita", "COMPLETADA");
                comando.Parameters.AddWithValue("@notas", this.notas);
                comando.Parameters.AddWithValue("@fechaModificacion", DateTime.Now);

                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    return comando.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Error al modificar: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }
    }
}
