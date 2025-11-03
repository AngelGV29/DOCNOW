using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace DocNowApp.AgendaDisponibilidad
{
    //Mediante esta clase se almacenan los datos de los consultorios
    public class Consultorio
    {
        public int idConsultorio { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
    }

    class AgendaDisponibilidadSQL
    {
        private string sentencia; //Recibirá la instrucción SQL
        private SqlConnection conexion; //Abre y cierrra la conexión
        private SqlCommand comando; //Es el comando SQL

        //Atributos
        private int idAgendaDisponibilidad;
        private int idMedico;
        private int idConsultorio;
        private int idDia;
        private TimeSpan horaInicioJornada;
        private TimeSpan horaFinJornada;
        public int duracionSlotMinutos;
        public bool agendaActiva;

        public AgendaDisponibilidadSQL(FranjaDto _franjaDto)
        {
            this.idAgendaDisponibilidad = _franjaDto.IdAgendaDisponibilidad;
            this.idMedico = _franjaDto.IdMedico;
            this.idConsultorio = _franjaDto.IdConsultorio;
            this.idDia = _franjaDto.IdDia;
            this.horaInicioJornada = _franjaDto.HoraInicioJornada;
            this.horaFinJornada = _franjaDto.HoraFinJornada;
            this.duracionSlotMinutos = _franjaDto.DuracionSlotMinutos;
            this.agendaActiva = _franjaDto.AgendaActiva;
        }

        public AgendaDisponibilidadSQL(int idMedico)
        {
            this.idMedico = idMedico;
        }

        public AgendaDisponibilidadSQL(int idMedico, int idConsultorio)
        {
            this.idMedico = idMedico;
            this.idConsultorio = idConsultorio;
        }

        public async Task<List<Consultorio>> ObtenerConsultorios()
        {
            sentencia = @"select c.idConsultorio, c.nombre, c.telefono, c.calle, c.numeroInterior, c.numeroExterior, c.colonia, c.codigoPostal from Consultorio c inner join MedicoConsultorio mc" +
                " on c.idConsultorio = mc.idConsultorio where mc.idMedico = @idMedico";
            List<Consultorio> consultorios = new List<Consultorio>();
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
                            //Crea instancias de la clase Consultorio y las almacena en una lista que es retornada
                            consultorios.Add(new Consultorio
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
                return consultorios;
            }
        }

        public async Task<List<FranjaDto>> ObtenerAgendaDisponibilidad()
        {
            sentencia = "select idAgendaDisponibilidad, idMedico, idConsultorio, idDia," +
                " horaInicioJornada, horaFinJornada, duracionSlotMinutos, agendaActiva from AgendaDisponibilidad" +
                " where idMedico = @idMedico and idConsultorio = @idConsultorio order by idDia, horaInicioJornada";
            List<FranjaDto> agendasDisponibilidad = new List<FranjaDto>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("idConsultorio", this.idConsultorio);
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
                            //Crea instancias de la clase FranjaDto y las almacena en una lista que es retornada
                            agendasDisponibilidad.Add(new FranjaDto
                            {
                                IdAgendaDisponibilidad = lector.GetInt32(0),
                                IdMedico = lector.GetInt32(1),
                                IdConsultorio = lector.GetInt32(2),
                                IdDia = lector.GetInt32(3),
                                HoraInicioJornada = lector.GetTimeSpan(4),
                                HoraFinJornada = lector.GetTimeSpan(5),
                                DuracionSlotMinutos = lector.GetInt32(6),
                                AgendaActiva = lector.GetBoolean(7)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return agendasDisponibilidad;
            }
        }

        //Método que valida que un medico no pueda tener agendas con días y horas que choquen con una ya existente
        //Esto incluye no permmitir a un mismo médico trabajar al mismo tiempo en dos consultorios simultaneamente
        public async Task<FranjaDto?> ExisteChoque()
        {
            string sentencia = "select ad.idAgendaDisponibilidad, ad.idMedico, ad.idConsultorio, c.nombre, ad.idDia, ad.horaInicioJornada, ad.horaFinJornada, ad.duracionSlotMinutos, ad.agendaActiva from AgendaDisponibilidad ad " +
                "inner join Consultorio c on ad.idConsultorio = c.idConsultorio where ad.idMedico = @idMedico and ad.idDia = @idDia and ad.idAgendaDisponibilidad <> @idAgendaIgnorar and (ad.horaInicioJornada < @horaFinJornada AND ad.horaFinJornada > @horaInicioJornada)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", this.idConsultorio);
                comando.Parameters.AddWithValue("@idDia", this.idDia);
                comando.Parameters.AddWithValue("@horaInicioJornada", this.horaInicioJornada);
                comando.Parameters.AddWithValue("@horaFinJornada", this.horaFinJornada);
                comando.Parameters.AddWithValue("@idAgendaIgnorar", this.idAgendaDisponibilidad);

                try
                {
                    if (conexion.State != System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                    }
                    using (SqlDataReader reader = await comando.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            //Si encuentra una coincidencia (choque) retornara un objeto FranjaDto con los datos del conflicto para mostrar en el DisplayAlert
                            return new FranjaDto
                            {
                                IdAgendaDisponibilidad = reader.GetInt32(0),
                                IdMedico = reader.GetInt32(1),
                                IdConsultorio = reader.GetInt32(2),
                                NombreConsultorio = reader.GetString(3),
                                IdDia = reader.GetInt32(4),
                                HoraInicioJornada = reader.GetTimeSpan(5),
                                HoraFinJornada = reader.GetTimeSpan(6),
                                DuracionSlotMinutos = reader.GetInt32(7),
                                AgendaActiva = reader.GetBoolean(8)
                            };
                        }
                    }
                    return null; //Si no hubo choque retorna un null
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Error verificando choque de horarios: {ex.Message}", "Aceptar");
                    return null;
                }
            }
        }

        public async Task<int> AgregarNuevaAgendaDisponibilidad()
        {
            sentencia = "insert into AgendaDisponibilidad (idMedico, idConsultorio, idDia, horaInicioJornada, horaFinJornada, duracionSlotMinutos, agendaActiva) " +
                "values (@idMedico, @idConsultorio, @idDia, @horaInicioJornada, @horaFinJornada, @duracionSlotMinutos, @agendaActiva)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", this.idConsultorio);
                comando.Parameters.AddWithValue("@idDia", this.idDia);
                comando.Parameters.AddWithValue("@horaInicioJornada", this.horaInicioJornada);
                comando.Parameters.AddWithValue("@horaFinJornada", this.horaFinJornada);
                comando.Parameters.AddWithValue("@duracionSlotMinutos", this.duracionSlotMinutos);
                comando.Parameters.AddWithValue("@agendaActiva", this.agendaActiva);

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
                    await Shell.Current.DisplayAlert("Error", $"Error al insertar: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }

        public async Task<int> ModificarAgendaDisponibilidad()
        {
            sentencia = "update AgendaDisponibilidad set idDia = @idDia, horaInicioJornada = @horaInicioJornada, horaFinJornada = @horaFinJornada, " +
                "duracionSlotMinutos = @duracionSlotMinutos, agendaActiva = @agendaActiva, fechaModificacion = @fechaModificacion where idAgendaDisponibilidad = @idAgendaDisponibilidad";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@IdAgendaDisponibilidad", this.idAgendaDisponibilidad);
                comando.Parameters.AddWithValue("@idDia", this.idDia);
                comando.Parameters.AddWithValue("@horaInicioJornada", this.horaInicioJornada);
                comando.Parameters.AddWithValue("@horaFinJornada", this.horaFinJornada);
                comando.Parameters.AddWithValue("@duracionSlotMinutos", this.duracionSlotMinutos);
                comando.Parameters.AddWithValue("@agendaActiva", this.agendaActiva);
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
                    await Shell.Current.DisplayAlert("Error", $"Error al insertar: {ex.Message}", "Aceptar");
                    return -1;
                }
            }
        }
    }    
}
