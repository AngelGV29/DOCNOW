using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace DocNowApp.Globales
{
    public class BoolToActivoConversor : IValueConverter
    {
        // Convierte el bool a texto
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool activo)
                return activo ? "Activo" : "Inactivo";

            return "Desconocido";
        }
        // No lo usarás, pero es obligatorio implementarlo
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string texto)
                return texto.Equals("Activo", StringComparison.OrdinalIgnoreCase);
            return false;
        }
    }
}
