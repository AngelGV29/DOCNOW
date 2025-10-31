using static Microsoft.Maui.ApplicationModel.Permissions;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

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
        var _franjaDto = new FranjaDto
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

        // Evitar setear resultado si ya se completó (doble click)
        this.tareaModal.TrySetResult(_franjaDto);

        // Cerrar modal
        await Navigation.PopModalAsync();
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
}