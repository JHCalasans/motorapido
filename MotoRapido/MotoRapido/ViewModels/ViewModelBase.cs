using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Acr.Settings;
using MotoRapido.Interfaces;
using MotoRapido.Models;
using Prism.Services;
using MotoRapido.Customs;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace MotoRapido.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IPageDialogService DialogService { get; private set; }
        private StoppableTimer _stoppableTimer;

        public StoppableTimer StoppableTimer
        {
            get => _stoppableTimer;
            set { SetProperty(ref _stoppableTimer, value); }
        }

        public ViewModelBase(INavigationService navigationService, IPageDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }

        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        public virtual void Destroy()
        {
        }

        public async void Localizar()
        {
            try
            { 
                var posicao = await GetCurrentPosition();
                VerificaPosicaoParam param = new VerificaPosicaoParam
                {
                    codMotorista = MotoristaLogado.codigo,
                    latitude = posicao.Latitude.ToString(),
                    longitude = posicao.Longitude.ToString()
                };

                var json = JsonConvert.SerializeObject(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await IniciarCliente(true).PostAsync("motorista/verificarPosicao", content);

                if (!response.IsSuccessStatusCode)
                {
                    await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result,
                        "OK");
                }
                
              
            }
            catch (Exception e)
            {
            }
        }

        

        public async void iniciarTimerPosicao()
        {
            if (StoppableTimer == null) StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), Localizar);

            if (CrossSettings.Current.Get<bool>("IsTimerOn"))
                StoppableTimer.Start();
           // await  StartListening();
           // StoppableTimer.Start();
            // CrossSettings.Current.Set("isTimerOn", true);


            //int count = 0;
            //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            //{

            //    Debug.WriteLine(count + " - " + TimerOn);
            //    count++;
            //    return TimerOn; // True = Repeat again, False = Stop the timer
            //});
        }

        public void pararTimerPosicao()
        {
            //StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), teste);
            StoppableTimer.Stop();
            CrossSettings.Current.Set("isTimerOn", false);
            //int count = 0;
            //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
            //{

            //    Debug.WriteLine(count + " - " + TimerOn);
            //    count++;
            //    return TimerOn; // True = Repeat again, False = Stop the timer
            //});
        }

        public Motorista MotoristaLogado
        {
            get { return CrossSettings.Current.Get<Motorista>("MotoristaLogado"); }
        }

        protected HttpClient IniciarCliente(bool comChave)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(35000);
            client.BaseAddress = new Uri("http://192.168.0.12:8080/motorapido/ws/");
            if (comChave) client.DefaultRequestHeaders.Add("Authentication", MotoristaLogado.chaveServicos);
            return client;
        }

        public static async Task<Position> GetCurrentPosition()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;
                // position = await locator.GetLastKnownLocationAsync();
                //if (position != null)
                //{
                //    //got a cahched position, so let's use it.
                //    return position;
                //}

                if (!locator.IsGeolocationAvailable ||
                    !locator.IsGeolocationEnabled)
                {
                    //not available or enabled
                    return null;
                }

                position = await locator.GetPositionAsync(null, new CancellationToken(false), true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("falha ao pegar localização: " + ex);
            }


            return position;
        }


        public async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

            CrossGeolocator.Current.PositionChanged += PositionChanged;
            CrossGeolocator.Current.PositionError += PositionError;
        }

        private void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            Debug.WriteLine(output);
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine(e.Error);
            //Handle event here for errors
        }

        async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
        }

        //public static bool IsInPolygon(Point[] poly, Point point)
        //{
        //    var coef = poly.Skip(1).Select((p, i) =>
        //            (point.Y - poly[i].Y) * (p.X - poly[i].X)
        //            - (point.X - poly[i].X) * (p.Y - poly[i].Y))
        //        .ToList();

        //    if (coef.Any(p => p == 0))
        //        return true;

        //    for (int i = 1; i < coef.Count(); i++)
        //    {
        //        if (coef[i] * coef[i - 1] < 0)
        //            return false;
        //    }
        //    return true;
        //}

        //protected void MontarPoligonos(List<CoordenadasArea> lista)
        //{
        //    var resultado = lista.GroupBy(coord => coord.area.codigo).Select(group => group.First());
        //    foreach (var res in resultado)
        //    {
        //        var list = lista.Where(area => area.area == res.area);

        //    }
        //}
    }
}
