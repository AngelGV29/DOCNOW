using static Microsoft.Maui.ApplicationModel.Permissions;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using static System.Net.Mime.MediaTypeNames;
using DocNowApp.Globales;
using System.Globalization;

namespace DocNowApp.AgendaDisponibilidad;

public partial class AgregarFranjaPage : ContentPage
{
    private int idAgendaDisponibilidad = 0;
    private int idMedico;
    private int idConsultorio;

    private readonly TaskCompletionSource<FranjaDto> tareaModal = new TaskCompletionSource<FranjaDto>();

    // Expuesto públicamente para que quien abra la página pueda await modal.Resultado
    public Task<FranjaDto> Resultado => tareaModal.Task;
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
    public AgregarFranjaPage(FranjaDto _franjaDto)
    {
        InitializeComponent();
        this.pickerDuracionSlot.ItemsSource = DuracionSlot;
        this.pickerDia.ItemsSource = listaDias;
        this.idAgendaDisponibilidad = _franjaDto.IdAgendaDisponibilidad;
        this.idMedico = _franjaDto.IdMedico;
        this.idConsultorio = _franjaDto.IdConsultorio;
        this.pickerDia.SelectedIndex = _franjaDto.IdDia - 1;
        this.tpInicio.Time = _franjaDto.HoraInicioJornada;
        this.tpFin.Time = _franjaDto.HoraFinJornada;
        this.pickerDuracionSlot.SelectedItem = Convert.ToString(_franjaDto.DuracionSlotMinutos);
        this.switchActivo.IsToggled = _franjaDto.AgendaActiva;
        
    }

    public AgregarFranjaPage(int idMedico, int idConsultorio)
	{
		InitializeComponent();
        this.pickerDuracionSlot.ItemsSource = DuracionSlot;
        this.pickerDia.ItemsSource = listaDias;
        this.idMedico = idMedico;
        this.idConsultorio = idConsultorio;
    }

    List<string> listaDias = new List<string>
    {
        "Lunes",
        "Martes",
        "Miércoles",
        "Jueves",
        "Viernes",
        "Sábado",
        "Domingo"
    };

    List<string> DuracionSlot = new List<string>
    {
        "10",
        "15",
        "30",
        "45",
        "50",
        "60",
        "90"
    };

    private async void btnAceptar_Clicked(object sender, EventArgs e)
    {
        // Validaciones básicas
        if (this.pickerDia.SelectedIndex < 0)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar un día.", "Aceptar");
            return;
        }

        if (this.pickerDuracionSlot.SelectedIndex < 0)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar una duración para los turnos.", "Aceptar");
            return;
        }

        TimeSpan horaInicioJornada = tpInicio.Time;
        TimeSpan horaFinJornada = tpFin.Time;

        if (horaInicioJornada >= horaFinJornada)
        {
            await DisplayAlert("Advertencia", "La hora de inicio debe ser menor que la hora de fin.", "Aceptar");
            return;
        }

        // Chequeo extra: que quepan al menos un turno
        TimeSpan minutosDisponibles = (horaFinJornada - horaInicioJornada);
        TimeSpan duracionSlotMinutosTime = new TimeSpan(0, Convert.ToInt32(this.pickerDuracionSlot.SelectedItem.ToString()), 0);
        if (minutosDisponibles < duracionSlotMinutosTime)
        {
            await DisplayAlert("Advertencia", "La duración no cabe en el intervalo seleccionado.", "Aceptar");
            return;
        }

        // Creamos el DTO con los valores
        FranjaDto _franjaDto = new FranjaDto
        {
            IdAgendaDisponibilidad = this.idAgendaDisponibilidad,
            IdMedico = this.idMedico,
            IdConsultorio = this.idConsultorio,
            IdDia = this.pickerDia.SelectedIndex + 1,     // 0..6
            HoraInicioJornada = horaInicioJornada,
            HoraFinJornada = horaFinJornada,
            DuracionSlotMinutos = Convert.ToInt32(this.pickerDuracionSlot.SelectedItem.ToString()),
            AgendaActiva = switchActivo.IsToggled
        };

        AgendaDisponibilidadSQL agregarFranja = new AgendaDisponibilidadSQL(_franjaDto);
        FranjaDto? franjaChoque = await agregarFranja.ExisteChoque();
        if (franjaChoque != null)
        {
            BoolToActivoConversor conversor = new BoolToActivoConversor();
            string estado = (string)conversor.Convert(franjaChoque.AgendaActiva, typeof(string), null, CultureInfo.CurrentCulture);
            string mensajeChoque = $"Información de la franja que causó el choque:\nConsultorio '{franjaChoque.NombreConsultorio}' | {franjaChoque.NombreDia}" +
                $"{franjaChoque.RangoHorario}{franjaChoque.TextoDuracion}{estado}";
            await DisplayAlert("Error", $"La franja de horario que intenta agregar o editar choca con una ya existente.\n{mensajeChoque}", "Aceptar");
            return;
        }

        // Evitar setear resultado si ya se completó (doble click)
        this.tareaModal.TrySetResult(_franjaDto);

        // Cerrar modal
        await Navigation.PopModalAsync();
    }

    //Cuando se elige una duración para los turnos, los TimePicker se activan
    private void pickerDuracionSlot_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.pickerDuracionSlot.SelectedItem == null)
        {
            return;
        }
        this.tpInicio.IsEnabled = true;
        this.tpFin.IsEnabled = true;
        int duracionTurno = int.Parse(this.pickerDuracionSlot.SelectedItem.ToString());
        this.tpInicio.Time = AjustarHora(this.tpInicio.Time, duracionTurno);
        this.tpFin.Time = AjustarHora(this.tpFin.Time, duracionTurno);
    }

    //Eventos que llaman al método AjustarHora para ajustar la hora seleccionada a una que coincida con la duración de los turnos
    private void tpInicio_Unfocused(object sender, FocusEventArgs e)
    {
        if (this.tpInicio.Time == default)
            return;

        int duracionTurno = int.Parse(this.pickerDuracionSlot.SelectedItem.ToString());
        this.tpInicio.Time = AjustarHora(this.tpInicio.Time, duracionTurno);
    }

    private void tpFin_Unfocused(object sender, FocusEventArgs e)
    {
        if (this.tpFin.Time == default)
            return;

        int duracionTurno = int.Parse(this.pickerDuracionSlot.SelectedItem.ToString());
        this.tpFin.Time = AjustarHora(this.tpFin.Time, duracionTurno);
    }

    // Método que ajusta la hora seleccionada al múltiplo más cercano de la duración del turno
    private TimeSpan AjustarHora(TimeSpan horaSeleccionada, int duracionTurnoMinutos)
    {
        int totalMinutos = (int)horaSeleccionada.TotalMinutes;

        // Calcula el múltiplo más cercano
        int minutosAjustados = (int)Math.Round((double)totalMinutos / duracionTurnoMinutos) * duracionTurnoMinutos;

        // Asegura que no pase de 23:59
        if (minutosAjustados >= 24 * 60)
            minutosAjustados = (24 * 60) - duracionTurnoMinutos;

        return TimeSpan.FromMinutes(minutosAjustados);
    }

    private async void btnCancelar_Clicked(object sender, EventArgs e)
    {
        this.tareaModal.TrySetResult(null); // null indica que se canceló
        await Navigation.PopModalAsync();
    }

    // En caso de back hardware (Android), devolver null si no completó
    protected override void OnDisappearing()
    {
        if (!this.tareaModal.Task.IsCompleted)
        {
            this.tareaModal.TrySetResult(null);
        }
        base.OnDisappearing();
    }

   //Método no funcional para ajustar la hora instantaneamente (el programa se congelaba)
   /*private bool eventoEnEjecucion = true;
   private void TimePicker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
   {
        if (e.PropertyName == "Time")
        {
            TimePicker? timePicker = sender as TimePicker;
            if (timePicker == null || pickerDuracionSlot?.SelectedItem == null)
                return;

            int duracionTurnoMinutos = Convert.ToInt32(pickerDuracionSlot.SelectedItem.ToString());
            TimeSpan original = timePicker.Time;

            int minutosOriginal = (int)original.TotalMinutes;

            // Calcula el múltiplo más cercano
            int minutosAjustados = (int)Math.Round((double)minutosOriginal / duracionTurnoMinutos) * duracionTurnoMinutos;
            TimeSpan ajustada = TimeSpan.FromMinutes(minutosAjustados);

            // Solo actualiza si los minutos cambian realmente
            if (original.Minutes != ajustada.Minutes || original.Hours != ajustada.Hours)
            {
                //Previene bucle: desuscribe y suscribe
                timePicker.PropertyChanged -= TimePicker_PropertyChanged;
                timePicker.Time = ajustada;
                timePicker.PropertyChanged += TimePicker_PropertyChanged;
            }

            
            int duracionTurnoMinutos = Convert.ToInt32(pickerDuracionSlot.SelectedItem.ToString());
            TimeSpan horaSeleccionada = timePicker.Time;

            int minutosTotales = (int)horaSeleccionada.TotalMinutes;
            int minutosAjustados = (int)Math.Round((double)minutosTotales / duracionTurnoMinutos) * duracionTurnoMinutos;
            TimeSpan horaAjustada = TimeSpan.FromMinutes(minutosAjustados);

            // Solo actualiza si la diferencia es significativa
            if (Math.Abs((horaAjustada - horaSeleccionada).TotalMinutes) >= 1)
            {
                // Quitar handler antes de actualizar para evitar bucle
                timePicker.PropertyChanged -= TimePicker_PropertyChanged;
                timePicker.Time = horaAjustada;
                // Volver a agregar el handler
                timePicker.PropertyChanged += TimePicker_PropertyChanged;
            }
        }
        
        if (!eventoEnEjecucion)
            break;

        // Solo dispara si la propiedad cambiada es "Time"
        if (e.PropertyName == "Time")
        {
            TimePicker? timePicker = sender as TimePicker;
            if (timePicker == null || this.pickerDuracionSlot == null || this.pickerDuracionSlot.SelectedItem == null)
                return;

            int duracionTurnoMinutos = Convert.ToInt32(this.pickerDuracionSlot.SelectedItem.ToString());
            TimeSpan horaSeleccionada = timePicker.Time;

            int minutosTotales = (int)horaSeleccionada.TotalMinutes;
            int minutosAjustados = (int)Math.Round((double)minutosTotales / duracionTurnoMinutos) * duracionTurnoMinutos;
            TimeSpan horaAjustada = TimeSpan.FromMinutes(minutosAjustados);

            if (horaAjustada != horaSeleccionada)
            {
                eventoEnEjecucion = true;
                timePicker.Time = horaAjustada;
                eventoEnEjecucion = false;
            }
        }
    }*/

}