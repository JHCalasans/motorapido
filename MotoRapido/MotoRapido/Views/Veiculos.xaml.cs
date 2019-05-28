using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Veiculos : ContentPage
    {
        public Veiculos()
        {
            InitializeComponent();
        }

        private void ListaVeiculos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListaVeiculos.SelectedItem = null;
        }
    }
}
