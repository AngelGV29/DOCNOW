using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Text.RegularExpressions;
using DocNowApp.Navegacion;

namespace DocNowApp.NuevoUsuario;

public partial class NuevoUsuarioPage : ContentPage
{
    public NuevoUsuarioPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Ajusta la fecha máxima del picker a la actual
        this.dateFechaNac.MaximumDate = DateTime.Today;
        //Hace que el picker para seleccionar el dominio de correo utilice la lista de dominios ya definida
        this.pickerDominio.ItemsSource = dominios;
        //Reinicia los controles de la página
        this.txtNombre.Text = "";
        this.txtApellP.Text = "";
        this.txtApellM.Text = "";
        this.txtCorreo.Text = "";
        this.txtContrasenia.Text = "";
        this.txtTelefono.Text = "";
        this.dateFechaNac.Date = DateTime.Today;
        this.rbHombre.IsChecked = false;
        this.rbMujer.IsChecked = false;
        this.pickerDominio.SelectedIndex = -1;
        this.lblDominio.IsVisible = true;
        this.txtNombre.Focus();
    }

    //Es una lista con los dominios de correo que el usuario puede utilizar
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
    
    //Mientras algún cuadro de texto este vacío, el botón para registrar el usuario no se activará
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

        //Dependiendo del ClassId de la entrada, se aplican diferentes filtros al texto introducido
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

            //Evitar introducir datos no númericos en donde no correspondan
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

    //Valida que el nombre del correo solo contenga los caracteres permitidos
    bool EsNombreCorreoValido(string nombre)
    {
        return Regex.IsMatch(nombre, @"^[a-zA-Z0-9._-]+$");
    }

    private async void btnRegistrarse_Clicked(object sender, EventArgs e)
    {
        try
        {
            char sexo;
            //Si no hay un sexo seleccionado, el usuario no podrá continuar
            if (!this.rbHombre.IsChecked && !this.rbMujer.IsChecked)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar un sexo", "Aceptar");
                return;
            }
            //Si no se selecciono ningun dominio de correo electrónico, el usuario no podrá continuar
            if (this.pickerDominio.SelectedItem == null)
            {
                await DisplayAlert("Advertencia", "Debe seleccionar un dominio de correo electrónico", "Aceptar");
                return;
            }
            //Si el nombre de correo introducido posee caracteres no permitidos, el usuario no podrá continuar
            if (!EsNombreCorreoValido(this.txtCorreo.Text))
            {
                await DisplayAlert("Advertencia", "Debe introducir solo caracteres permitidos en el campo de correo (letras, numeros, guiones y puntos)", "Aceptar");
                return;
            }
            //Si el teléfono introducido no tiene 10 dígitos (los usados en México), el usuario no podrá continuar
            if (this.txtTelefono.Text.Length != 10)
            {
                await DisplayAlert("Advertencia", "El número de teléfono debe ser de 10 dígitos", "Aceptar");
                return;
            }
            
            //Se pasa el valor de sexo seleccionado como un caracter
            if (this.rbHombre.IsChecked)
            {
                sexo = 'H';
            }
            else
            {
                sexo = 'M';
            }
            //Se concatena el nombre del cooreo introducido y el dominio seleccionado
            string correo = this.txtCorreo.Text + this.pickerDominio.SelectedItem.ToString();
            
            //Se crea el objeto para crear el nuevo usuario
            NuevoUsuario.NuevoUsuarioSQL crear = new NuevoUsuario.NuevoUsuarioSQL(this.txtNombre.Text, this.txtApellP.Text, this.txtApellM.Text,
                correo, this.txtTelefono.Text, this.txtContrasenia.Text, this.dateFechaNac.Date, sexo, "PENDIENTE", DateTime.Now, DateTime.Now);

            //Valida que el correo introducido no esté ya registrado en la base de datos
            DataSet datos = await crear.ValidarCorreo();
            if (datos.Tables.Count == 0 || datos.Tables["Tabla"].Rows.Count == 0)
            {
                //Ejecuta el método para crear el nuevo usuario, si se afecto más de una fila el registro fue exitoso
                int resultado = await crear.Creacion();
                if (resultado > 0)
                {
                    //Se obitene el ID del usuario recien registrado para poder administrar su sesión
                    datos = await crear.ObtenerIdUsuario();
                    Globales.AdministradorDeSesion.idUsuario = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idUsuario"].ToString());

                    /*La plantilla para crear el usuario es la misma, sin embargo, si quien lo está creando es un administrador
                    tras registrar el usuario le aparecerá la pantalla para llenar los datos del médico
                    Un usuario normal será dirigido a la pantalla para llenar los datos del paciente*/
                    if (Globales.AdministradorDeSesion.Rol == "ADMIN")
                    {
                        await Shell.Current.GoToAsync("//NuevoMedicoPage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//NuevoPacientePage");

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

    //Botón que oculta o muestra la contraseña
    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }

    //Elimina la etiqueta superpuesta en el picker para seleccionar un dominio de correo
    private void PickerDominio_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.lblDominio.IsVisible = pickerDominio.SelectedIndex == -1;
    }
}