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

            //MessagingCenter.Subscribe<App>(this, "GPSHabilitou", (sender) =>
            //{

            //    if (MotoristaLogado.disponivel.Equals("S"))
            //    {
            //        Thread.Sleep(TimeSpan.FromSeconds(3));
            //        CrossSettings.Current.Set("UltimaLocalizacaoValida", Task.Run(() => GetCurrentPosition()));
            //        Localizar(UltimaLocalizacaoValida);
            //    }


            //});



        }

        private void MessagesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            MessagesListView.SelectedItem = null;
        }
    }
}
