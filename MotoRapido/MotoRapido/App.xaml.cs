using Acr.Settings;
using MotoRapido.ViewModels;
using MotoRapido.Views;
using Prism.Unity;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MotoRapido
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            //Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //{
            //    // Do something
            //     Application.Current.MainPage.DisplayAlert("Aviso", "Falha ao efetuar login", "OK");
            //    return true;  // True = Repeat again, False = Stop the timer
            //});
            if (CrossSettings.Current.Contains("MotoristaLogado"))
                await NavigationService.NavigateAsync("NavigationPage/Home");
            else
                await NavigationService.NavigateAsync("NavigationPage/Login");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<Login>();
            Container.RegisterTypeForNavigation<Home>();
        }
    }
}
