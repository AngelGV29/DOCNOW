using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendarCita
{
    public class TurnoDto
    {
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool Disponible { get; set; }

        public string RangoHorario =>
        $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
    }
}
