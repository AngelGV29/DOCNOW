namespace DocNowApp.MedicoPages;

public partial class MedicoPrincipalPage : ContentPage
{
	public MedicoPrincipalPage()
	{
		InitializeComponent();
	}

    //Botón para que el médico configure las agendas de disponibilidad (franjas de tiempo)
    private async void btnConfigurarAgenda_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ConfigurarAgendaDisponibilidadPage");
    }

    //Botón para que el médico administre las citas solicitadas por los pacientes
    private async void btnAdministrarCitas_Clicked(object sender, EventArgs e)
    {
        AdministrarCitasPage administrarCitas = new AdministrarCitasPage();
        await Navigation.PushAsync(administrarCitas);
    }
}