using System.Data;

namespace DocNowApp.NuevoMedico;

public partial class NuevoMedicoPage : ContentPage
{
	public NuevoMedicoPage()
	{
		InitializeComponent();
        
    }

    //M�todo que se ejecuta al aparecer la p�gina
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        //Reinicia los controles de la p�gina
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

            //Pasa las especialidades disponibles a un picker para su selecci�n
            foreach (Especialidad especialidad in listaEspecialidades)
            {
                this.pickerEspecialidad.Items.Add(especialidad.especialidad);
            }

            //Se asigna la lista de consultorios obtenida de la base de datos como origne de datos del CollectionView
            //El CollectionView muestra los consultorios disponibles, el m�dico o admin selecciona en cuales trabaja el m�dico
            this.CollectionConsultorios.ItemsSource = listaConsultorios;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Excepci�n al cargar: {ex.Message}", "OK");
        }
    }

    

    private async void btnContinuar_Clicked(object sender, EventArgs e)
    {
        //Obtiene la lista de consultorios seleccionados en el CollectionView
        var consultoriosSeleccionados = this.CollectionConsultorios.SelectedItems.Cast<Consultorio>().ToList();
        //Si la cedula profesional est� vac�a, el usuario no podr� continuar
        if (string.IsNullOrWhiteSpace(this.txtNumCedula.Text))
        {
            await DisplayAlert("Advertencia", "Debe ingresar la c�dula profesional", "Aceptar");
            return;
        }
        //Si no se selecciono una especialidad en el picker, el usuario no podr� continuar
        if(this.pickerEspecialidad.SelectedItem == null)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar una especialidad para continuar", "Aceptar");
            return;
        }
        //Si no se selecciono al menos un consultorio, el usuario no podr� continuar
        if (consultoriosSeleccionados.Count == 0)
        {
            await DisplayAlert("Advertencia", "Debe seleccionar al menos un consultorio para continuar", "Aceptar");
            return;
        }

        try
        {
            //Crea el objeto para la creaci�n del nuevo m�dico
            NuevoMedico.NuevoMedicoSQL crear = new NuevoMedico.NuevoMedicoSQL(this.txtNumCedula.Text, this.pickerEspecialidad.SelectedItem.ToString());
            //M�todo para crear el m�dico, si se afecto m�s de una fila la creaci�n fue exitosa
            int resultado = await crear.Creacion();
            if (resultado > 0)
            {
                //M�todo que devuelve la informaci�n del m�dico recien registrado para obtener su ID de m�dico y pasarlo al administrador de Sesi�n
                DataSet datos = await crear.ObtenerIdMedico();
                Globales.AdministradorDeSesion.idMedico = Convert.ToInt32(datos.Tables["Tabla"].Rows[0]["idMedico"].ToString());
                //M�todo que le asigan el rol de M�dico al usuario que se est� creando
                await crear.AsignacionRol();
                await DisplayAlert("�xito", "Ha sido registrado como un nuevo m�dico. Asegurese de mantener su informaci�n personal siempre actualizada en la pesta�a PENDIENTE", "Aceptar");
                
                //Por cada consultorio seleccionado, se inserta una fila en la tabla MedicoConsultorio
                foreach (Consultorio consultorio in consultoriosSeleccionados)
                {
                    resultado = await crear.VinculacionMedicoConsultorio(consultorio.idConsultorio);
                    //Si en alg�n momento ocurre una excepci�n, el proceso se detiene y se muestra un mensaje de error
                    if (resultado == -1)
                    {
                        await DisplayAlert("Error", "Ocurrio un error al asignar uno o varios consultorios al m�dico. Vuelva a intentarlo m�s tarde", "Aceptar");
                        break;
                    }
                }
                //Tras haber creado el m�dico, el administrador regresa a su pantalla principal
                await Shell.Current.GoToAsync("//AdminPrincipalPage");
            }
            else if (resultado == 0)
            {
                await DisplayAlert("Error", "Creaci�n de nuevo m�dico fallida", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Excepci�n", $"Error: {ex.Message}", "Aceptar");
        }
    }
}