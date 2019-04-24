using Acr.Settings;
using MotoRapido.Renderers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Bindings;

namespace MotoRapido.ViewModels
{
    public class ChamadaViewModel : ViewModelBase
    {

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


        public ChamadaViewModel(INavigationService navigationService, IPageDialogService dialogService)
                   : base(navigationService, dialogService)
        {
            

          


            //if (CrossSettings.Current.Contains("ExisteChamada"))            
            //    CrossSettings.Current.Remove("ExisteChamada");

            //UrlMapa = "https://www.google.com/maps/search/?api=1&query=-10.950752,-37.069523";
            ////"http://maps.google.com/maps?&daddr=-10.950752,-37.069523";


        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            // Pins = new ObservableCollection<Pin>();

            

            String polylines = null;
            if (parameters.ContainsKey("polylines_encoded"))
                polylines = (String)parameters["polylines_encoded"];

            if (MotoristaLogado.verDestino.Equals("S"))
            {
                List<Position> lista = DecodePolylinePoints(polylines);

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
            else
            {
                Pin inicioPin = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Início",
                    Position = new Position(-10.903183, -37.077807),
                    IsDraggable = true,
                    Icon = BitmapDescriptorFactory.DefaultMarker(Color.Green)
                };
                Pins.Add(inicioPin);
                MoveToRegionReq.MoveToRegion(MapSpan.FromCenterAndRadius(inicioPin.Position, Distance.FromKilometers(6.0)));
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



    }

}
