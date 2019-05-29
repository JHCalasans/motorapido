namespace MotoRapido.ViewModels
{
    using Acr.Settings;
    using Acr.UserDialogs;
    using MotoRapido.Customs;
    using MotoRapido.Models;
    using Newtonsoft.Json;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using Prism.Mvvm;
    using Prism.Navigation;
    using Prism.Services;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="ViewModelBase" />
    /// </summary>
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        /// <summary>
        /// Gets the NavigationService
        /// </summary>
        protected INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Gets the DialogService
        /// </summary>
        protected IPageDialogService DialogService { get; private set; }

        /// <summary>
        /// Defines the _stoppableTimer
        /// </summary>
        private StoppableTimer _stoppableTimer;

        /// <summary>
        /// Gets or sets the StoppableTimer
        /// </summary>
        public StoppableTimer StoppableTimer
        {
            get => _stoppableTimer;
            set { SetProperty(ref _stoppableTimer, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="navigationService">The navigationService<see cref="INavigationService"/></param>
        /// <param name="dialogService">The dialogService<see cref="IPageDialogService"/></param>
        public ViewModelBase(INavigationService navigationService, IPageDialogService dialogService)
        {
            NavigationService = navigationService;
            DialogService = dialogService;
        }

        /// <summary>
        /// The OnNavigatedFrom
        /// </summary>
        /// <param name="parameters">The parameters<see cref="NavigationParameters"/></param>
        public virtual void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// The OnNavigatedTo
        /// </summary>
        /// <param name="parameters">The parameters<see cref="NavigationParameters"/></param>
        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// The OnNavigatingTo
        /// </summary>
        /// <param name="parameters">The parameters<see cref="NavigationParameters"/></param>
        public virtual void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// The Destroy
        /// </summary>
        public virtual void Destroy()
        {
        }

        /// <summary>
        /// The Localizar
        /// </summary>
        /// <param name="posicao">The posicao<see cref="Position"/></param>
        public async void Localizar(Position posicao)
        {
            try
            {
                
                VerificaPosicaoParam param = new VerificaPosicaoParam
                {
                    codMotorista = MotoristaLogado.codigo,
                    latitude = posicao.Latitude.ToString().Replace(",", "."),
                    longitude = posicao.Longitude.ToString().Replace(",",".")
                };

                var json = JsonConvert.SerializeObject(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await IniciarCliente(true).PostAsync("motorista/verificarPosicao", content);

                if (!response.IsSuccessStatusCode)
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
                    {
                        AreaPosicao = new RetornoVerificaPosicao();
                        AreaPosicao.msgErro = response.Content.ReadAsStringAsync().Result;
                    }

                    //await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result,
                    //    "OK");
                }
                else
                {                   
                    var respStr = await response.Content.ReadAsStringAsync();
                    AreaPosicao = JsonConvert.DeserializeObject<RetornoVerificaPosicao>(respStr);
                    if (AreaPosicao.areaAtual == null)
                        AreaPosicao = null;
                   
                    
                }
                if (CrossSettings.Current.Contains("ChamadaEmCorrida") || CrossSettings.Current.Contains("ChamadaAceita"))
                    AreaPosicao.msgErro = "Chamada Em Andamento!";

            }
            catch (Exception e)
            {
                if (CrossSettings.Current.Contains("ChamadaEmCorrida") || CrossSettings.Current.Contains("ChamadaAceita"))
                    AreaPosicao.msgErro = "Chamada Em Andamento!";
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao verificar posição", "OK");
            }
        }

        /// <summary>
        /// The iniciarTimerPosicao
        /// </summary>
        public async void iniciarTimerPosicao()
        {
            //   if (StoppableTimer == null) StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), Localizar);

            if (CrossSettings.Current.Get<bool>("IsTimerOn"))
               await  StartListening();
           
        }

        /// <summary>
        /// The pararTimerPosicao
        /// </summary>
        public void pararTimerPosicao()
        {
            //StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), teste);
            //StoppableTimer.Stop();
           
        }

        /// <summary>
        /// Gets the MotoristaLogado
        /// </summary>
        public Motorista MotoristaLogado
        {
            get { return CrossSettings.Current.Get<Motorista>("MotoristaLogado"); }
            set { ; }
        }

        /// <summary>
        /// Defines the _areaPosicao
        /// </summary>
        private RetornoVerificaPosicao _areaPosicao;

        /// <summary>
        /// Gets or sets the AreaPosicao
        /// </summary>
        public RetornoVerificaPosicao AreaPosicao
        {
            get { return _areaPosicao; }
            set { SetProperty(ref _areaPosicao, value); }
        }

        /// <summary>
        /// Gets or sets the ultimaLocalizacaoValida
        /// </summary>
       // private Position _ultimaLocalizacaoValida;

        public Position UltimaLocalizacaoValida
        {
            get { return CrossSettings.Current.Get<Position>("UltimaLocalizacaoValida"); }
          
        }

        /// <summary>
        /// The IniciarCliente
        /// </summary>
        /// <param name="comChave">The comChave<see cref="bool"/></param>
        /// <returns>The <see cref="HttpClient"/></returns>
        protected HttpClient IniciarCliente(bool comChave)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(35000),
                BaseAddress = new Uri(_urlBase)
            };
            if (comChave) client.DefaultRequestHeaders.Add("Authentication", MotoristaLogado.chaveServicos);
            return client;
        }



       // private String _urlBase = "http://10.0.3.2:8080/motorapido/ws/";

        private String _urlBase = "http://192.168.0.4:8080/motorapido/ws/";

        // private String _urlBase = "http://104.248.186.97:8080/motorapido/ws/";

        /// <summary>
        /// The GetCurrentPosition
        /// </summary>
        /// <returns>The <see cref="Task{Position}"/></returns>
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

        /// <summary>
        /// The StartListening
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task StartListening()
        {
            if (CrossSettings.Current.Get<bool>("IsTimerOn"))
            {
                if (CrossGeolocator.Current.IsListening)
                    return;


                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);


                CrossGeolocator.Current.PositionChanged += PositionChanged;
                CrossGeolocator.Current.PositionError += PositionError;

                //await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, false, new ListenerSettings
                //{
                //    AllowBackgroundUpdates = true,
                //    PauseLocationUpdatesAutomatically = false
                //});


            }
        }

        /// <summary>
        /// The PositionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="PositionEventArgs"/></param>
        private void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            if (UltimaLocalizacaoValida == null || isLocalizacaoDiferente(position, UltimaLocalizacaoValida))
            {               
                Localizar(position);
                CrossSettings.Current.Set("UltimaLocalizacaoValida", position);
                if (CrossSettings.Current.Contains("ChamadaEmCorrida") || CrossSettings.Current.Contains("ChamadaAceita"))
                {
                   // MessagingCenter.Send(this, "MudancaPin", position);
                    AreaPosicao.msgErro = "Chamada Em Andamento!";
                }
                
            }
        }

        /// <summary>
        /// The isLocalizacaoDiferente
        /// </summary>
        /// <param name="localizacao1">The localizacao1<see cref="Position"/></param>
        /// <param name="localizacao2">The localizacao2<see cref="Position"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool isLocalizacaoDiferente(Position localizacao1, Position localizacao2)
        {
            return localizacao1.Latitude != localizacao2.Latitude || localizacao1.Altitude != localizacao2.Altitude;
        }

        /// <summary>
        /// The PositionError
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="PositionErrorEventArgs"/></param>
        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine(e.Error);
        }

        /// <summary>
        /// The StopListening
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        internal async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;

            CrossSettings.Current.Set("IsTimerOn", false);
        }



        public enum SituacaoChamadaEnum : int
        {
            CANCELADA = 1,
            PENDENTE = 2,
            ACEITA = 3,
            EXPIRADA = 4,
            PENDENTE_GERAL = 5,
            FINALIZADA = 6
        };

    }
}
