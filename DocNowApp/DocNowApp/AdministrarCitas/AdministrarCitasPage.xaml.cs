using DocNowApp.AdministrarCitas;
using DocNowApp.AgendarCita;
using System.Collections.ObjectModel;

namespace DocNowApp.MedicoPages;

public partial class AdministrarCitasPage : ContentPage
{
    private CancellationTokenSource? _cts;
    private List<CitaDto> listaCitasAgendadas = new List<CitaDto>();
    private ObservableCollection<CitaDto> listaFiltrada = new();
    public AdministrarCitasPage()
    {
        InitializeComponent();
        this.collectionCitas.ItemsSource = listaFiltrada;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            //Llama al método para cargar las citas del paciente
            this.CargarCitasAgendadas();
            AdministrarCitasSQL cargarPickers = new AdministrarCitasSQL(Globales.AdministradorDeSesion.idMedico);
            List<string> listaConsultorios = await cargarPickers.ObtenerConsultorios();
            listaConsultorios.Insert(0, "TODOS");
            this.pickerConsultorio.ItemsSource = listaConsultorios;
            this.pickerConsultorio.SelectedIndex = 0;

            this.pickerEstadoCita.ItemsSource = listaEstadosCita;
            this.pickerEstadoCita.SelectedIndex = 0;

            List<string> listaMotivosConsulta = await cargarPickers.ObtenerMotivosConsulta();
            listaMotivosConsulta.Insert(0, "TODOS");
            this.pickerMotivoConsulta.ItemsSource = listaMotivosConsulta;
            this.pickerMotivoConsulta.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar algunos de los controles: {ex.Message}", "Aceptar");
        }
    }

    List<string> listaEstadosCita = new List<string>
    {
        "TODOS",
        "AGENDADA",
        "REAGENDADA",
        "CANCELADA",
        "COMPLETADA"
    };

    private async void CargarCitasAgendadas()
    {
        try
        {
            AdministrarCitasSQL cargarCitas = new AdministrarCitasSQL(Globales.AdministradorDeSesion.idMedico);
            listaCitasAgendadas = await cargarCitas.ObtenerCitasAgendadas();

            listaFiltrada.Clear();
            if (listaCitasAgendadas != null && listaCitasAgendadas.Any())
            {
                foreach (CitaDto cita in listaCitasAgendadas)
                {
                    listaFiltrada.Add(cita);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al intentar cargar sus citas: {ex.Message}", "Aceptar");
        }
        return;
    }

    private async void btnCompletar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            CitaDto? citaSeleccionada = btn.CommandParameter as CitaDto;
            if (citaSeleccionada == null)
            {
                return;
            }
            if (citaSeleccionada.EstadoCita == "CANCELADA" || citaSeleccionada.EstadoCita == "COMPLETADA")
            {
                await DisplayAlert("Advertencia", "No se puede cancelar una cita que ya ha sido completada o cancelada.", "Aceptar");
                return;
            }

            bool continuar = await DisplayAlert("Advertencia", $"Está por marcar como completada la cita del paciente {citaSeleccionada.NombrePaciente} agendada el día {citaSeleccionada.FechaCita:dd/MM/yyyy} a las {citaSeleccionada.HoraInicio:hh\\:mm} " +
                $"con usted en el consultorio {citaSeleccionada.NombreConsultorio}.\n¿Desea continuar?", "Aceptar", "Regresar");
            if (!continuar)
            {
                return;
            }
            int opcion = 1; //Representa la acción que se cometerá con la cita: 1 para completar y 2 para cancelar
            AgregarNotasPage modalAgregarNotas = new AgregarNotasPage(opcion);
            await Navigation.PushModalAsync(modalAgregarNotas);

            string notas = await modalAgregarNotas.Resultado;
            if (notas == null)
            {
                //El médico canceló el agregar notas y por tanto el proceso de eliminación
                await DisplayAlert("Advertencia", "Se canceló el proceso para marcar la cita como completada.", "Aceptar");
                return;
            }
            citaSeleccionada.Notas = notas;
            AdministrarCitasSQL cancelarCita = new AdministrarCitasSQL(citaSeleccionada);
            int resultadoOperacion;
            resultadoOperacion = await cancelarCita.CompletarCita();

            if (resultadoOperacion > 0)
            {
                await DisplayAlert("Éxito", "La cita se marco como completada correctamente.", "Aceptar");
                this.CargarCitasAgendadas();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo marcar como completada la cita.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al intentar marcar la cita como completada: {ex.Message}", "Aceptar");
        }
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
            if (citaSeleccionada.EstadoCita == "CANCELADA" || citaSeleccionada.EstadoCita == "COMPLETADA")
            {
                await DisplayAlert("Advertencia", "No se puede reagendar una cita que ya ha sido completada o cancelada.", "Aceptar");
                return;
            }

            bool continuar = await DisplayAlert("Advertencia", $"Está por reagendar la cita del paciente {citaSeleccionada.NombrePaciente} agendada el día {citaSeleccionada.FechaCita:dd/MM/yyyy} a las {citaSeleccionada.HoraInicio:hh\\:mm} " +
                $"con usted en el consultorio {citaSeleccionada.NombreConsultorio}.\n¿Desea continuar?", "Aceptar", "Regresar");
            if (!continuar)
            {
                return;
            }
            int opcion = 1; //Representa la acción que se cometerá con la cita: 1 para completar y 2 para cancelar
            AgendarCitaPage reagendarCita = new AgendarCitaPage(citaSeleccionada);
            await Navigation.PushAsync(reagendarCita);

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
            if (citaSeleccionada.EstadoCita == "CANCELADA" || citaSeleccionada.EstadoCita == "COMPLETADA")
            {
                await DisplayAlert("Advertencia", "No se puede cancelar una cita que ya ha sido completada o cancelada.", "Aceptar");
                return;
            }

            bool continuar = await DisplayAlert("Advertencia", $"Está por cancelar la cita del paciente {citaSeleccionada.NombrePaciente} agendada el día {citaSeleccionada.FechaCita:dd/MM/yyyy} a las {citaSeleccionada.HoraInicio:hh\\:mm} " +
                $"con usted en el consultorio {citaSeleccionada.NombreConsultorio}.\n¿Desea continuar?", "Aceptar", "Regresar");
            if (!continuar)
            {
                return;
            }
            int opcion = 2; //Representa la acción que se cometerá con la cita: 1 para completar y 2 para cancelar
            AgregarNotasPage modalAgregarNotas = new AgregarNotasPage(opcion);
            await Navigation.PushModalAsync(modalAgregarNotas);

            string notas = await modalAgregarNotas.Resultado;
            if (notas == null)
            {
                //El médico canceló el agregar notas y por tanto el proceso de eliminación
                await DisplayAlert("Advertencia", "Se canceló el proceso de cancelación de la cita.", "Aceptar");
                return;
            }
            citaSeleccionada.Notas = notas;
            AdministrarCitasSQL cancelarCita = new AdministrarCitasSQL(citaSeleccionada);
            int resultadoOperacion;
            resultadoOperacion = await cancelarCita.CancelarCita();

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

    private async void btnMostrarNotas_Clicked(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        CitaDto? citaSeleccionada = btn.CommandParameter as CitaDto;
        if (citaSeleccionada == null)
        {
            return;
        }
        
        await DisplayAlert("Notas de la cita", $"Paciente: {citaSeleccionada.NombrePaciente}\n" +
            $"Fecha de la cita: {citaSeleccionada.FechaCita}\n" +
            $"Hora de la cita: {citaSeleccionada.HoraInicio:hh\\:mm} - {citaSeleccionada.HoraFin:hh\\:mm}" +
            $"\n\nNotas de la cita:\n" +
            $"{citaSeleccionada.Notas}", "Aceptar");
    }

    private async void sbBuscarPaciente_TextChanged(object sender, TextChangedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            await Task.Delay(200, token); // debounce 200ms
            if (token.IsCancellationRequested) return;
            ApplyFilter(e.NewTextValue ?? "");
        }
        catch (TaskCanceledException) { }
    }

    // ---------------------------
    // Pickers: cuando el usuario cambia una opción
    // ---------------------------
    private void Pickers_SelectedIndexChanged(object sender, EventArgs e)
    {
        ApplyFilter(sbBuscarPaciente.Text ?? "");
    }

    // ---------------------------
    // Función central de filtrado
    // ---------------------------
    private void ApplyFilter(string textoBusqueda)
    {
        // protección
        if (listaCitasAgendadas == null) return;

        string term = (textoBusqueda ?? "").Trim().ToLowerInvariant();

        IEnumerable<CitaDto> resultado = listaCitasAgendadas;

        // filtro texto sobre nombre paciente
        if (!string.IsNullOrEmpty(term))
        {
            resultado = resultado.Where(x => (x.NombrePaciente ?? "").ToLowerInvariant().Contains(term));
        }

        // filtro consultorio
        if (this.pickerConsultorio.SelectedItem is string consultorioSel && consultorioSel != "TODOS")
        {
            resultado = resultado.Where(x => (x.NombreConsultorio ?? "") == consultorioSel);
        }

        // filtro estado
        if (this.pickerEstadoCita.SelectedItem is string estadoSel && estadoSel != "TODOS")
        {
            resultado = resultado.Where(x => (x.EstadoCita ?? "") == estadoSel);
        }

        // filtro motivo
        if (pickerMotivoConsulta.SelectedItem is string motivoSel && motivoSel != "TODOS")
        {
            resultado = resultado.Where(x => (x.DescripcionMotivo ?? "") == motivoSel);
        }

        // Actualizar ObservableCollection de forma eficiente
        listaFiltrada.Clear();
        foreach (var it in resultado)
            listaFiltrada.Add(it);
    }
}
