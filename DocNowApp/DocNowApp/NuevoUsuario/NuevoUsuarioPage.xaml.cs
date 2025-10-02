using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace DocNowApp.NuevoUsuario;

public partial class NuevoUsuarioPage : ContentPage
{
	public NuevoUsuarioPage()
	{
		InitializeComponent();
	}

    private void Entry_TextChangued(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.txtNombre.Text) || string.IsNullOrWhiteSpace(this.txtApellP.Text) || string.IsNullOrWhiteSpace(this.txtApellM.Text)
           || string.IsNullOrWhiteSpace(this.txtCorreo.Text) || string.IsNullOrWhiteSpace(this.txtTelefono.Text) || string.IsNullOrEmpty(this.txtContrasenia.Text))
        {
            this.btnRegistrarse.IsEnabled = false;
        }
        else
        {
            this.btnRegistrarse.IsEnabled = true;
        }
    }

    private async void btnRegistrarse_Clicked(object sender, EventArgs e)
    {
        char sexo;
        if (this.rbHombre.IsChecked || this.rbMujer.IsChecked)
        {
            if (this.rbHombre.IsChecked)
            {
                sexo = 'H';
            }
            else
            {
                sexo = 'M';
            }
            NuevoUsuario.NuevoUsuarioSQL crear = new NuevoUsuario.NuevoUsuarioSQL(this.txtNombre.Text, this.txtApellP.Text, this.txtApellM.Text,
                this.txtCorreo.Text, this.txtTelefono.Text, this.txtTelefono.Text, this.dateFechaNac.Date, sexo, "ROL PENDIENTE", DateTime.Today, DateTime.Today);
            DataSet datos = await crear.ValidarCorreo();
            if (datos.Tables.Count == 0 || datos.Tables["Tabla"].Rows.Count == 0)
            {
                int resultado = await crear.Creacion();
                if (resultado > 0)
                {
                    bool opcion = await DisplayAlert("Exito", "Se ha creado correctamente el usuario. A continuación, elige el rol que deseas tener", "Paciente", "Medico");
                    if (opcion)
                    {
                        await Shell.Current.GoToAsync("//NuevoPacientePage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//NuevoMedicoPage");
                    }
                }
                else if (resultado == 0)
                {
                    await DisplayAlert("Error", "Creación de nuevo usuario fallida", "Aceptar");
                }
            }
            else
            {
                await DisplayAlert("Advertencia", "El correo introducido ya existe", "Aceptar");
            }
        }
        else
        {
            await DisplayAlert("Advertencia", "Debe seleccionar un sexo", "Aceptar");
        }
    }
}