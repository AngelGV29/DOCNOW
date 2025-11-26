using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocNowApp.AgendarCita
{
    public class MedicoConsultorioDto
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
        public List<int> Dias { get; set; }

        public string DiasTexto
        {
            get
            {
                // Evitar null-reference y devolver cadena vacía si no hay días
                if (Dias == null || Dias.Count == 0) return string.Empty;

                // Convertir números a nombres y unir con coma
                return string.Join(", ", Dias.Select(d => NombreDia(d)));
            }
        }

        // Helper privado para mapear número -> nombre del día
        private static string NombreDia(int d) => d switch
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

    }
}
