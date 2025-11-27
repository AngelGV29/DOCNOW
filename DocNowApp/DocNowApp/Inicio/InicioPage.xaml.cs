using DocNowApp.NuevoUsuario;

namespace DocNowApp.Inicio;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
	}

    //Método que se ejecuta al iniciar la página
    protected override void OnAppearing()
    {
        /*La llamada a base.OnAppearing() es necesaria para asegurar que la clase
        base realice cualquier inicialización necesaria y funcionalidad definida,
        es decir, su comportamiento original*/
        base.OnAppearing();
        //Reinicia los valores de la sesión actual
        Globales.AdministradorDeSesion.idUsuario = 0;
        Globales.AdministradorDeSesion.idPaciente = 0;
        Globales.AdministradorDeSesion.idMedico = 0;
        Globales.AdministradorDeSesion.idAdmin = 0;
        Globales.AdministradorDeSesion.Rol = "";
    }

    //Botón para registrarse
    private async void btnNuevo_Clicked(object sender, EventArgs e)
    {
        NuevoUsuarioPage nuevoPaciente = new NuevoUsuarioPage();
        await Navigation.PushAsync(nuevoPaciente);
    }

    //Botón para iniciar sesión
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}