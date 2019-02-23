using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Pendencias : ContentPage
    {
        public Pendencias()
        {
            InitializeComponent();
        }

        private void ListaChamadas_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListaChamadas.SelectedItem = null;
        }
    }
}
