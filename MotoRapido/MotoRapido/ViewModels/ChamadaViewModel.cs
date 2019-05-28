using Acr.Settings;
using Acr.UserDialogs;
using Matcha.BackgroundService;
using MotoRapido.Models;
using MotoRapido.Renderers;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;

namespace MotoRapido.ViewModels
{
    public class ChamadaViewModel : ViewModelBase
    {

        public DelegateCommand IniciarCorridaCommand =>
           new DelegateCommand(IniciarCorrida);


        public MoveToRegionRequest MoveToRegionReq { get; } = new MoveToRegionRequest();

        private ObservableCollection<Pin> _pins;
        public ObservableCollection<Pin> Pins
        {
            get { return _pins; }
            set { SetProperty(ref _pins, value); }
        }

        private ObservableCollection<Polyline> _polylines;
        public ObservableCollection<Polyline> Polylines
        {
            get { return _polylines; }
            set { SetProperty(ref _polylines, value); }
        }

        private Chamada _chamada;
        public Chamada Chamada
        {
            get { return _chamada; }
            set { SetProperty(ref _chamada, value); }
        }


        private String _textoBotaoFinal;
        public String TextoBotaoFinal
        {
            get { return _textoBotaoFinal; }
            set { SetProperty(ref _textoBotaoFinal, value); }
        }

        private Boolean _showBotaoInicio;
        public Boolean ShowBotaoInicio
        {
            get { return _showBotaoInicio; }
            set { SetProperty(ref _showBotaoInicio, value); }
        }


        public ChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
                   : base(navigationService, dialogService)
        {
            if (!CrossSettings.Current.Contains("ChamadaEmCorrida"))
            {
                TextoBotaoFinal = "Cancelar";
                ShowBotaoInicio = true;
            }
            else
            {
                CrossSettings.Current.Set("isTimerOn", true);
                iniciarTimerPosicao();
                TextoBotaoFinal = "Finalizar";
                ShowBotaoInicio = false;
            }
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            // Pins = new ObservableCollection<Pin>();
            //String polylines = null;
            if (CrossSettings.Current.Contains("ChamadaAceita"))
                Chamada = CrossSettings.Current.Get<Chamada>("ChamadaAceita");
            else if (CrossSettings.Current.Contains("ChamadaEmCorrida"))
                Chamada = CrossSettings.Current.Get<Chamada>("ChamadaEmCorrida");
            else
                return;


            AjustePosicaoMapa();



        }

        private async void AjustePosicaoMapa()
        {
            List<Position> lista = DecodePolylinePoints(Chamada.polylines);
            if (MotoristaLogado.verDestino.Equals("S"))
            {
               
                if (CrossSettings.Current.Contains("ChamadaEmCorrida"))
                {
                    Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();
                    Pin localAtualPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Atual",
                        Position = new Position(pos.Latitude, pos.Longitude),
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Gray)
                    };
                   // Pins.Add(localAtualPin);
                    MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(localAtualPin.Position, Distance.FromKilometers(6.0)));
                }
                else
                {
                    Pin inicioPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Início",
                        Position = lista[0],
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green)
                    };
                    Pins.Add(inicioPin);

                    Pin finalPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Final",
                        Position = lista[lista.Count - 1],
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.FromBundle("chegada.png")

                    };
                    Pins.Add(finalPin);

                    var polyline = new Polyline();
                    foreach (Position posi in lista)
                    {
                        polyline.Positions.Add(posi);
                    }
                    //polyline.Positions.Add(new Position(40.77d, -73.93d));
                    //polyline.Positions.Add(new Position(40.81d, -73.91d));
                    //polyline.Positions.Add(new Position(40.83d, -73.87d));
                    polyline.IsClickable = true;
                    polyline.StrokeColor = Color.Blue;
                    polyline.StrokeWidth = 2f;
                    Polylines.Add(polyline);
                    MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(lista[0], Distance.FromKilometers(6.0)));
                }


            }
            else
            {
                if (CrossSettings.Current.Contains("ChamadaEmCorrida"))
                {
                    Plugin.Geolocator.Abstractions.Position pos = await GetCurrentPosition();
                    Pin localAtualPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Atual",
                        Position = new Position(pos.Latitude, pos.Longitude),
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Blue)
                    };

                    Pin finalPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Final",
                        Position = lista[lista.Count - 1],
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.FromBundle("chegada.png")

                    };
                    Pins.Add(finalPin);

                    var polyline = new Polyline();
                    foreach (Position posi in lista)
                    {
                        polyline.Positions.Add(posi);
                    }
                    //polyline.Positions.Add(new Position(40.77d, -73.93d));
                    //polyline.Positions.Add(new Position(40.81d, -73.91d));
                    //polyline.Positions.Add(new Position(40.83d, -73.87d));
                    polyline.IsClickable = true;
                    polyline.StrokeColor = Color.Blue;
                    polyline.StrokeWidth = 2f;
                    Polylines.Add(polyline);

                    // Pins.Add(localAtualPin);
                    MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(localAtualPin.Position, Distance.FromKilometers(1.0)));
                }
                else
                {
                    Pin inicioPin = new Pin()
                    {
                        Type = PinType.Place,
                        Label = "Início",
                        Position = new Position(Double.Parse(Chamada.latitudeOrigem), Double.Parse(Chamada.longitudeOrigem)),
                        IsDraggable = true,
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green)

                    };
                    Pins.Add(inicioPin);
                    MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(inicioPin.Position, Distance.FromKilometers(1.0)));
                }

            }
        }


        private List<Position> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Position> poly = new List<Position>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Position p = new Position(Convert.ToDouble(currentLat) / 100000.0, Convert.ToDouble(currentLng) / 100000.0);
                    //p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                    //p.Longitude = Convert.ToDouble(currentLng) / 100000.0;
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                // logo it
            }
            return poly;
        }


        private async void IniciarCorrida()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Carregando...");

                SelecaoChamadaParam param = new SelecaoChamadaParam();
                param.chamada = Chamada;
                param.inicioCorrida = DateTime.Now;

                var json = JsonConvert.SerializeObject(param);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                TextoBotaoFinal = "Finalizar";
                ShowBotaoInicio = false;
                //Insiro uma chamada em corrida na memória e retio a chamada aceita
                CrossSettings.Current.Set("ChamadaEmCorrida", CrossSettings.Current.Get<Chamada>("ChamadaAceita"));
                CrossSettings.Current.Remove("ChamadaAceita");
                MessagingCenter.Subscribe<ViewModelBase, Plugin.Geolocator.Abstractions.Position>(this, "MudancaPin",
                    (sender, arg) =>
                    {
                        Pins.RemoveAt(0);
                        Pins.Add(new Pin()
                        {
                            Type = PinType.Place,
                            Label = "Início",
                            Position = new Position(Double.Parse(Chamada.latitudeOrigem), Double.Parse(Chamada.longitudeOrigem)),
                            IsDraggable = true,
                            Icon = BitmapDescriptorFactory.DefaultMarker(Color.Gray)
                        });
                    });

                var response = await IniciarCliente(true).PostAsync("motorista/iniciarCorrida", content);

                if (response.IsSuccessStatusCode)
                {
                    var respStr = await response.Content.ReadAsStringAsync();



                    // var navParam = new NavigationParameters();
                    // navParam.Add("chamadaAceita", chamada);

                }
                else
                {
                    //CrossSettings.Current.Set("ChecagemChamada", param);
                    // BackgroundAggregatorService.StartBackgroundService();
                    await DialogService.DisplayAlertAsync("Aviso", response.Content.ReadAsStringAsync().Result, "OK");
                }
            }
            catch (Exception e)
            {
                await DialogService.DisplayAlertAsync("Aviso", "Falha ao iniciar corrida", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }



    }

}
