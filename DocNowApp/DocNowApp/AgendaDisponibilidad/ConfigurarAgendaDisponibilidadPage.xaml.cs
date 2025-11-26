using System.Collections.ObjectModel;
using System.Data;

namespace DocNowApp.AgendaDisponibilidad;

public partial class ConfigurarAgendaDisponibilidadPage : ContentPage
{
    private List<Consultorio> listaConsultorios;


   // private int idHorarioMedico; // asigna según el horario que estés editando
    public ConfigurarAgendaDisponibilidadPage()
	{
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            //Llama al método para cargar las agendas de disponibilidad del médico
            this.CargarAgendaDisponibilidad();
            //Crea el objeto que ejecutará el método para guardar los consultorios con los que el médico está vinculado en una lista
            AgendaDisponibilidadSQL loader = new AgendaDisponibilidadSQL(Globales.AdministradorDeSesion.idMedico);
            List<Consultorio> listaConsultorios = await loader.ObtenerConsultorios();
            //La lista se utiliza como fuente de datos para el picker de consultorios (solo muestra el nombre del consultorio)
            this.pickerConsultorio.ItemsSource = listaConsultorios;
            this.pickerConsultorio.ItemDisplayBinding = new Binding("nombre");
            this.pickerConsultorio.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al cargar: {ex.Message}", "Aceptar");
        }
    }

    //Cada vez que se seleeciona un consultorio, se recargan las agendas disponibilidad
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
            FranjaDto? franja = btn.CommandParameter as FranjaDto;
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

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        try
        {
            var btn = (Button)sender;
            FranjaDto? franja = btn.CommandParameter as FranjaDto;
            if (franja == null)
            {
                return;
            }
            int resultadoOperacion;
            bool continuar = await DisplayAlert("Advertencia", "¿Está seguro de que desea eliminar esta franja de disponibilidad?\n" +
                "Recuerde que también puede desactivarla en el modo edición.", "Aceptar", "Cancelar");

            if (continuar)
            {
                AgendaDisponibilidadSQL eliminarFranja = new AgendaDisponibilidadSQL(franja);
                resultadoOperacion = await eliminarFranja.EliminarAgendaDisponibilidad();
            }
            else
            {
                //El médico decidio cancelar la eliminación
                return;
            }

            AgendaDisponibilidadSQL modificarFranja = new AgendaDisponibilidadSQL(franja);
            int resultado = await modificarFranja.ModificarAgendaDisponibilidad();
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "La franja se eliminó correctamente.", "Aceptar");
                this.CargarAgendaDisponibilidad();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar la franja.", "Aceptar");
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al editar la franja: {ex.Message}", "Aceptar");
        }
    }
}