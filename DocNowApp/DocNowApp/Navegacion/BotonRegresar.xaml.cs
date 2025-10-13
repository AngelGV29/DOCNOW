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
            //Esto permitir� navegar hacia una p�gina especidicada gracias al atributo ruta
            //Bot�nRegresar es una plantilla que puede ser utilizada en cualquier p�gina, y
            //al cual se le deb asignar un valor para el atributo ruta que defina hacia que
            //p�gina debe regresar (o navegar) la apliaci�n
            await Shell.Current.GoToAsync($"//{Ruta}");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}