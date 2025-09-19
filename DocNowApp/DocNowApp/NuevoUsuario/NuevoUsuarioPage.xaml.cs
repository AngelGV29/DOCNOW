namespace DocNowApp.NuevoUsuario;

public partial class NuevoUsuarioPage : ContentPage
{
	public NuevoUsuarioPage()
	{
		InitializeComponent();
		this.dateFechaNac.MaximumDate = DateTime.Today;
	}
}