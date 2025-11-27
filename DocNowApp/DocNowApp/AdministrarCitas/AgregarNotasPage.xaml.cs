namespace DocNowApp.AdministrarCitas;

public partial class AgregarNotasPage : ContentPage
{
    private readonly TaskCompletionSource<string> tareaModal = new TaskCompletionSource<string>();

    // Expuesto públicamente para que quien abra la página pueda await modal.Resultado
    public Task<string> Resultado => tareaModal.Task;

    string notasDefault;
    public AgregarNotasPage()
	{
		InitializeComponent();
	}

    public AgregarNotasPage(int opcion)
    {
        InitializeComponent();
        switch (opcion)
        {
            case 1:
                this.notasDefault = "Cita completada.";
                break;
            case 2:
                this.notasDefault = "Cita cancelada por el médico.";
                break;
        }
    }
    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        try
        {
            //Con esto se evita una excepción a causa de un valor nulo
            string notas = this.txtNotas.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(notas))
            {
                notas = this.notasDefault;
            }

            bool continuar = await DisplayAlert("Advertencia", "Las notas no podrán ser editadas de nuevo, ¿Desea continuar?", "Aceptar", "Cancelar");
            if (!continuar)
            {
                return;
            }
            // Completa la tarea modal con las notas ingresadas
            this.tareaModal.TrySetResult(notas);
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron guardar las notas: {ex.Message}", "Aceptar");
        }
    }

    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        this.tareaModal.TrySetResult(null);
        await Navigation.PopModalAsync();
    }  
}