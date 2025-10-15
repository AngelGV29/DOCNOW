namespace DocNowApp.Inicio;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
	}

    //M�todo que se ejecuta al iniciar la p�gina
    protected override void OnAppearing()
    {
        /*La llamada a base.OnAppearing() es necesaria para asegurar que la clase
        base realice cualquier inicializaci�n necesaria y funcionalidad definida,
        es decir, su comportamiento original*/
        base.OnAppearing();
        //Reinicia los valores de la sesi�n actual
        Globales.AdministradorDeSesion.idUsuario = 0;
        Globales.AdministradorDeSesion.idPaciente = 0;
        Globales.AdministradorDeSesion.idMedico = 0;
        Globales.AdministradorDeSesion.idAdmin = 0;
        Globales.AdministradorDeSesion.Rol = "";
    }

    //Bot�n para registrarse
    private async void btnNuevo_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NuevoUsuarioPage");
    }

    //Bot�n para iniciar sesi�n
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}