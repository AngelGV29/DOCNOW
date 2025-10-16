namespace DocNowApp.AgendaDisponibilidad;

public partial class AgendaDisponibilidadPage : ContentPage
{
	public AgendaDisponibilidadPage()
	{
		InitializeComponent();
	}

    List<string> DuracionSlot = new List<string>
    {
        "10 min",
        "15 min",
        "30 min",
        "45 min",
        "50 min",
        "60 min",
        "90 min"
    };
}