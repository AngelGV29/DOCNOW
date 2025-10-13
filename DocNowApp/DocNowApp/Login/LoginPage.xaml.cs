using System.Data;

namespace DocNowApp.Login;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    //M�todo que se ejecuta al cargar la p�gina
    protected override void OnAppearing()
    {
        /*La llamada a base.OnAppearing() es necesaria para asegurar que la clase
        base realice cualquier inicializaci�n necesaria y funcionalidad definida,
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

        //Validaci�n por medio de un estadoLogin
        switch (await acceso.Validacion())
		{
			case LoginSQL.estadoLogin.Exito:
                //Si el login fue exitoso, la aplicaci�n obtendr� el ID del usuario y el rol que posee
                DataSet datos = await acceso.ObtenerIdUsuario();

                //El ID y el rol del usuario son pasados al administrador de sesi�n
                Globales.AdministradorDeSesion.idUsuario = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idUsuario"].ToString());
                Globales.AdministradorDeSesion.Rol = datos.Tables["Tabla"].Rows[0]["rol"].ToString();

                //M�todo que actualiza la �ltima fecha de login del usuario
                await acceso.ModificarUltimoLogin();

                //En funci�n del rol, la aplicaci�n navegar� a la p�gina principal correspondiente
                /*Si por alg�n motivo, el rolo no pertenece a ninguno, navegar� a la p�gina de nuevo paciente
                en el entendido de que el registron del nuevo usuario se qued� a medias*/
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
				await DisplayAlert("Error", "Correo o contrase�a incorrectos", "Aceptar");
				break;
			case LoginSQL.estadoLogin.Error:
				break;
        }
    }

    //S� alguno de los cuadros de texto se queda vac�o, la aplicaci�n no permitir� continuar con el login
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
            //Convierte el texto de entradas de correo en min�sculas
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

    //Al pulsar el bot�n, la contrase�a se mostrar� o se ocultar�
    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }
}