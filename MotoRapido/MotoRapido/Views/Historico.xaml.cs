using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Historico : ContentPage
    {
        public Historico()
        {
            InitializeComponent();
        }

        private void ListaHistorico_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListaHistorico.SelectedItem = null;
        }
    }
}
