namespace DocNowApp.AdminPages;

public partial class PrincipalPage : ContentPage
{
	public PrincipalPage()
	{
		InitializeComponent();
	}

    //Bot�n para crear un nuevo m�dico
    private async void btnNuevoMedico_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//NuevoUsuarioPage");
    }

    private async void btnNuevoConsultorio_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NuevoConsultorioPage");
    }
}