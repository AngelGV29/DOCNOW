namespace DocNowApp.MedicoPages;

public partial class MedicoPrincipalPage : ContentPage
{
	public MedicoPrincipalPage()
	{
		InitializeComponent();
	}

    private async void btnConfigurarAgenda_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ConfigurarAgendaDisponibilidadPage");
    }
}