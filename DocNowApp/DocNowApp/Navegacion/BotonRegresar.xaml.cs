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
            //Esto permitirá navegar hacia una página especidicada gracias al atributo ruta
            //BotónRegresar es una plantilla que puede ser utilizada en cualquier página, y
            //al cual se le deb asignar un valor para el atributo ruta que defina hacia que
            //página debe regresar (o navegar) la apliación
            await Shell.Current.GoToAsync($"//{Ruta}");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}