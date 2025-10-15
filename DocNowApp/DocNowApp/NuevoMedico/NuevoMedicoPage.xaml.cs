using System.Data;

namespace DocNowApp.NuevoMedico;

public partial class NuevoMedicoPage : ContentPage
{
	public NuevoMedicoPage()
	{
		InitializeComponent();
        
    }

    //Método que se ejecuta al aparecer la página
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //Reinicia los controles de la página
        this.txtNumCedula.Text = "";
        this.pickerEspecialidad.Items.Clear();
        this.CollectionConsultorios.ItemsSource = null;
        this.txtNumCedula.Focus();
        try
        {
            //Obtiene las especialidades y consultorios disponibles en la base de datos
            NuevoMedico.NuevoMedicoSQL disponibles = new NuevoMedico.NuevoMedicoSQL();
            List<Especialidad> listaEspecialidades = await disponibles.ObtenerEspecialidades();
            List<Consultorio> listaConsultorios = await disponibles.ObtenerConsultorios();

            //Pasa las especialidades disponibles a un picker para su selección
            foreach (Especialidad especialidad in listaEspecialidades)
            {
                this.pickerEspecialidad.Items.Add(especialidad.especialidad);
            }

            //Se asigna la lista de consultorios obtenida de la base de datos como origne de datos del CollectionView
            //El CollectionView muestra los consultorios disponibles, el médico o admin selecciona en cuales trabaja el médico
            this.CollectionConsultorios.ItemsSource = listaConsultorios;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepción al cargar: {ex.Message}", "OK");
        }
    }

    

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        //Obtiene la lista de consultorios seleccionados en el CollectionView
        var consultoriosSeleccionados = this.CollectionConsultorios.SelectedItems.Cast<Consultorio>().ToList();
        //Si la cedula profesional está vacía, el usuario no podrá continuar
        if (string.IsNullOrWhiteSpace(this.txtNumCedula.Text))
        {
            await DisplayAlert("Advertencia", "Debe ingresar la cédula profesional", "Aceptar");
            return;
        }
        //Si no se selecciono una especialidad en el picker, el usuario no podrá continuar
        if(this.pickerEspecialidad.SelectedItem == null)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar una especialidad para continuar", "Aceptar");
            return;
        }
        //Si no se selecciono al menos un consultorio, el usuario no podrá continuar
        if (consultoriosSeleccionados.Count == 0)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar al menos un consultorio para continuar", "Aceptar");
            return;
        }

        try
        {
            //Crea el objeto para la creación del nuevo médico
            NuevoMedico.NuevoMedicoSQL crear = new NuevoMedico.NuevoMedicoSQL(this.txtNumCedula.Text, this.pickerEspecialidad.SelectedItem.ToString());
            //Método para crear el médico, si se afecto más de una fila la creación fue exitosa
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                //Método que devuelve la información del médico recien registrado para obtener su ID de médico y pasarlo al administrador de Sesión
                DataSet datos = await crear.ObtenerIdMedico();
                Globales.AdministradorDeSesion.idMedico = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idMedico"].ToString());
                //Método que le asigan el rol de Médico al usuario que se está creando
                await crear.AsignacionRol();
                await DisplayAlert("Éxito", "Ha sido registrado como un nuevo médico. Asegurese de mantener su información personal siempre actualizada en la pestaña PENDIENTE", "Aceptar");
                
                //Por cada consultorio seleccionado, se inserta una fila en la tabla MedicoConsultorio
                foreach (Consultorio consultorio in consultoriosSeleccionados)
                {
                    resultado = await crear.VinculacionMedicoConsultorio(consultorio.idConsultorio);
                    //Si en algún momento ocurre una excepción, el proceso se detiene y se muestra un mensaje de error
                    if (resultado == -1)
                    {
                        await DisplayAlert("Error", "Ocurrio un error al asignar uno o varios consultorios al médico. Vuelva a intentarlo más tarde", "Aceptar");
                        break;
                    }
                }
                //Tras haber creado el médico, el administrador regresa a su pantalla principal
                await Shell.Current.GoToAsync("//AdminPrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creación de nuevo médico fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepción", $"Error: {ex.Message}", "Aceptar");
        }
    }
}