using DocNowApp.AgendaDisponibilidad;
using DocNowApp.AgendarCita;

namespace DocNowApp.PacientePages;

public partial class PacientePrincipalPage : ContentPage
{
	public PacientePrincipalPage()
	{
		InitializeComponent();
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            //Llama al método para cargar las citas del paciente
            this.CargarCitasAgendadas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar las citas agendadas: {ex.Message}", "Aceptar");
        }
    }

    private async void CargarCitasAgendadas()
    {
        try
        {
            AgendarCitaSQL cargarCitas = new AgendarCitaSQL(Globales.AdministradorDeSesion.idPaciente);
            List<CitaDto> listaCitasAgendadas = await cargarCitas.ObtenerCitasAgendadas();
            if (listaCitasAgendadas != null)
            {
                this.collectionCitas.ItemsSource = listaCitasAgendadas;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al cargar la agenda de dispnibilidad: {ex.Message}", "Aceptar");
        }
        return;
    }

    private async void btnAgendarCita_Clicked(object sender, EventArgs e)
    {
        AgendarCitaPage AgendarNuevaCita= new AgendarCitaPage();
        await Navigation.PushAsync(AgendarNuevaCita);
    }

    private async void btnReagendar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            CitaDto? citaSeleccionada = btn.CommandParameter as CitaDto;
            if (citaSeleccionada == null)
            {
                return;
            }
            AgendarCitaPage ReagendarCita = new AgendarCitaPage(citaSeleccionada);
            await Navigation.PushAsync(ReagendarCita);

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al intentar reagendar la cita: {ex.Message}", "Aceptar");
        }
    }

    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            CitaDto? citaSeleccionada = btn.CommandParameter as CitaDto;
            if (citaSeleccionada == null)
            {
                return;
            }
            int resultadoOperacion;
            bool continuar = await DisplayAlert("Advertencia", $"Esta por cancelar la cita agendada el día {citaSeleccionada.FechaCita:dd/MM/yyyy} a las {citaSeleccionada.HoraInicio:hh\\:mm} " +
                $"con el médico {citaSeleccionada.NombreMedico} en el consultorio {citaSeleccionada.NombreConsultorio}.\n¿Desea continuar?", "Aceptar", "Regresar");
            if (continuar)
            {
                AgendarCitaSQL cancelarCita = new AgendarCitaSQL(citaSeleccionada);
                resultadoOperacion = await cancelarCita.CancelarCita();
            }
            else
            {
                //El paciente decidió no continuar con la cancelación
                return;
            }

            if (resultadoOperacion > 0)
            {
                await DisplayAlert("Éxito", "La cita se canceló correctamente.", "Aceptar");
                this.CargarCitasAgendadas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo cancelar la cita.", "Aceptar");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al intentar cancelar la cita: {ex.Message}", "Aceptar");
        }

        
    }
}