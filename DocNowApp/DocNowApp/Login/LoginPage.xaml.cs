using System.Data;

namespace DocNowApp.Login;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    //Método que se ejecuta al cargar la página
    protected override void OnAppearing()
    {
        /*La llamada a base.OnAppearing() es necesaria para asegurar que la clase
        base realice cualquier inicialización necesaria y funcionalidad definida,
        es decir, su comportamiento original*/
        base.OnAppearing();
        //Reinicia los valores de los controles
        this.txtCorreo.Text = "";
        this.txtContrasenia.Text = "";
        this.txtCorreo.Focus();
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        //Crea una instancia de la clase LoginSQL con los datos ingresados en los cuadros de texto
        Login.LoginSQL acceso = new Login.LoginSQL(this.txtCorreo.Text, this.txtContrasenia.Text);

        //Validación por medio de un estadoLogin
        switch (await acceso.Validacion())
		{
			case LoginSQL.estadoLogin.Exito:
                //Si el login fue exitoso, la aplicación obtendrá el ID del usuario y el rol que posee
                DataSet datos = await acceso.ObtenerIdUsuario();

                //El ID y el rol del usuario son pasados al administrador de sesión
                Globales.AdministradorDeSesion.idUsuario = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idUsuario"].ToString());
                Globales.AdministradorDeSesion.Rol = datos.Tables["Tabla"].Rows[0]["rol"].ToString();

                //Método que actualiza la última fecha de login del usuario
                await acceso.ModificarUltimoLogin();

                //En función del rol, la aplicación navegará a la página principal correspondiente
                /*Si por algún motivo, el rolo no pertenece a ninguno, navegará a la página de nuevo paciente
                en el entendido de que el registron del nuevo usuario se quedó a medias*/
                switch (Globales.AdministradorDeSesion.Rol)
                {
                    case "PACIENTE":
                        await Shell.Current.GoToAsync("//PacientePrincipalPage");
                        break;
                    case "MEDICO":
                        await Shell.Current.GoToAsync("//MedicoPrincipalPage");
                        break;
                    case "ADMIN":
                        await Shell.Current.GoToAsync("//AdminPrincipalPage");
                        break;
                    default:
                        await Shell.Current.GoToAsync("//NuevoPacientePage");
                        break;
                }
                break;
			case LoginSQL.estadoLogin.CredencialesIncorrectas:
				await DisplayAlert("Error", "Correo o contraseña incorrectos", "Aceptar");
				break;
			case LoginSQL.estadoLogin.Error:
				break;
        }
    }

    //Sí alguno de los cuadros de texto se queda vacío, la aplicación no permitirá continuar con el login
    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtCorreo.Text) || string.IsNullOrWhiteSpace(this.txtContrasenia.Text))
        {
            this.btnLogin.IsEnabled = false;
        }
        else
        {
            this.btnLogin.IsEnabled = true;
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
            //Permite todos los caracteres
            case "contrasenia":
            default:
                //No hacee nada
                break;
        }
    }

    //Al pulsar el botón, la contraseña se mostrará o se ocultará
    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }
}