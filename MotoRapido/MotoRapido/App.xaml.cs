using Acr.Settings;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using MotoRapido.ViewModels;
using MotoRapido.Views;
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

        protected override async void OnInitialized()
        {

            OneSignal.Current.StartInit("a1a45079-6c44-4353-9588-47d8fbc306bb").HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened).EndInit();
            //    OneSignal.Current.SetLogLevel(LOG_LEVEL.VERBOSE, LOG_LEVEL.WARN);
            //    OneSignal.Current.StartInit("a1a45079-6c44-4353-9588-47d8fbc306bb").Settings(new Dictionary<string, bool>() {
            //    { IOSSettings.kOSSettingsKeyAutoPrompt, false },
            //    { IOSSettings.kOSSettingsKeyInAppLaunchURL, true } })
            //   .InFocusDisplaying(OSInFocusDisplayOption.Notification)
            //   .HandleNotificationOpened((result) =>
            //   {
            //       Debug.WriteLine("HandleNotificationOpened: {0}", result.notification.payload.body);
            //   })
            //   .HandleNotificationReceived((notification) =>
            //   {
            //       Debug.WriteLine("HandleNotificationReceived: {0}", notification.payload.body);
            //   })
            //   .EndInit();

            //    OneSignal.Current.IdsAvailable((playerID, pushToken) =>
            //    {
            //        Debug.WriteLine("OneSignal.Current.IdsAvailable:D playerID: {0}, pushToken: {1}", playerID, pushToken);
            //    });


            InitializeComponent();
            //Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //{
            //    // Do something
            //     Application.Current.MainPage.DisplayAlert("Aviso", "Falha ao efetuar login", "OK");
            //    return true;  // True = Repeat again, False = Stop the timer
            //});

            await NavigationService.NavigateAsync("MainPage");
            //if (CrossSettings.Current.Contains("MotoristaLogado"))
            //    await NavigationService.NavigateAsync("NavigationPage/Home");
            //else
            //    await NavigationService.NavigateAsync("NavigationPage/Login");
        }

        // Called when your app is in focus and a notificaiton is recieved.
        // The name of the method can be anything as long as the signature matches.
        // Method must be static or this object should be marked as DontDestroyOnLoad
        private static void HandleNotificationReceived(OSNotification notification)
        {
            OSNotificationPayload payload = notification.payload;
            string message = payload.body;
            
            Debug.WriteLine("GameControllerExample:HandleNotificationReceived: " + message);
            Debug.WriteLine("displayType: " + notification.displayType);
            Debug.WriteLine( "Notification received with text: " + message);
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

            Debug.WriteLine("GameControllerExample:HandleNotificationOpened: " + message);
            Debug.WriteLine( "Notification opened with text: " + message);

            if (additionalData != null)
            {
                if (additionalData.ContainsKey("discount"))
                {
                  //  extraMessage = (string)additionalData["discount"];
                    // Take user to your store.
                }
            }
            if (actionID != null)
            {
                // actionSelected equals the id on the button the user pressed.
                // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
               // extraMessage = "Pressed ButtonId: " + actionID;
            }
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
