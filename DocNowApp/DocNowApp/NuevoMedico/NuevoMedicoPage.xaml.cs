using System.Data;

namespace DocNowApp.NuevoMedico;

public partial class NuevoMedicoPage : ContentPage
{
	public NuevoMedicoPage()
	{
		InitializeComponent();
    }

    //Método que se ejecuta al aparecer la página
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        NuevoMedico.NuevoMedicoSQL disponibles = new NuevoMedico.NuevoMedicoSQL();
        List<Consultorio> listaConsultorios = await disponibles.ObtenerConsultorios();

        this.CollectionConsultorios.ItemsSource = listaConsultorios;
    }

    

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        var consultoriosSeleccionados = this.CollectionConsultorios.SelectedItems.Cast<Consultorio>().ToList();

        if (string.IsNullOrWhiteSpace(this.txtCedula.Text) || string.IsNullOrWhiteSpace(this.txtEspecialidad.Text))
        {
            await DisplayAlert("Advertencia", "Debe rellenar los campos con la información solicitada", "Aceptar");
            return;
        }

        if (consultoriosSeleccionados.Count == 0)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar al menos un consultorio para continuar", "Aceptar");
            return;
        }
        try
        {
            NuevoMedico.NuevoMedicoSQL crear = new NuevoMedico.NuevoMedicoSQL(this.txtCedula.Text, this.txtEspecialidad.Text);
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                DataSet datos = await crear.ObtenerIdMedico();
                Globales.AdministradorDeSesion.idPaciente = Convert.ToInt32(datos.Tables["Tabla"].Columns["idMedico"].ToString());
                resultado = await crear.VinculacionMedicoConsultorio(consultoriosSeleccionados);
                await DisplayAlert("Éxito", "Ha sido registrado como un nuevo médico. Asegurese de mantener su información personal siempre actualizada en la pestaña PENDIENTE", "Aceptar");
                await Shell.Current.GoToAsync("//PrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creación de nuevo médico fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", $"Error: {ex.Message}", "Aceptar");
        }
    }
}