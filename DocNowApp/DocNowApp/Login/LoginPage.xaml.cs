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
                await Shell.Current.GoToAsync("//PrincipalPage");
				break;
			case LoginSQL.estadoLogin.CredencialesIncorrectas:
				await DisplayAlert("Error", "Correo o contraseña incorrectos", "Aceptar");
				break;
			case LoginSQL.estadoLogin.Error:
				break;
        }
    }

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

    private void btnMostrarContrasenia_Clicked(object sender, EventArgs e)
    {
        this.txtContrasenia.IsPassword = !this.txtContrasenia.IsPassword;

        this.btnMostrarContrasenia.Source = this.txtContrasenia.IsPassword ? "mostrar_contrasenia.png" : "ocultar_contrasenia.png";
    }
}