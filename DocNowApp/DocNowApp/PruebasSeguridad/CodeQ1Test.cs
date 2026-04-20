using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

namespace DocNowApp.PruebasSeguridad
{
    internal class CodeQlTest
    {
        public void ConsultaInsegura(string userInput, SqlConnection connection)
        {
            // EJEMPLO INTENCIONALMENTE INSEGURO PARA PROBAR CODEQL
            // No lo uses en código real.
            var query = "SELECT * FROM Usuarios WHERE Nombre = '" + userInput + "'";

            using var command = new SqlCommand(query, connection);
            // No se ejecuta la consulta: es suficiente para que CodeQL detecte el patrón.
        }
    }
}