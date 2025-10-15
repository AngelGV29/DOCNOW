using System.Data;

namespace DocNowApp.NuevoPaciente;

public partial class NuevoPacientePage : ContentPage
{
	public NuevoPacientePage()
	{
		InitializeComponent();
        //Ajusta la fecha máxima del DatePicker a la fecha actual
        this.dateUltimaVisita.MaximumDate = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        //Reinicia los controles de la página
        this.txtAlergia.Text = "";
        this.txtMedicacion.Text = "";
        this.dateUltimaVisita.Date = DateTime.Today;
        this.txtAlergia.Focus();
    }

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        try
        {
            //Si alguno de los cuadros de texto está vacío, el cuadro no podrá continuar
            if (string.IsNullOrWhiteSpace(this.txtAlergia.Text) || string.IsNullOrWhiteSpace(this.txtMedicacion.Text))
            {
                await DisplayAlert("Advertencia", "Debe rellenar los campos con la información solicitada", "Aceptar");
                return;
            }
            
            //Crea el objeto para crear el paciente
            NuevoPaciente.NuevoPacienteSQL crear = new NuevoPaciente.NuevoPacienteSQL(this.txtAlergia.Text, this.txtMedicacion.Text, this.dateUltimaVisita.Date);
            
            //Ejecuta el método para crear el nuevo paciente, si se afectó más de una fila la creación fue exitosa
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                //Obtiene el ID del paciente recién creado y lo asigna al administrador de sesión
                DataSet datos = await crear.ObtenerIdPaciente();
                Globales.AdministradorDeSesion.idPaciente = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idPaciente"].ToString());
                
                //Ejecuta el método que define al usuario como paciente, posteriormente, lo envía a la página principal de los pacientes
                await crear.AsignacionRol();
                await DisplayAlert("Éxito", "Ha sido registrado como un nuevo paciente. Asegurese de mantener su información personal siempre actualizada en la pestaña PENDIENTE", "Aceptar");
                await Shell.Current.GoToAsync("//PacientePrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creación de nuevo paciente fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", $"Error: {ex.Message}", "Aceptar");
        }
    }
}