using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Text.RegularExpressions;

namespace DocNowApp.NuevoUsuario;

public partial class NuevoUsuarioPage : ContentPage
{
    public NuevoUsuarioPage()
    {
        InitializeComponent();
        this.dateFechaNac.MaximumDate = DateTime.Today;
        this.pickerDominio.ItemsSource = dominios;
    }

    List<string> dominios = new List<string>
    {
        "@gmail.com",
        "@outlook.com",
        "@hotmail.com",
        "@hotmail.com.mx",
        "@live.com.mx",
        "@yahoo.com",
        "@yahoo.com.mx",
        "@icloud.com"
    };
    


    private void Entry_TextChangued(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtNombre.Text) || string.IsNullOrWhiteSpace(this.txtApellP.Text) || string.IsNullOrWhiteSpace(this.txtApellM.Text)
           || string.IsNullOrWhiteSpace(this.txtCorreo.Text) || string.IsNullOrWhiteSpace(this.txtTelefono.Text) || string.IsNullOrWhiteSpace(this.txtContrasenia.Text))
        {
            this.btnRegistrarse.IsEnabled = false;
        }
        else
        {
            this.btnRegistrarse.IsEnabled = true;
        }

        var entry = (Entry)sender;

        switch (entry.ClassId)
        {
            //Convierte el texto de entradas de correo en minúsculas
            case "lowercase":
                if (!string.IsNullOrEmpty(entry.Text) && entry.Text != entry.Text.ToLower())
                {
                    int cursor = entry.CursorPosition;
                    entry.Text = entry.Text.ToLower();
                    entry.CursorPosition = cursor;
                }
                break;

            //Coloca el texto de entradas de nombres personales en mayúsculas
            case "uppercase":
                if (!string.IsNullOrEmpty(entry.Text) && entry.Text != entry.Text.ToUpper())
                {
                    int cursor = entry.CursorPosition;
                    entry.Text = entry.Text.ToUpper();
                    entry.CursorPosition = cursor;
                }
                break;

            //Evitara introducir datos no númericos en donde no correspondan
            case "numero":
                if (!string.IsNullOrEmpty(entry.Text))
                {
                    string filtered = new string(entry.Text.Where(char.IsDigit).ToArray());

                    if (entry.Text != filtered)
                    {
                        int cursor = entry.CursorPosition;
                        entry.Text = filtered;
                        entry.CursorPosition = Math.Min(cursor, filtered.Length);
                    }
                }
                break;

            //No modifica el texto de la contraseña
            case "contrasenia":
            default:
                //No hacee nada
                break;
        }
    }
    bool EsNombreCorreoValido(string nombre)
    {
        return Regex.IsMatch(nombre, @"^[a-zA-Z0-9._-]+$");
    }

    private async void btnRegistrarse_Clicked(object sender, EventArgs e)
    {
        try
        {
            char sexo;
            if (!this.rbHombre.IsChecked && !this.rbMujer.IsChecked)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar un sexo", "Aceptar");
                return;
            }
            if (this.pickerDominio.SelectedItem == null)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar un dominio de correo electrónico", "Aceptar");
                return;
            }
            if (!EsNombreCorreoValido(this.txtCorreo.Text))
            {
                await DisplayAlert("Advertencia", "Debe introducir solo caracteres permitidos (letras, numeros, guiones y puntos)", "Aceptar");
                return;
            }
            if (this.txtTelefono.Text.Length != 10)
            {
                await DisplayAlert("Advertencia", "El número de teléfono debe ser de 10 dígitos", "Aceptar");
                return;
            }

            if (this.rbHombre.IsChecked)
            {
                sexo = 'H';
            }
            else
            {
                sexo = 'M';
            }
            string correo = this.txtCorreo.Text + this.pickerDominio.SelectedIndex.ToString();
            NuevoUsuario.NuevoUsuarioSQL crear = new NuevoUsuario.NuevoUsuarioSQL(this.txtNombre.Text, this.txtApellP.Text, this.txtApellM.Text,
                correo, this.txtTelefono.Text, this.txtContrasenia.Text, this.dateFechaNac.Date, sexo, "ROL PENDIENTE", DateTime.Today, DateTime.Today);
            DataSet datos = await crear.ValidarCorreo();
            if (datos.Tables.Count == 0 || datos.Tables["Tabla"].Rows.Count == 0)
            {
                int resultado = await crear.Creacion();
                if (resultado > 0)
                {
                    datos = await crear.ObtenerIdUsuario();
                    Globales.AdministradorDeSesion.idUsuario = Convert.ToInt32(datos.Tables["Tabla"].Columns["idUsuario"].ToString());
                    bool opcion = await DisplayAlert("Exito", "Se ha creado correctamente el usuario. A continuación, elige el rol que deseas tener", "Paciente", "Medico");
                    if (opcion)
                    {
                        await Shell.Current.GoToAsync("//NuevoPacientePage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//NuevoMedicoPage");
                    }
                }
                else if (resultado == 0)
                {
                    await DisplayAlert("Error", "Creación de nuevo usuario fallida", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Advertencia", "El correo introducido ya existe", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", $"Excepcioón: {ex.Message}", "Aceptar");
        }
    }

    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }

    private void PickerDominio_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.lblDominio.IsVisible = pickerDominio.SelectedIndex == -1;
    }
}