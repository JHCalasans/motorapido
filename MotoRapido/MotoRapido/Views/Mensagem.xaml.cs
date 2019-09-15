using MotoRapido.ViewModels;
using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Mensagem : ContentPage
    {
        public Mensagem()
        {
            InitializeComponent();

            if (((MensagemViewModel)(BindingContext)).ListMessages != null)
            {
                var target1 = ((MensagemViewModel)(BindingContext)).ListMessages[((MensagemViewModel)(BindingContext)).ListMessages.Count - 1];
                MessagesListView.ScrollTo(target1, ScrollToPosition.End, false);

                ((MensagemViewModel)(BindingContext)).ListMessages.CollectionChanged += (sender, e) =>
                {
                    var target = ((MensagemViewModel)(BindingContext)).ListMessages[((MensagemViewModel)(BindingContext)).ListMessages.Count - 1];
                    MessagesListView.ScrollTo(target, ScrollToPosition.End, true);

                };
            }
        }
    }
}
