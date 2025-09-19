namespace DocNowApp.Inicio;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
	}

    private async void btnNuevo_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NuevoUsuarioPage");
    }

    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}