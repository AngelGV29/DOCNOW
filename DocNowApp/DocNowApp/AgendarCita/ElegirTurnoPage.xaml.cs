using DocNowApp.AgendaDisponibilidad;

namespace DocNowApp.AgendarCita;

public partial class ElegirTurnoPage : ContentPage
{
    private readonly MedicoConsultorioDto medicoConusltorioSeleccionado;
    private readonly DateTime fechaSeleccionada;
    private TurnoDto? TurnoSeleccionado;

    private readonly TaskCompletionSource<TurnoDto> tareaModal = new TaskCompletionSource<TurnoDto>();

    // Expuesto públicamente para que quien abra la página pueda await modal.Resultado
    public Task<TurnoDto> Resultado => tareaModal.Task;
    public ElegirTurnoPage(MedicoConsultorioDto medicoConsultorioSeleccionado, DateTime fechaSeleccionada, TurnoDto? TurnoSeleccionado)
	{
		InitializeComponent();
        this.medicoConusltorioSeleccionado = medicoConsultorioSeleccionado;
        this.fechaSeleccionada = fechaSeleccionada;
        this.TurnoSeleccionado = TurnoSeleccionado;
        CargarTurnos();
    }

    private async void CargarTurnos()
    {
        try
        {
            
            var listaTurnos = await GenerarTurnosDisponibles(medicoConusltorioSeleccionado, fechaSeleccionada);
            collectionTurnos.ItemsSource = listaTurnos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los turnos: {ex.Message}", "Aceptar");
        }
    }

    private async Task<List<TurnoDto>> GenerarTurnosDisponibles(MedicoConsultorioDto medico, DateTime fecha)
    {
        //Se consulta tu tabla AgendasDisponibilidad según el médico, consultorio y día
        //en que seagendó la cita para btener las franjas disponibles desde la BD
        AgendarCitaSQL _cargarTurnos = new AgendarCitaSQL(medicoConusltorioSeleccionado, fechaSeleccionada);
        List<FranjaDto> listaAgendas = await _cargarTurnos.ObtenerAgendasDisponibilidad();

        List<CitaDto> citasExistentes = await _cargarTurnos.ObtenerCitasExistentes();

        List<TurnoDto> listaTurnos = new List<TurnoDto>();

        //Generar los turnos por cada franja
        foreach (FranjaDto agenda in listaAgendas)
        {
            TimeSpan horaActual = agenda.HoraInicioJornada;
            while (horaActual < agenda.HoraFinJornada)
            {
                TimeSpan siguienteHora = horaActual.Add(TimeSpan.FromMinutes(agenda.DuracionSlotMinutos));

                if (siguienteHora > agenda.HoraFinJornada)
                    break;

                TurnoDto turno = new TurnoDto
                {
                    HoraInicio = horaActual,
                    HoraFin = siguienteHora,
                };
                
                bool solapa = citasExistentes.Any(cita =>
                    (turno.HoraInicio < cita.HoraFin) && (turno.HoraFin > cita.HoraInicio)
                );
                if (!solapa)
                {
                    listaTurnos.Add(turno);
                }

                horaActual = siguienteHora;
            }
        }

        return listaTurnos;
    }

    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        this.tareaModal.TrySetResult(this.TurnoSeleccionado);
        await Navigation.PopModalAsync();
    }

    private async void collectionTurnos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(this.collectionTurnos.SelectedItem == null)
        {
            return;
        }

        TurnoDto turnoHechoClick = (TurnoDto)this.collectionTurnos.SelectedItem;
        bool continuar = await DisplayAlert("Advertencia", $"El horario elegido fue de {turnoHechoClick.RangoHorario}.\n¿Desea continuar?", "Aceptar", "Cancelar");
        
        if (continuar)
        {
            this.TurnoSeleccionado = (TurnoDto)this.collectionTurnos.SelectedItem;
            this.tareaModal.TrySetResult(this.TurnoSeleccionado);
            await Navigation.PopModalAsync();
        }
        else
        {
            this.collectionTurnos.SelectedItem = null;
        }
    }
}