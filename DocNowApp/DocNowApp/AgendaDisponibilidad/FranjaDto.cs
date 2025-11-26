using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendaDisponibilidad
{
    public class FranjaDto
    {
        public int IdAgendaDisponibilidad { get; set; }
        public int IdMedico { get; set; }
        public int IdConsultorio { get; set; }
        public string NombreConsultorio { get; set; }
        public int IdDia { get; set; }
        public TimeSpan HoraInicioJornada { get; set; }
        public TimeSpan HoraFinJornada { get; set; }
        public int DuracionSlotMinutos { get; set; }
        public bool AgendaActiva { get; set; } = true;

        public string NombreDia =>
           IdDia switch
           {
               1 => "Lunes",
               2 => "Martes",
               3 => "Miércoles",
               4 => "Jueves",
               5 => "Viernes",
               6 => "Sábado",
               7 => "Domingo",
               _ => "Desconocido"
           };
        public string RangoHorario =>
        $" | {HoraInicioJornada:hh\\:mm} - {HoraFinJornada:hh\\:mm} | ";

        public string TextoDuracion =>
        $"Cada turno es de {DuracionSlotMinutos} min | ";
    }
}
