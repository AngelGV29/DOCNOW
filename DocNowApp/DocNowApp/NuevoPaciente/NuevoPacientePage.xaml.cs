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
                await DisplayAlert("Advertencia", "Debe rellenar los campos con la información solicitada", "Aceptar");
                return;
            }
            
            NuevoPaciente.NuevoPacienteSQL crear = new NuevoPaciente.NuevoPacienteSQL(this.txtAlergia.Text, this.txtMedicacion.Text, this.dateUltimaVisita.Date);
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                DataSet datos = await crear.ObtenerIdPaciente();
                Globales.AdministradorDeSesion.idPaciente = Convert.ToInt32(datos.Tables["Tabla"].Columns["idPaciente"].ToString());
                await DisplayAlert("Éxito", "Ha sido registrado como un nuevo paciente. Asegurese de mantener su información personal siempre actualizada en la pestaña PENDIENTE", "Aceptar");
                await Shell.Current.GoToAsync("//PrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creación de nuevo paceinte fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", $"Error: {ex.Message}", "Aceptar");
        }
    }
}