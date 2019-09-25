using Acr.Settings;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Matcha.BackgroundService;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Customs;
using MotoRapido.Views;
using Prism.Navigation;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Essentials;
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

        public static bool IsGPSEnable = false;

        public App(IPlatformInitializer initializer) : base(initializer) { }

        public static INavigationService AppNavigationService => (Current as App)?.CreateNavigationService();

        public static bool IsInForeground { get; set; } = false;
        public static String DeviceID { get; set; }

        public static String OneSignalID { get; set; }
        private Boolean _desviarParaChamada { get; set; }

        protected override void OnStart()
        {
            base.OnStart();
            try
            {
                AppCenter.Start("android=3da30223-d2af-4457-80c8-d55fbe32880d;",
                          typeof(Analytics), typeof(Crashes));


                //BackgroundAggregatorService.Add(() => new ChecagemInformacaoPendente());
                //BackgroundAggregatorService.StartBackgroundService();
                IsInForeground = true;

                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

               
                //Gyroscope.ReadingChanged -= Gyroscope_ReadingChanged;
                //Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;


            }
            catch(Exception e)
            {
                Crashes.TrackError(e);
            }

        }

        void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading;
            CrossSettings.Current.Set("OrientacaoPsoicao", data);

            // Process Angular Velocity X, Y, and Z reported in rad/s
            //Console.WriteLine($"Reading: X: {data.AngularVelocity.X}, Y: {data.AngularVelocity.Y}, Z: {data.AngularVelocity.Z}");
        }

        public void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;
            if (access != NetworkAccess.Internet)
            {
                MessagingCenter.Send(this, "SemInternet", true);
            }
            else
            {
                MessagingCenter.Send(this, "SemInternet", false);
            }
            //else if(access == NetworkAccess.ConstrainedInternet) {
            //    lblNetStatus.Text = "Limited internet access";
            //}
            //else if(access == NetworkAccess.Local) {
            //    lblNetStatus.Text = "Local network access only";
            //}
            //else if(access == NetworkAccess.None) {
            //    lblNetStatus.Text = "No connectivity is available";
            //}
            //else if(access == NetworkAccess.Unknown) {
            //    lblNetStatus.Text = "Unable to determine internet connectivity";
            //}
            //if (profiles.Contains(ConnectionProfile.WiFi))
            //{
            //    lblNetProfile.Text = profiles.FirstOrDefault().ToString();
            //}
            //else
            //{
            //    lblNetProfile.Text = profiles.FirstOrDefault().ToString();
            //}
        }



        protected override async void OnInitialized()
        {

            OneSignal.Current.StartInit("a1a45079-6c44-4353-9588-47d8fbc306bb").HandleNotificationReceived(HandleNotificationReceived)
                .HandleNotificationOpened(HandleNotificationOpened).EndInit();

            OneSignal.Current.IdsAvailable((id, token) => OneSignalID = id);


            //if (CrossSettings.Current.Contains("IdAparelhoVinculado"))
            //{
            //    if (CrossSettings.Current.Get<Boolean>("IdAparelhoVinculado"))
            //    {

            //    }
            //}
            //else
            //{
            //    CrossSettings.Current.Set("IdAparelhoVinculado", false);
            //}


            InitializeComponent();
//#if DEBUG
//            HotReloader.Current.Run(this);
//#endif


            CrossSettings.Current.Remove("ServidorFora");
            if (CrossSettings.Current.Contains("IdAparelhoVinculado") && CrossSettings.Current.Get<Boolean>("IdAparelhoVinculado"))
            {
                if (CrossSettings.Current.Contains("MotoristaLogado"))
                {
                    if (CrossSettings.Current.Contains("VeiculoSelecionado"))
                    {
                        if (CrossSettings.Current.Contains("ChamadaEmCorrida"))
                            await NavigationService.NavigateAsync("//NavigationPage/Chamada", null, useModalNavigation: true);

                        if (!CrossSettings.Current.Contains("ChamadaParaResposta"))
                            await NavigationService.NavigateAsync("NavigationPage/Home");
                        else
                            await NavigationService.NavigateAsync("//NavigationPage/ResponderChamada", null, useModalNavigation: true);
                    }
                    else
                    {
                        NavigationParameters param = new NavigationParameters();
                        param.Add("pesquisar", true);
                        await NavigationService.NavigateAsync("NavigationPage/Veiculos", param);
                    }
                }
                else
                    await NavigationService.NavigateAsync("NavigationPage/Logar");
            }
            else
            {
                await NavigationService.NavigateAsync("NavigationPage/EnvioIdAparelho");
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            IsInForeground = true;
            Boolean tes = Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationEnabled;
            if (tes && CrossSettings.Current.Contains("GPSDesabilitado"))
            {
                MessagingCenter.Send(this, "GPSHabilitou");
            }
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
                if (additionalData.ContainsKey("logout"))
                {
                    CrossSettings.Current.Clear();
                    AppNavigationService.NavigateAsync("NavigationPage/Logar");
                }
                if (additionalData.ContainsKey("tempoEsperaAceitacao"))
                {

                    CrossSettings.Current.Set("dataRecebimentoChamada", DateTime.Now);
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
                if (additionalData.ContainsKey("codChamadaVeiculo"))
                {

                    //DateTime dtRecebimento = CrossSettings.Current.Get<DateTime>("dataRecebimentoChamada");
                    if (additionalData.ContainsKey("tempoEsperaAceitacao"))
                    {
                        string tempoEspera = additionalData["tempoEsperaAceitacao"].ToString();
                        CrossSettings.Current.Set("tempoEsperaAceitacao", tempoEspera);
                    }

                    //  var tempoEspera = CrossSettings.Current.Contains

                    var value = additionalData["codChamadaVeiculo"];
                    CrossSettings.Current.Set("ChamadaParaResposta", value.ToString());
                    //NavigationParameters param = new NavigationParameters();
                    //  param.Add("codChamada", value);  
                    // if(actionID != null)
                    AppNavigationService.NavigateAsync("//NavigationPage/ResponderChamada", null, true);

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


        protected override void OnSleep()
        {
            base.OnSleep();
            IsInForeground = false;
        }

        
       

        protected override void RegisterTypes()
        {
            // Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<Home>();
            Container.RegisterTypeForNavigation<Views.Chamada>();
            Container.RegisterTypeForNavigation<Mensagem>();
            Container.RegisterTypeForNavigation<Views.NavigationPage>();
            Container.RegisterTypeForNavigation<Configuracao>();
            Container.RegisterTypeForNavigation<Pendencias>();
            Container.RegisterTypeForNavigation<Historico>();
            Container.RegisterTypeForNavigation<Veiculos>();
            Container.RegisterTypeForNavigation<ResponderChamada>();
            Container.RegisterTypeForNavigation<EnvioIdAparelho>();
            Container.RegisterTypeForNavigation<Logar>();
        }


    }
}
