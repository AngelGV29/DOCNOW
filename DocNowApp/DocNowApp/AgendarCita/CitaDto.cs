using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendarCita
{
    public class CitaDto
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public int IdConsultorio { get; set; }
        public int IdMotivo { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string EstadoCita { get; set; }
        public string Notas { get; set; }

        public string NombrePaciente { get; set; }
        public string NombreMedico { get; set; }
        public string NombreConsultorio { get; set; }
        public string AlergiaPaciente { get; set; }
        public string MedicacionPaciente { get; set; }
        public string DescripcionMotivo { get; set; }
        public string InstruccionMotivo { get; set; }
    }
}
