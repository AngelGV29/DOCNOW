using System.Collections.ObjectModel;
using System.Data;

namespace DocNowApp.AgendaDisponibilidad;

public partial class ConfigurarAgendaDisponibilidadPage : ContentPage
{
    private List<Consultorio> listaConsultorios;


    private int idHorarioMedico; // asigna según el horario que estés editando
    public ConfigurarAgendaDisponibilidadPage()
	{
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            AgendaDisponibilidadSQL loader = new AgendaDisponibilidadSQL(Globales.AdministradorDeSesion.idMedico);
            List<Consultorio> listaConsultorios = await loader.ObtenerConsultorios();
            this.pickerConsultorio.ItemsSource = listaConsultorios;
            this.pickerConsultorio.ItemDisplayBinding = new Binding("nombre");
            this.pickerConsultorio.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al cargar: {ex.Message}", "Aceptar");
        }
    }

    private void pickerConsultorio_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.CargarAgendaDisponibilidad();
    }

    private async void CargarAgendaDisponibilidad()
    {
        try
        {
            if (this.pickerConsultorio.SelectedItem is Consultorio seleccionado)
            {
                int idConsultorio = seleccionado.idConsultorio;
                AgendaDisponibilidadSQL cargarAgenda = new AgendaDisponibilidadSQL(Globales.AdministradorDeSesion.idMedico, idConsultorio);
                List<FranjaDto> listaAgendaDisponibilidad = await cargarAgenda.ObtenerAgendaDisponibilidad();
                if (listaAgendaDisponibilidad != null)
                {
                    this.collectionFranjas.ItemsSource = listaAgendaDisponibilidad;
                }

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al cargar la agenda de dispnibilidad: {ex.Message}", "Aceptar");
        }
        return;
    }

    private async void btnAniadir_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (this.pickerConsultorio.SelectedItem == null)
            {
                await DisplayAlert("Adevetencia", "Debe seleccionar un consultorio primero.", "Aceptar");
                return;
            }
            Consultorio seleccionado = (Consultorio)this.pickerConsultorio.SelectedItem;
            int idConsultorio = seleccionado.idConsultorio;
            AgregarFranjaPage modalNuevaFranja = new AgregarFranjaPage(Globales.AdministradorDeSesion.idMedico, idConsultorio);
            await Navigation.PushModalAsync(modalNuevaFranja);

            FranjaDto nuevaFranja = await modalNuevaFranja.Resultado;
            if (nuevaFranja == null)
            {
                return;
            }

            AgendaDisponibilidadSQL agregarFranja = new AgendaDisponibilidadSQL(nuevaFranja);
            bool existeChoque = await agregarFranja.ExisteChoque();
            if (existeChoque)
            {
                await DisplayAlert("Error", "La franja de horario que intenta agregar choca con una ya existente.", "Aceptar");
                return;
            }

            int resultado = await agregarFranja.AgregarNuevaAgendaDisponibilidad();
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "La nueva franja se agregó correctamente.", "Aceptar");
                this.CargarAgendaDisponibilidad();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar la nueva franja.", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al agregar la nueva franja: {ex.Message}", "Aceptar");
        }

    }

    private async void btnEditar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            FranjaDto franja = btn.CommandParameter as FranjaDto;
            if (franja == null)
            {
                return;
            }
            AgregarFranjaPage modalEditarFranja = new AgregarFranjaPage(franja);
            await Navigation.PushModalAsync(modalEditarFranja);
            
            franja = await modalEditarFranja.Resultado;
            if (franja == null)
            {
                return;
            }
            
            AgendaDisponibilidadSQL modificarFranja = new AgendaDisponibilidadSQL(franja);
            bool existeChoque = await modificarFranja.ExisteChoque();
            if (existeChoque)
            {
                await DisplayAlert("Error", "La franja de horario que intenta modificar choca con una ya existente.", "Aceptar");
                return;
            }
            int resultado = await modificarFranja.ModificarAgendaDisponibilidad();
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "La franja se modificó correctamente.", "Aceptar");
                this.CargarAgendaDisponibilidad();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar la franja.", "Aceptar");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al editar la franja: {ex.Message}", "Aceptar");
        }
    }

    private void btnEliminar_Clicked(object sender, EventArgs e)
    {

    }
}