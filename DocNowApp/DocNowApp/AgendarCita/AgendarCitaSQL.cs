using DocNowApp.AgendaDisponibilidad;
using DocNowApp.Globales;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendarCita
{
    class MedicoConsultorioTemporal
    {
        public int IdMedicoConsultorio { get; set; }
        public int IdMedico { get; set; }
        public int IdConsultorio { get; set; }
        public string NombreMedico { get; set; }
        public string NombreEspecialidad { get; set; }
        public string NombreConsultorio { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int Dia { get; set; }

    }
    class AgendarCitaSQL
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
        private DateTime fechaCreacion;
        private DateTime fechaModificacion;


        public AgendarCitaSQL(CitaDto _citaDto)
        {
            this.idCita = _citaDto.IdCita;
            this.idPaciente = _citaDto.IdPaciente;
            this.idMedico = _citaDto.IdMedico;
            this.idConsultorio = _citaDto.IdConsultorio;
            this.idMotivo = _citaDto.IdMotivo;
            this.fechaCita = _citaDto.FechaCita;
            this.horaInicio = _citaDto.HoraInicio;
            this.horaFin = _citaDto.HoraFin;
            this.estadoCita = _citaDto.EstadoCita;
        }

        public AgendarCitaSQL(int idPaciente)
        {
            this.idPaciente = idPaciente;
        }

        public AgendarCitaSQL(int idMedico, int idConsultorio)
        {
            this.idMedico = idMedico;
            this.idConsultorio = idConsultorio;
        }

        public AgendarCitaSQL(MedicoConsultorioDto medicoConsultorio, DateTime fechaSeleccionada)
        {
            this.idMedico = medicoConsultorio.IdMedico;
            this.idConsultorio = medicoConsultorio.IdConsultorio;
            this.fechaCita = fechaSeleccionada;
            this.idDia = (int)fechaSeleccionada.DayOfWeek;
            if (this.idDia == 0)
            {
                this.idDia = 7; //Domingo
            }
        }

        public async Task<List<CitaDto>> ObtenerCitasAgendadas()
        {
            sentencia = "select c.idCita, c.idPaciente, c.idMedico, c.idConsultorio, c.idMotivo, c.fechaCita, c.horaInicio, c.horaFin, c.estadoCita, " +
                "uMed.nombre + ' ' + uMed.apellPaterno + ' ' + uMed.apellMaterno as NombreMedico, " +
                "cons.nombre as NombreConsultorio," +
                "motc.descripcion as DescripcionMotivo, " +
                "motc.instruccion as InstruccionMotivo " +
                "from Cita c " +
                "inner join Medico m on c.idMedico = m.idMedico " +
                "inner join Usuario uMed on m.idUsuario = uMed.idUsuario " +
                "inner join Consultorio cons on c.idConsultorio = cons.idConsultorio " +
                "inner join MotivoConsulta motc on c.idMotivo = motc.idMotivo " +
                "where c.idPaciente = @idPaciente and c.estadoCita in ('AGENDADA', 'REAGENDADA') " +
                "order by c.fechaCita asc, c.horaInicio";
            List<CitaDto> citasAgendadas = new List<CitaDto>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idPaciente", this.idPaciente);
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
                                NombreMedico = lector.GetString(9),
                                NombreConsultorio = lector.GetString(10),
                                DescripcionMotivo = lector.GetString(11),
                                InstruccionMotivo = lector.IsDBNull(12) ? "" : lector.GetString(12)
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

        public async Task<List<MedicoConsultorioDto>> ObtenerMedicoConsultorios()
        {
            sentencia = "select mc.idMedicoConsultorio, mc.idMedico, mc.idConsultorio, uMed.nombre + ' ' + uMed.apellPaterno + ' ' + uMed.apellMaterno as NombreMedico, esp.nombreEspecialidad, " +
                "c.nombre, c.telefono, c.calle + ', #Int: ' + isnull(c.numeroInterior, 'Sin número interior') + ', #Ext: ' + c.numeroExterior + ', Col: ' + c.colonia + ', CP: ' + c.codigoPostal as Direccion, " +
                "ad.idDia from MedicoConsultorio mc inner join Medico m on m.idMedico = mc.idMedico inner join Usuario uMed on m.idUsuario = uMed.idUsuario " +
                "inner join EspecialidadMedico esp on m.idEspecialidad = esp.idEspecialidad inner join Consultorio c on c.idConsultorio = mc.idConsultorio " +
                "inner join AgendaDisponibilidad ad on ad.idMedico = mc.idMedico and ad.idConsultorio = mc.idConsultorio where ad.agendaActiva= 1 " +
                "order by mc.idMedicoConsultorio, ad.idDia";

            List<MedicoConsultorioTemporal> medicoConsultorioDisponibles = new List<MedicoConsultorioTemporal>();
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
                            //Crea instancias de la clase temporal MedicoConsultorioTemporal y las almacena en una lista que posteriormente será organizada
                            medicoConsultorioDisponibles.Add(new MedicoConsultorioTemporal
                            {
                                IdMedicoConsultorio = lector.GetInt32(0),
                                IdMedico = lector.GetInt32(1),
                                IdConsultorio = lector.GetInt32(2),
                                NombreMedico = lector.GetString(3),
                                NombreEspecialidad = lector.GetString(4),
                                NombreConsultorio = lector.GetString(5),
                                Telefono = lector.GetString(6),
                                Direccion = lector.GetString(7),
                                Dia = lector.GetInt32(8)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                var listaFinal = medicoConsultorioDisponibles
                    .GroupBy(r => new { r.IdMedicoConsultorio, r.IdMedico, r.IdConsultorio, r.NombreMedico, r.NombreEspecialidad, r.NombreConsultorio, r.Telefono, r.Direccion })
                    .Select(g => new MedicoConsultorioDto
                    {
                        IdMedicoConsultorio = g.Key.IdMedicoConsultorio,
                        IdMedico = g.Key.IdMedico,
                        IdConsultorio = g.Key.IdConsultorio,
                        NombreMedico = g.Key.NombreMedico,
                        NombreEspecialidad = g.Key.NombreEspecialidad,
                        NombreConsultorio = g.Key.NombreConsultorio,
                        Telefono = g.Key.Telefono,
                        Direccion = g.Key.Direccion,
                        Dias = g.Select(r => r.Dia).Distinct().OrderBy(d => d).ToList()
                    })
                    .ToList();
                return listaFinal;
            }
        }

        public async Task<List<CitaDto>> ObtenerMotivosConsulta()
        {
            sentencia = "select idMotivo, descripcion, instruccion from MotivoConsulta where idMedico = @idMedico";

            List<CitaDto> listaMotivosConsulta = new List<CitaDto>();
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
                            //Crea instancias de la clase citaDto para guardar únicamente los valores del motivo de consulta y mostrarlos posteriormente en el picker
                            listaMotivosConsulta.Add(new CitaDto
                            {
                                IdMotivo = lector.GetInt32(0),
                                DescripcionMotivo = lector.GetString(1),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
            }
            return listaMotivosConsulta;
        }

        public async Task<List<FranjaDto>> ObtenerAgendasDisponibilidad()
        {
            sentencia = " select horaInicioJornada, horaFinJornada, duracionSlotMinutos from AgendaDisponibilidad " +
                "where idMedico = @idMedico and idConsultorio = @idConsultorio and idDia = @idDia and agendaActiva = @agendaActiva";

            List<FranjaDto> listaFranjas = new List<FranjaDto>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", this.idConsultorio);
                comando.Parameters.AddWithValue("@idDia", this.idDia);
                comando.Parameters.AddWithValue("@agendaActiva", 1);
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
                            //Crea instancias de la clase citaDto para guardar únicamente los valores del motivo de consulta y mostrarlos posteriormente en el picker
                            listaFranjas.Add(new FranjaDto
                            {
                                HoraInicioJornada = lector.GetTimeSpan(0),
                                HoraFinJornada = lector.GetTimeSpan(1),
                                DuracionSlotMinutos = lector.GetInt32(2)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
            }
            return listaFranjas;
        }

        public async Task<List<CitaDto>> ObtenerCitasExistentes()
        {
            sentencia = "select horaInicio, horaFin from cita where idMedico = @idMedico and idConsultorio = @idConsultorio and " +
                "fechaCita = @fechaCita and estadoCita not in ('CANCELADA', 'COMPLETADA')";
            List<CitaDto> citasExistentes = new List<CitaDto>();
            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", this.idConsultorio);
                comando.Parameters.AddWithValue("@fechaCita", this.fechaCita);
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
                            citasExistentes.Add(new CitaDto
                            {
                                HoraInicio = lector.GetTimeSpan(0),
                                HoraFin = lector.GetTimeSpan(1)
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Si surge una excepción, muestra un mensaje de error
                    await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "Aceptar");
                }
                return citasExistentes;
            }
        }

        public async Task<int> AgendarNuevaCita()
        {
            sentencia = "insert into Cita (idPaciente, idMedico, idConsultorio, idMotivo, fechaCita, horaInicio, horaFin, estadoCita) " +
                "values (@idPaciente, @idMedico, @idConsultorio, @idMotivo, @fechaCita, @horaInicio, @horaFin, @estadoCita)";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("@idPaciente", this.idPaciente);
                comando.Parameters.AddWithValue("@idMedico", this.idMedico);
                comando.Parameters.AddWithValue("@idConsultorio", this.idConsultorio);
                comando.Parameters.AddWithValue("@idMotivo", this.idMotivo);
                comando.Parameters.AddWithValue("@fechaCita", this.fechaCita);
                comando.Parameters.AddWithValue("@horaInicio", this.horaInicio);
                comando.Parameters.AddWithValue("@horaFin", this.horaFin);
                comando.Parameters.AddWithValue("@estadoCita", this.estadoCita);

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

        public async Task<int> ReagendarCita()
        {
            sentencia = "update Cita set fechaCita = @fechaCita, horaInicio = @horaInicio, horaFin = @horaFin, estadoCita = @estadoCita, " +
                "fechaModificacion = @fechaModificacion where idCita = @idCita";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("idCita", this.idCita);
                comando.Parameters.AddWithValue("@fechaCita", this.fechaCita);
                comando.Parameters.AddWithValue("@horaInicio", this.horaInicio);
                comando.Parameters.AddWithValue("@horaFin", this.horaFin);
                comando.Parameters.AddWithValue("@estadoCita", this.estadoCita);
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

        public async Task<int> CancelarCita()
        {
            sentencia = "update Cita set estadoCita = @estadoCita, notas = @notas " +
                "fechaModificacion = @fechaModificacion where idCita = @idCita";

            using (conexion = new SqlConnection(Globales.CadenaConexion.miConexion))
            using (comando = new SqlCommand(sentencia, conexion))
            {
                comando.Parameters.AddWithValue("idCita", this.idCita);
                comando.Parameters.AddWithValue("@estadoCita", "CANCELADA");
                comando.Parameters.AddWithValue("@notas", "La cita fue cancelada por el paciente.");
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
