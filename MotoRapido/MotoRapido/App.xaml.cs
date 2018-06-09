using Acr.Settings;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using MotoRapido.ViewModels;
using MotoRapido.Views;
using Prism.Navigation;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static INavigationService AppNavigationService => (Current as App)?.CreateNavigationService();

        private Boolean _desviarParaChamada { get; set; }

        protected override async void OnInitialized()
        {

            OneSignal.Current.StartInit("a1a45079-6c44-4353-9588-47d8fbc306bb").HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened).EndInit();
          

            InitializeComponent();




            if (CrossSettings.Current.Contains("MotoristaLogado"))
            {
                if (CrossSettings.Current.Contains("ExisteChamada"))
                    await NavigationService.NavigateAsync("NavigationPage/Home/Chamada");
                else
                    await NavigationService.NavigateAsync("NavigationPage/Home");
            }
            else
                await NavigationService.NavigateAsync("NavigationPage/Login");
        }


        protected override void OnResume()
        {
            base.OnResume();
        }

        // Called when your app is in focus and a notificaiton is recieved.
        // The name of the method can be anything as long as the signature matches.
        // Method must be static or this object should be marked as DontDestroyOnLoad
        private static void HandleNotificationReceived(OSNotification notification)
        {
            OSNotificationPayload payload = notification.payload;
            string message = payload.body;
            Dictionary<string, object> additionalData = payload.additionalData;

            if (additionalData != null)
            {
                if (additionalData.ContainsKey("mdn_area"))
                {
                    CrossSettings.Current.Set("atualizarDados", true);
                }
            }

        }

        // Called when a notification is opened.
        // The name of the method can be anything as long as the signature matches.
        // Method must be static or this object should be marked as DontDestroyOnLoad
        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            OSNotificationPayload payload = result.notification.payload;
            Dictionary<string, object> additionalData = payload.additionalData;
            string message = payload.body;
            string actionID = result.action.actionID;

            if (additionalData != null)
            {
                if (additionalData.ContainsKey("chamada"))
                {
                    CrossSettings.Current.Set("ExisteChamada", true);
                    AppNavigationService.NavigateAsync("Home/Chamada");
                }
            }
            
            //if (actionID != null)
            //{
            //    if (actionID.Equals("__DEFAULT__"))
            //        AppNavigationService.NavigateAsync("Chamada");

            //    // actionSelected equals the id on the button the user pressed.
            //    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
            //    // extraMessage = "Pressed ButtonId: " + actionID;
            //}
        }


        protected override void RegisterTypes()
        {
           // Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<Login>();
            Container.RegisterTypeForNavigation<Home>();
            Container.RegisterTypeForNavigation<Chamada>();
            Container.RegisterTypeForNavigation<Mensagem>();
            Container.RegisterTypeForNavigation<Views.NavigationPage>();
        }

        
    }
}
