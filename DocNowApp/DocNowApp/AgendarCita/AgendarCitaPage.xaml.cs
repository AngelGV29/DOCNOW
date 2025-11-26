using DocNowApp.AgendaDisponibilidad;
using System.Collections.ObjectModel;

namespace DocNowApp.AgendarCita;

public partial class AgendarCitaPage : ContentPage
{
    private CancellationTokenSource? _cts;
    private CitaDto citaAgendada;
    private List<MedicoConsultorioDto> listaMedicoConsultorio;
    private List<CitaDto> listaMotivoConsulta;
    private ObservableCollection<MedicoConsultorioDto> listaFiltrada = new(); // lista filtrada
    private TurnoDto? turnoSeleccionado;
    private bool esNuevaCita;
    private bool aceptarReinicio;
    public AgendarCitaPage()
	{
		InitializeComponent();
        listaFiltrada = new ObservableCollection<MedicoConsultorioDto>();
        this.collectionMedicoConsultorio.ItemsSource = listaFiltrada;
        this.turnoSeleccionado = null;
        aceptarReinicio = true;
        esNuevaCita = true;
	}

    public AgendarCitaPage(CitaDto citaActual)
    {
        InitializeComponent();
        citaAgendada = citaActual;
        listaFiltrada = new ObservableCollection<MedicoConsultorioDto>();
        this.collectionMedicoConsultorio.ItemsSource = listaFiltrada;
        aceptarReinicio = false;
        esNuevaCita = false;
        CargarCitaActual(citaAgendada);
    }

    public async void CargarCitaActual(CitaDto citaActual)
    {
        try
        {
            //Carga los medico y consultorio disponibles
            await this.CargarMedicoConsultorioDisponibles();
            //Autoselecciona el medico y consultorio de la cita
            var medConSeleccionado = listaFiltrada.FirstOrDefault(medCon =>
                medCon.IdMedico == citaActual.IdMedico &&
                medCon.IdConsultorio == citaActual.IdConsultorio);
            this.collectionMedicoConsultorio.SelectedItem = medConSeleccionado;

            //Carga los motivos de consulta disponibles para el medico y consultorio seleccionados
            AgendarCitaSQL cargarMotivosConsulta = new AgendarCitaSQL(citaActual.IdMedico, citaActual.IdConsultorio);
            listaMotivoConsulta = await cargarMotivosConsulta.ObtenerMotivosConsulta();
            this.pickerMotivoConsulta.ItemsSource = listaMotivoConsulta;
            //Autoselecciona el motivo de consulta
            var motConSeleccionado = listaMotivoConsulta.FirstOrDefault(motCon =>
                motCon.IdMotivo == citaActual.IdMotivo);
            this.pickerMotivoConsulta.SelectedItem = motConSeleccionado;

            //Autoselecciona la fecha en que se agendó la cita
            this.dateFechaCita.Date = citaActual.FechaCita;

            //Crea un objeto de tipo turno para guardar su información
            turnoSeleccionado = new TurnoDto
            {
                HoraInicio = citaActual.HoraInicio,
                HoraFin = citaActual.HoraFin,
            };
            //Muestra el turno de la cita
            this.lblTurnoSeleccionado.Text = turnoSeleccionado.RangoHorario;

            //Desactiva todos los controles para permitir únicamente cambiar el día y hora de la cita
            this.sbBarraDeBusqueda.IsEnabled = false;
            this.pickerFiltro.IsEnabled = false;
            this.collectionMedicoConsultorio.IsEnabled = false;
            this.btnAceptar.IsEnabled = false;
            this.btnCancelar.IsEnabled = false;
            this.pickerMotivoConsulta.IsEnabled = false;
            this.dateFechaCita.IsEnabled = true;
            this.btnElegirHorario.IsEnabled = true;
            this.btnAgendarCita.IsEnabled = true;

            ValidarFechaSeleccionada(this.dateFechaCita.Date);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar la cita actual: {ex.Message}", "Aceptar");
        }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        this.pickerFiltro.ItemsSource = filtros;
        try
        {
            this.dateFechaCita.MinimumDate = DateTime.Today;
            this.ReiniciarControles();
            this.aceptarReinicio = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar los médicos y consultorios: {ex.Message}", "Aceptar");
        }
    }

    List<string> filtros = new List<string>
    {
        "Médico",
        "Consultorio",
        "Especialidad",
        "Día laborado",
    };

    private async Task CargarMedicoConsultorioDisponibles()
    {
        try
        {
            AgendarCitaSQL cargarMedicoConsultorio = new AgendarCitaSQL(Globales.AdministradorDeSesion.idPaciente);
            listaMedicoConsultorio = await cargarMedicoConsultorio.ObtenerMedicoConsultorios();

            listaFiltrada.Clear();
            if (listaMedicoConsultorio != null && listaMedicoConsultorio.Any())
            {
                foreach (MedicoConsultorioDto mc in listaMedicoConsultorio)
                {
                    listaFiltrada.Add(mc);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar la los Medicos y Consultorios disponibles: {ex.Message}", "Aceptar");
        }
        return;
    }

    private void collectionMedicoConsultorio_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }

    private async void btnAceptar_Clicked(object sender, EventArgs e)
    {
        if(this.collectionMedicoConsultorio.SelectedItem == null || this.collectionMedicoConsultorio.SelectedItem is not MedicoConsultorioDto seleccionado)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar un médico y consultorio para continuar.", "Aceptar");
            return;
        }
        this.sbBarraDeBusqueda.IsEnabled = false;
        this.pickerFiltro.IsEnabled = false;
        this.collectionMedicoConsultorio.IsEnabled = false;
        this.btnAceptar.IsEnabled = false;

        this.btnCancelar.IsEnabled = true;
        this.pickerMotivoConsulta.IsEnabled = true;
        this.dateFechaCita.IsEnabled = true;
        this.btnElegirHorario.IsEnabled = true;

        AgendarCitaSQL cargarMotivosConsulta = new AgendarCitaSQL(seleccionado.IdMedico, seleccionado.IdConsultorio);
        listaMotivoConsulta = await cargarMotivosConsulta.ObtenerMotivosConsulta();
        this.pickerMotivoConsulta.ItemsSource = listaMotivoConsulta;

        ValidarFechaSeleccionada(this.dateFechaCita.Date);
    }

    private void btnCancelar_Clicked(object sender, EventArgs e)
    {
        this.ReiniciarControles();
    }

    private async void ReiniciarControles()
    {
        if (!aceptarReinicio)
        {
            return;
        }
        //Llama al método para cargar los medicos y consultorios disponibles para agendar una cita
        await this.CargarMedicoConsultorioDisponibles();

        this.pickerMotivoConsulta.IsEnabled = false;
        this.dateFechaCita.IsEnabled = false;
        this.btnElegirHorario.IsEnabled = false;
        this.btnCancelar.IsEnabled = false;
        this.btnAgendarCita.IsEnabled = false;

        this.sbBarraDeBusqueda.IsEnabled = true;
        this.pickerFiltro.IsEnabled = true;
        this.collectionMedicoConsultorio.IsEnabled = true;
        this.btnAceptar.IsEnabled = true;

        this.sbBarraDeBusqueda.Text = string.Empty;
        this.pickerFiltro.SelectedIndex = 0;
        this.dateFechaCita.DateSelected -= dateFechaCita_DateSelected;
        this.dateFechaCita.Date = DateTime.Today;
        this.dateFechaCita.DateSelected += dateFechaCita_DateSelected;
        this.pickerMotivoConsulta.ItemsSource = null;
        this.turnoSeleccionado = null;
        this.lblTurnoSeleccionado.Text = "Horario no seleccionado";
    }

    private async void btnElegirHorario_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (this.collectionMedicoConsultorio.SelectedItem is not MedicoConsultorioDto medicoConsultorioSeleccionado)
            {
                await DisplayAlert("Advertencia", "No ha seleccionado ningún Médico ni Consultorio.", "Aceptar");
                return;
            }

            if (this.pickerMotivoConsulta.SelectedIndex == -1)
            {
                await DisplayAlert("Advertencia", "No ha seleccionado el motivo de su consulta.", "Aceptar");
                return;
            }

            aceptarReinicio = false;
            DateTime fechaSeleccionada = this.dateFechaCita.Date;
            ElegirTurnoPage modalElegirTurno = new ElegirTurnoPage(medicoConsultorioSeleccionado, fechaSeleccionada, this.turnoSeleccionado);
            await Navigation.PushModalAsync(modalElegirTurno);
            
            this.turnoSeleccionado = await modalElegirTurno.Resultado;
            if (this.turnoSeleccionado == null)
            {
                return;
            }

            //Se muestra el turno seleccionado en el .xaml
            this.lblTurnoSeleccionado.Text = this.turnoSeleccionado.RangoHorario;
            this.btnAgendarCita.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al elegir el turno: {ex.Message}", "Aceptar");
        }
    }

    private async void btnAgendarCita_Clicked(object sender, EventArgs e)
    {
        int resultadoOperacion;
        if (esNuevaCita)
        {
            citaAgendada = new CitaDto
            {
                IdPaciente = Globales.AdministradorDeSesion.idPaciente,
                IdMedico = ((MedicoConsultorioDto)this.collectionMedicoConsultorio.SelectedItem).IdMedico,
                IdConsultorio = ((MedicoConsultorioDto)this.collectionMedicoConsultorio.SelectedItem).IdConsultorio,
                IdMotivo = ((CitaDto)this.pickerMotivoConsulta.SelectedItem).IdMotivo,
                FechaCita = this.dateFechaCita.Date,
                HoraInicio = this.turnoSeleccionado.HoraInicio,
                HoraFin = this.turnoSeleccionado.HoraFin,
                EstadoCita = "AGENDADA",
            };
            AgendarCitaSQL agendarCita = new AgendarCitaSQL(citaAgendada);
            resultadoOperacion = await agendarCita.AgendarNuevaCita();
        }
        else
        {
            citaAgendada.FechaCita = this.dateFechaCita.Date;
            citaAgendada.HoraInicio = this.turnoSeleccionado.HoraInicio;
            citaAgendada.HoraFin = this.turnoSeleccionado.HoraFin;
            citaAgendada.EstadoCita = "REAGENDADA";
            AgendarCitaSQL agendarCita = new AgendarCitaSQL(citaAgendada);
            resultadoOperacion = await agendarCita.ReagendarCita();
        }
        
        if (resultadoOperacion > 0)
        {
            await DisplayAlert("Éxito", $"La cita se agendó correctamente.\n ATENCIÓN: {citaAgendada.InstruccionMotivo}", "Aceptar");
            //await Shell.Current.GoToAsync("//PacientePrincipalPage");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Éxito", $"Ocurrio un error al intentar guardar la cita agendada.","Aceptar");
        }
    }

    // ---------------------------
    // Evento del SearchBar
    // ---------------------------
    private async void sbBarraDeBusqueda_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Cancela el debounce anterior si el usuario sigue escribiendo
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            // Espera 200ms; si el usuario vuelve a escribir, se cancela
            await Task.Delay(200, token);

            // Aplica el filtro con el texto actual
            AplicarFiltro(e.NewTextValue ?? "");
        }
        catch (TaskCanceledException)
        {
            // Cancelación esperada, no hacer nada
        }
    }

    // ---------------------------
    // Evento del Picker de filtro
    // ---------------------------
    private void pickerFiltro_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Cuando cambia la opción (Médico / Consultorio / Día), re-aplicar el filtro
        AplicarFiltro(this.sbBarraDeBusqueda.Text ?? "");
    }

    // ---------------------------
    // Función central de filtrado
    // ---------------------------
    private void AplicarFiltro(string textoBusqueda)
    {
        string term = (textoBusqueda ?? "").Trim().ToLowerInvariant();
        IEnumerable<MedicoConsultorioDto> resultado = listaMedicoConsultorio;

        if (!string.IsNullOrEmpty(term))
        {
            int idx = this.pickerFiltro.SelectedIndex; // 0 = Médico, 1 = Consultorio, 2 = Especialidad, 3 = Día laborado
            switch (idx)
            {
                case 0: // Médico
                    resultado = listaMedicoConsultorio.Where(x =>
                        (x.NombreMedico ?? "").ToLowerInvariant().Contains(term));
                    break;

                case 1: // Consultorio
                    resultado = listaMedicoConsultorio.Where(x =>
                        (x.NombreConsultorio ?? "").ToLowerInvariant().Contains(term));
                    break;

                case 2: // Especialidad
                    resultado = listaMedicoConsultorio.Where(x =>
                        (x.NombreEspecialidad ?? "").ToLowerInvariant().Contains(term));
                    break;

                case 3: // Día laborado
                    int diaNum = ParseDiaDesdeTexto(term);
                    if (diaNum > 0)
                        resultado = listaMedicoConsultorio.Where(x => x.Dias.Contains(diaNum));
                    else
                        resultado = listaMedicoConsultorio.Where(x =>
                            x.DiasTexto.ToLowerInvariant().Contains(term));
                    break;

                default:
                    resultado = listaMedicoConsultorio;
                    break;
            }
        }

        // Actualiza la lista observable (UI)
        listaFiltrada.Clear();
        foreach (var it in resultado)
            listaFiltrada.Add(it);
    }

    // ---------------------------
    // Helper: interpreta texto de día
    // ---------------------------
    private static int ParseDiaDesdeTexto(string term)
    {
        term = term.Trim().ToLowerInvariant();
        if (int.TryParse(term, out int num) && num >= 1 && num <= 7)
            return num;

        if (term.StartsWith("lun")) return 1;
        if (term.StartsWith("mar")) return 2;
        if (term.StartsWith("mié") || term.StartsWith("mie")) return 3;
        if (term.StartsWith("jue")) return 4;
        if (term.StartsWith("vie")) return 5;
        if (term.StartsWith("sáb") || term.StartsWith("sab")) return 6;
        if (term.StartsWith("dom")) return 7;

        return 0; // no se pudo interpretar
    }

    private void dateFechaCita_DateSelected(object sender, DateChangedEventArgs e)
    {
        DateTime fechaAnterior = e.OldDate;
        DateTime fechaNueva = e.NewDate;
        ValidarFechaSeleccionada(fechaNueva);
        if (this.dateFechaCita.Date.DayOfWeek != fechaAnterior.DayOfWeek)
        {
            this.turnoSeleccionado = null;
            this.lblTurnoSeleccionado.Text = "Horario no seleccionado";
            this.btnAgendarCita.IsEnabled = false;
        }

    }

    private void ValidarFechaSeleccionada(DateTime fechaSeleccionada)
    {
        // 1. Obtener el médico consultorio seleccionado
        if (this.collectionMedicoConsultorio.SelectedItem is not MedicoConsultorioDto medicoSeleccionado)
        {
            DisplayAlert("Advertencia", "Seleccione primero un médico y consultorio.", "Aceptar");
            return;
        }

        // 2. Obtener la lista de días válidos
        List<int> diasValidos = medicoSeleccionado.Dias;
        if (diasValidos == null || diasValidos.Count == 0)
        {
            DisplayAlert("advertencia", "El médico no tiene días disponibles.", "Aceptar");
            return;
        }

        // 3. Determinar el día de la semana del DatePicker (1 = Lunes … 7 = Domingo)
        int diaSeleccionado = (int)fechaSeleccionada.DayOfWeek;
        // En .NET, Sunday=0, Monday=1... por eso ajustamos
        if (diaSeleccionado == 0) diaSeleccionado = 7;

        // 4. Si el día no es válido, buscar el siguiente disponible
        if (!diasValidos.Contains(diaSeleccionado))
        {
            DateTime nuevaFecha = BuscarSiguienteDiaDisponible(fechaSeleccionada, diasValidos);

            DisplayAlert("Aviso",
                $"El médico no atiende el día {NombreDia(diaSeleccionado)}. " +
                $"La fecha se ajustó automáticamente al {nuevaFecha:dddd dd 'de' MMMM 'de' yyyy}.",
                "Aceptar");

            this.dateFechaCita.Date = nuevaFecha;
        }
    }

    // -------------------------------
    // Función auxiliar para buscar el siguiente día disponible
    // -------------------------------
    private DateTime BuscarSiguienteDiaDisponible(DateTime fechaActual, List<int> diasValidos)
    {
        for (int i = 1; i <= 7; i++)
        {
            DateTime siguiente = fechaActual.AddDays(i);
            int diaNum = (int)siguiente.DayOfWeek;
            if (diaNum == 0) diaNum = 7;
            if (diasValidos.Contains(diaNum))
                return siguiente;
        }
        return fechaActual; // fallback (si no encontró nada)
    }

    // -------------------------------
    // Función auxiliar para nombre del día (para mostrar mensajes)
    // -------------------------------
    private static string NombreDia(int d) => d switch
    {
        1 => "Lunes",
        2 => "Martes",
        3 => "Miércoles",
        4 => "Jueves",
        5 => "Viernes",
        6 => "Sábado",
        7 => "Domingo",
        _ => "Día desconocido"
    };

    
}