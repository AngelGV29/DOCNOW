namespace DocNowApp.AdminPages;

public partial class AdminPrincipalPage : ContentPage
{
	public AdminPrincipalPage()
	{
		InitializeComponent();
	}

    //Botón para crear un nuevo médico
    private async void btnNuevoMedico_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NuevoUsuarioPage");
    }

    private async void btnNuevoConsultorio_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NuevoConsultorioPage");
    }
}