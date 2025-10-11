using System.Data;

namespace DocNowApp.NuevoPaciente;

public partial class NuevoPacientePage : ContentPage
{
	public NuevoPacientePage()
	{
		InitializeComponent();
		this.dateUltimaVisita.MaximumDate = DateTime.Today;
    }

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(this.txtAlergia.Text) || string.IsNullOrWhiteSpace(this.txtMedicacion.Text))
            {
                await DisplayAlert("Advertencia", "Debe rellenar los campos con la informaci�n solicitada", "Aceptar");
                return;
            }
            
            NuevoPaciente.NuevoPacienteSQL crear = new NuevoPaciente.NuevoPacienteSQL(this.txtAlergia.Text, this.txtMedicacion.Text, this.dateUltimaVisita.Date);
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                DataSet datos = await crear.ObtenerIdPaciente();
                Globales.AdministradorDeSesion.idPaciente = Convert.ToInt32(datos.Tables["Tabla"].Columns["idPaciente"].ToString());
                await DisplayAlert("�xito", "Ha sido registrado como un nuevo paciente. Asegurese de mantener su informaci�n personal siempre actualizada en la pesta�a PENDIENTE", "Aceptar");
                await Shell.Current.GoToAsync("//PrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creaci�n de nuevo paceinte fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepci�n", $"Error: {ex.Message}", "Aceptar");
        }
    }
}