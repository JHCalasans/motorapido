namespace MotoRapido.ViewModels
{
    using Acr.Settings;
    using Acr.UserDialogs;
    using Microsoft.AppCenter.Crashes;
    using MotoRapido.BD.Repositorio;
    using MotoRapido.Customs;
    using MotoRapido.Models;
    using Newtonsoft.Json;
    using Plugin.Connectivity;
    using Plugin.Geolocator;
    using Plugin.Geolocator.Abstractions;
    using Prism.Mvvm;
    using Prism.Navigation;
    using Prism.Services;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xamarin.Essentials;
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

        public ViewModelBase() { }

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


        public async Task ConectarSocket()
        {
            try
            {
                // MessagingCenter.Send(this, "TesteOps");
                //if (client == null)
                //    client = new WebSocketClientDefault();
                //  await client.Connect(MotoristaLogado.chaveServicos, MotoristaLogado.codigo.ToString());
                await WebSocketClientClass.Connect(MotoristaLogado.chaveServicos, MotoristaLogado.codigo.ToString());
                //await WebSocketClientClass.SenMessagAsync("ListarSessoes=>");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                throw new WebSocketException("Falha na conexão com servidor"); // await DialogService.DisplayAlertAsync("Aviso", "Falha na conexão com servidor", "OK");
            }
        }

        public void DesconectarSocket()
        {
            try
            {
                WebSocketClientClass.CloseWs();
                //await WebSocketClientClass.SenMessagAsync("ListarSessoes=>");
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                throw new WebSocketException("Falha na conexão com servidor"); // await DialogService.DisplayAlertAsync("Aviso", "Falha na conexão com servidor", "OK");
            }
        }



        /// <summary>
        /// The Localizar
        /// </summary>
        /// <param name="posicao">The posicao<see cref="Position"/></param>
        public async void Localizar(Position posicao)
        {

            if (MotoristaLogado.disponivel.Equals("S"))
            {
                if (CrossGeolocator.Current.IsGeolocationEnabled)//App.IsGPSEnable)
                {
                    CrossSettings.Current.Remove("GPSDesabilitado");
                    try
                    {
                        if (CrossSettings.Current.Contains("ChamadaEmCorrida") || CrossSettings.Current.Contains("ChamadaAceita"))
                            AreaPosicao.msgErro = "Chamada Em Andamento!";
                        //  else
                        //  {



                        VerificaPosicaoParam param = new VerificaPosicaoParam
                        {
                            codMotorista = MotoristaLogado.codigo,
                            latitude = posicao.Latitude.ToString().Replace(",", "."),
                            longitude = posicao.Longitude.ToString().Replace(",", "."),
                            loginMotorista = MotoristaLogado.login


                        };

                        if (AreaPosicao != null && AreaPosicao.areaAtual != null)
                            param.codUltimaArea = AreaPosicao.areaAtual.codigo;

                        var json = JsonConvert.SerializeObject(param);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        CrossSettings.Current.Set("UltimaAtualizacaoLocalidade", new DateTime());
                        await ConectarSocket();
                        // await client.SenMessagAsync("InformarLocalizacao=>" + json);
                        await WebSocketClientClass.SendMessagAsync("InformarLocalizacao=>" + json);

                      


                    }
                    catch (AccessViolationException e)
                    {
                        await DialogService.DisplayAlertAsync("Aviso", e.Message, "OK");
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                        if (CrossSettings.Current.Contains("ChamadaEmCorrida") || CrossSettings.Current.Contains("ChamadaAceita"))
                            AreaPosicao.msgErro = "Chamada Em Andamento!";
                        // await DialogService.DisplayAlertAsync("Aviso", "Falha ao verificar posição", "OK");
                    }
                }
                else
                {
                    AreaPosicao = new RetornoVerificaPosicao();
                    AreaPosicao.msgErro = "Favor ativar gps no celular.";
                    CrossSettings.Current.Set("GPSDesabilitado", true);
                    MessagingCenter.Subscribe<App>(this, "GPSHabilitou", (sender) =>
                    {

                        if (MotoristaLogado.disponivel.Equals("S"))
                        {
                            CrossSettings.Current.Set("UltimaLocalizacaoValida", Task.Run(() => GetCurrentPosition()));
                            Localizar(UltimaLocalizacaoValida);
                        }


                    });
                }
            }
        }

        public static async void InformarCoordenada()
        {
           
                if (CrossGeolocator.Current.IsGeolocationEnabled)//App.IsGPSEnable)
                {
                    CrossSettings.Current.Remove("GPSDesabilitado");
                    try
                    {

                       var pos = await GetCurrentPosition();

                        VerificaPosicaoParam param = new VerificaPosicaoParam
                        {
                            codMotorista = CrossSettings.Current.Get<Motorista>("MotoristaLogado").codigo,
                            latitude = pos.Latitude.ToString().Replace(",", "."),
                            longitude = pos.Longitude.ToString().Replace(",", "."),
                            loginMotorista = CrossSettings.Current.Get<Motorista>("MotoristaLogado").login


                        };

                      
                        var json = JsonConvert.SerializeObject(param);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        CrossSettings.Current.Set("UltimaAtualizacaoLocalidade", new DateTime());
                        // await client.SenMessagAsync("InformarLocalizacao=>" + json);
                        await WebSocketClientClass.SendMessagAsync("InformarLocalizacao=>" + json);




                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                       
                    }
                }
                
            
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
         
        }


        public async void MostrarMensagem(String msg)
        {
            await DialogService.DisplayAlertAsync("Aviso", msg, "OK");
        }

        /// <summary>
        /// The iniciarTimerPosicao
        /// </summary>
        public async void IniciarTimerPosicao()
        {
            //   if (StoppableTimer == null) StoppableTimer = new StoppableTimer(TimeSpan.FromSeconds(2), Localizar);

            if (CrossGeolocator.Current.IsGeolocationEnabled)//App.IsGPSEnable)
            {
                CrossSettings.Current.Remove("GPSDesabilitado");
                if (CrossSettings.Current.Get<bool>("IsTimerOn"))
                    await StartListening();
            }
            else
            {
                AreaPosicao = new RetornoVerificaPosicao();
                AreaPosicao.msgErro = "Favor ativar gps no celular.";
                CrossSettings.Current.Set("GPSDesabilitado", true);
                MessagingCenter.Subscribe<App>(this, "GPSHabilitou", (sender) =>
                    {

                        if (MotoristaLogado.disponivel.Equals("S"))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                            CrossSettings.Current.Set("UltimaLocalizacaoValida", Task.Run(() => GetCurrentPosition()));
                            Localizar(UltimaLocalizacaoValida);
                        }


                    });
              
            }

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
            set {; }
        }

        public String IDAparelho
        {
            get { return App.DeviceID; }
            set {; }
        }

        public static Motorista GetMotoristaStatic()
        {
            return CrossSettings.Current.Get<Motorista>("MotoristaLogado");
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

        private static IInformacaoPendenteRepositorio _informacaoPendenteRepositorio;

        public static void IniciarInformacaoPendenteRepositorio()
        {
            _informacaoPendenteRepositorio = new InformacaoPendenteRepositorio();
        }

        public static void AdicionarInfoPendente(string conteudo, string servico)
        {
            if (_informacaoPendenteRepositorio == null)
                IniciarInformacaoPendenteRepositorio();

            _informacaoPendenteRepositorio.AdicionarInformacaoPendente(new InformacaoPendente() { conteudo = conteudo, dtHora = DateTime.Now, servico = servico });

        }

        private static IMessageRepositorio _messageRepositorio;

        public static void IniciarMessageRepositorio()
        {
            _messageRepositorio = new MessageRepositorio();
        }

        public static Message GravarMensagem(string mensagem)
        {
            if (_messageRepositorio == null)
                IniciarMessageRepositorio();

            Message message = new Message { Text = mensagem, IsTextIn = false, MessageDateTime = DateTime.Now };
            _messageRepositorio.GravarMensagem(message);
            return message;
        }

        public static void GravarMensagem(Message mensagem)
        {
            if (_messageRepositorio == null)
                IniciarMessageRepositorio();

          
            _messageRepositorio.GravarMensagem(mensagem);
            
        }

        public static List<Message> ObterMensagens()
        {
            if (_messageRepositorio == null)
                IniciarMessageRepositorio();

            return _messageRepositorio.ObterTodasAsMensagens();

        }

        public static void TratarMensagemChamada(string json)
        {
            var chamadaNova = JsonConvert.DeserializeObject<Chamada>(json);
            if (App.IsInForeground)
                MessagingCenter.Send(chamadaNova, "NovaChamada");
            //  Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Nova Chamada", "Nova Chamada", "OK"));
            //  await DialogService.DisplayAlertAsync("Nova Chamada", "Nova Chamada", "OK");
            else
            {
                CrossSettings.Current.Set("ChamadaParaResposta", chamadaNova);
                // CrossLocalNotifications.Current.Show("Nova Chamada", "nova chamada");
            }
        }


        public static void RemoverInfoPendente(Int64 codInfoPendente)
        {
            if (_informacaoPendenteRepositorio == null)
                IniciarInformacaoPendenteRepositorio();

            _informacaoPendenteRepositorio.DeletarInformacaoPendente(codInfoPendente);
        }

        public static void RemoverTodasInfoPendentes()
        {
            if (_informacaoPendenteRepositorio == null)
                IniciarInformacaoPendenteRepositorio();

            _informacaoPendenteRepositorio.DeletarTodosInformacaoPendentes();
        }

        public static List<InformacaoPendente> ListarTodasInfoPendentes()
        {
            if (_informacaoPendenteRepositorio == null)
                IniciarInformacaoPendenteRepositorio();

            return _informacaoPendenteRepositorio.ObterTodosInformacaoPendentes();
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
            if (CrossConnectivity.Current.IsConnected)
            {
                var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMilliseconds(35000),
                    BaseAddress = new Uri(_urlBase)
                };
                if (comChave) client.DefaultRequestHeaders.Add("Authentication", MotoristaLogado.chaveServicos);
                return client;
            }
            else
            {
                throw new AccessViolationException("Sem conexão com internet(Tente mais tarde)!");
            }

        }


         private static readonly String _urlBase = "http://"+GetUrlBase()+"/motorapido/wes/";

       

        public static string GetUrlBase()
        {
            return "192.168.0.4:8080";

          //  return "104.248.186.97:8080";

          //  return "10.0.3.2:8080";
        }

        private static Position _posicaoTeste;

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
                var tes = CrossGeolocator.IsSupported;
                if (!locator.IsGeolocationAvailable ||
                    !locator.IsGeolocationEnabled)
                {
                    //not available or enabled
                    return null;
                }

                //   position = await locator.GetPositionAsync();

                //  _posicaoTeste = new Position(position.Latitude, position.Longitude);

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(3), new CancellationToken(false), true);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Debug.WriteLine("falha ao pegar localização: " + ex);
            }
            finally
            {
                var x = position;
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



                // if (UltimaLocalizacaoValida == null)
                // {
                //   Position pos;
                //while (_posicaoTeste == new Position(0, 0) || _posicaoTeste == null)
                //   await  GetCurrentPosition();

                //  PositionEventArgs args = new PositionEventArgs(pos);
                // CrossSettings.Current.Set("UltimaLocalizacaoValida", await GetCurrentPosition());
                // PositionChanged(null, args);
                //pos = await GetCurrentPosition();
                //CrossSettings.Current.Set("UltimaLocalizacaoValida", pos);
                //Localizar(pos);
                //  }

                CrossGeolocator.Current.PositionChanged += PositionChanged;
                CrossGeolocator.Current.PositionError += PositionError;

            }
        }

        //public  void EnviarMessaginCenter(String definicao)
        //{
        //    MessagingCenter.Send(this, "MudancaPosicao", definicao);

        //}

        double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c; // Distance in km
            return d;
        }

        double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }



        public void BuscaPosicao(Position position)
        {
            Localizar(position);
            Chamada chamadatemp = new Chamada();
            chamadatemp = CrossSettings.Current.Get<Chamada>("ChamadaEmCorrida");
            MessagingCenter.Send(this, "MudancaPosicao", position);
            if (CrossSettings.Current.Contains("ChamadaEmCorrida"))
            {
                AreaPosicao.msgErro = "Chamada Em Andamento!";
                double distancia = Location.CalculateDistance(new Location(UltimaLocalizacaoValida.Latitude, UltimaLocalizacaoValida.Longitude),
                    new Location(position.Latitude, position.Longitude), DistanceUnits.Kilometers);


                chamadatemp.distanciaPercorrida = chamadatemp.distanciaPercorrida + (float)distancia;

                if (distancia > 0)
                {
                    chamadatemp.valorFinal = (float.Parse(chamadatemp.valorFinal) + (chamadatemp.valorPorDistancia * (float)distancia)).ToString("N2");
                    CrossSettings.Current.Set("ChamadaEmCorrida", chamadatemp);

                }
                else
                    CrossSettings.Current.Set("ChamadaEmCorrida", chamadatemp);
            }
            else if (CrossSettings.Current.Contains("ChamadaAceita"))
            {
                AreaPosicao.msgErro = "Chamada Em Andamento!";

            }
            CrossSettings.Current.Set("UltimaLocalizacaoValida", position);
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
                BuscaPosicao(position);
        }



        /// <summary>
        /// The isLocalizacaoDiferente
        /// </summary>
        /// <param name="localizacao1">The localizacao1<see cref="Position"/></param>
        /// <param name="localizacao2">The localizacao2<see cref="Position"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool isLocalizacaoDiferente(Position localizacao1, Position localizacao2)
        {
            //var lat1 = (Math.Truncate(localizacao1.Latitude * 100000) / 100000);
            //var lat2 = (Math.Truncate(localizacao2.Latitude * 100000) / 100000);
            //var long1 = (Math.Truncate(localizacao1.Longitude * 100000) / 100000);
            //var long2 = (Math.Truncate(localizacao2.Longitude * 100000) / 100000);

            double distancia = Location.CalculateDistance(new Location(localizacao1.Latitude, localizacao1.Longitude),
                       new Location(localizacao2.Latitude, localizacao2.Longitude), DistanceUnits.Kilometers);

            // return lat1 != lat2 || long1 != long2;

            return distancia > 0.02;
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

            DesconectarSocket();
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
