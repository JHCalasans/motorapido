using Xamarin.Forms;

namespace MotoRapido.Views
{
    public partial class Chamada : ContentPage
    {
        public Chamada()
        {
            InitializeComponent();

           // map.MoveCamera(CameraUpdateFactory.NewPosition(new Position(-10.950752, -37.069523)));
           
          //  map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;


            //var polyline = new Polyline();
            //polyline.Positions.Add(new Position(40.77d, -73.93d));
            //polyline.Positions.Add(new Position(40.81d, -73.91d));
            //polyline.Positions.Add(new Position(40.83d, -73.87d));
            //polyline.IsClickable = true;
            //polyline.StrokeColor = Color.Blue;
            //polyline.StrokeWidth = 3f;
            //map.Polylines.Add(polyline);

            //var pinNewYork = new Pin()
            //{
            //    Type = PinType.Place,
            //    Label = "Central Park NYC",
            //    Address = "New York City, NY 10022",
            //    Position = new Position(40.78d, -73.96d),
            //    IsDraggable = true
            //};
            //map.Pins.Add(pinNewYork);
            //map.SelectedPin = pinNewYork;



           // map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(40.77d, -73.93d), Distance.FromKilometers(6.0)));

            // var customMap = new BindableMap();

            //ChamadaViewModel.myMap = MyMap;

            //MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-19.6303, -43.8983), Distance.FromKilometers(1.0)));

            //  ((ChamadaViewModel)BindingContext).MoveToRegion();
        }
    }
}
