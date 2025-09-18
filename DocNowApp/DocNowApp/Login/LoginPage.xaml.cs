namespace DocNowApp.Login;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
		Login.LoginSQL acceso = new Login.LoginSQL(this.txtCorreo.Text, this.txtContrasenia.Text);

        //Validación por medio de un estadoLogin
        switch (await acceso.Validacion())
		{
			case LoginSQL.estadoLogin.Exito:
                //Si el login fue exitoso la aplicación avanzará a la página principal
                await Shell.Current.GoToAsync("//PrincipalPage");
				break;
			case LoginSQL.estadoLogin.CredencialesIncorrectas:
				await DisplayAlert("Error", "Correo o contraseña incorrectos", "Aceptar");
				break;
			case LoginSQL.estadoLogin.Error:
				break;
        }
    }

    //Sí alguno de los cuadros de texto se queda vacío, la aplicación no permitirá continuar con el login
    private void txtCorreo_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtCorreo.Text) || string.IsNullOrWhiteSpace(this.txtContrasenia.Text))
        {
            this.btnLogin.IsEnabled = false;
        }
        else
        {
            this.btnLogin.IsEnabled = true;
        }
    }

    //Sí alguno de los cuadros de texto se queda vacío, la aplicación no permitirá continuar con el login
    private void txtContrasenia_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtCorreo.Text) || string.IsNullOrWhiteSpace(this.txtContrasenia.Text))
        {
            this.btnLogin.IsEnabled = false;
        }
        else
        {
            this.btnLogin.IsEnabled = true;
        }
    }

    //Al pulsar el botón, la contraseña se mostrará o se ocultará
    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }
}