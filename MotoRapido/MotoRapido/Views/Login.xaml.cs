using MotoRapido.ViewModels;
using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            var tes = BindingContext as LoginViewModel;
            //tes.Dispose();
            base.OnDisappearing();
            BindingContext = null;
            Content = null;
        }
    }
}
