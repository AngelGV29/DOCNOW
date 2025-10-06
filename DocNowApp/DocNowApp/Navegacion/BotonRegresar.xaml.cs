namespace DocNowApp.Navegacion;

public partial class BotonRegresar : ContentView
{
	public BotonRegresar()
	{
		InitializeComponent();
	}
    public string Ruta { get; set; }
    
    private async void btnRegresar_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Con Shell, esto regresa a la p�gina anterior en la pila de navegaci�n
            await Shell.Current.GoToAsync($"//{Ruta}");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}