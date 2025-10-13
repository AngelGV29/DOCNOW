using System.Data;

namespace DocNowApp.NuevoPaciente;

public partial class NuevoPacientePage : ContentPage
{
	public NuevoPacientePage()
	{
		InitializeComponent();
        //Ajusta la fecha m�xima del DatePicker a la fecha actual
        this.dateUltimaVisita.MaximumDate = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        //Reinicia los controles de la p�gina
        this.txtAlergia.Text = "";
        this.txtMedicacion.Text = "";
        this.dateUltimaVisita.Date = DateTime.Today;
        this.txtAlergia.Focus();
    }

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        try
        {
            //Si alguno de los cuadros de texto est� vac�o, el cuadro no podr� continuar
            if (string.IsNullOrWhiteSpace(this.txtAlergia.Text) || string.IsNullOrWhiteSpace(this.txtMedicacion.Text))
            {
                await DisplayAlert("Advertencia", "Debe rellenar los campos con la informaci�n solicitada", "Aceptar");
                return;
            }
            
            //Crea el objeto para crear el paciente
            NuevoPaciente.NuevoPacienteSQL crear = new NuevoPaciente.NuevoPacienteSQL(this.txtAlergia.Text, this.txtMedicacion.Text, this.dateUltimaVisita.Date);
            
            //Ejecuta el m�todo para crear el nuevo paciente, si se afect� m�s de una fila la creaci�n fue exitosa
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                //Obtiene el ID del paciente reci�n creado y lo asigna al administrador de sesi�n
                DataSet datos = await crear.ObtenerIdPaciente();
                Globales.AdministradorDeSesion.idPaciente = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idPaciente"].ToString());
                
                //Ejecuta el m�todo que define al usuario como paciente, posteriormente, lo env�a a la p�gina principal de los pacientes
                await crear.AsignacionRol();
                await DisplayAlert("�xito", "Ha sido registrado como un nuevo paciente. Asegurese de mantener su informaci�n personal siempre actualizada en la pesta�a PENDIENTE", "Aceptar");
                await Shell.Current.GoToAsync("//PacientePrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creaci�n de nuevo paciente fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepci�n", $"Error: {ex.Message}", "Aceptar");
        }
    }
}