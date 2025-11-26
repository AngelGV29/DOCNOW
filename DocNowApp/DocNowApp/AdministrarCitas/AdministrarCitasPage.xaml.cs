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

    private void btnReagendar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnCancelar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnCompletar_Clicked(object sender, EventArgs e)
    {

    }

    private void btnMostrarNotas_Clicked(object sender, EventArgs e)
    {

    }
}
