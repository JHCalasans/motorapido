using MotoRapido.ViewModels;
using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Mensagem : ContentPage
    {
        public Mensagem()
        {
            InitializeComponent();


            ((MensagemViewModel)(BindingContext)).ListMessages.CollectionChanged += (sender, e) =>
            {
                var target = ((MensagemViewModel)(BindingContext)).ListMessages[((MensagemViewModel)(BindingContext)).ListMessages.Count - 1];
                MessagesListView.ScrollTo(target, ScrollToPosition.End, true);

            };


        }      

        private void MessagesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MessagesListView.SelectedItem = null;
        }
    }
}
