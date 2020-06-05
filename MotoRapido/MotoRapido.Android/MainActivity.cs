using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Matcha.BackgroundService.Droid;
using Plugin.Permissions;
using Prism.Unity;
using Unity;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Android;
using static Android.Provider.Settings;

[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyCXnSw7uj9P9oZIc_7c74peSmkmkYU1O5s")]
namespace MotoRapido.Droid
{
    [Activity(Label = "MotoRapido", Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            BackgroundAggregator.Init(this);

            base.OnCreate(bundle);


            Forms.SetFlags("FastRenderers_Experimental");


            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);
            UserDialogs.Init(this);
            // Xamarin.FormsMaps.Init(this, bundle);
            //  Xamarin.FormsGoogleMaps.Init(this, bundle); 



            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            };

            Xamarin.FormsGoogleMaps.Init(this, bundle, platformConfig); // initialize for Xamarin.Forms.GoogleMaps
            Xamarin.FormsGoogleMapsBindings.Init();
            Xamarin.Essentials.Platform.Init(this, bundle);
            // CrossCurrentActivity.Current.Init(this, bundle);

            LocationManager mlocManager = (LocationManager)GetSystemService(LocationService);
            App.IsGPSEnable = mlocManager.IsProviderEnabled(LocationManager.GpsProvider);
            ObterID();


            LoadApplication(new App(new AndroidInitializer()));

        }



        protected void ObterID()
        {
            string id = string.Empty;

            if (!string.IsNullOrWhiteSpace(id))
                App.DeviceID = id;

            id = Android.OS.Build.Serial;
            if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
            {
                try
                {
                    var context = Android.App.Application.Context;
                    id = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                }
                catch (System.Exception ex)
                {
                    Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                }
            }

            App.DeviceID = id;
        }


        protected override void OnResume()
        {
            base.OnResume();
            LocationManager mlocManager = (LocationManager)GetSystemService(LocationService);
            App.IsGPSEnable = mlocManager.IsProviderEnabled(LocationManager.GpsProvider);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }


    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            // Register any platform specific implementations
        }
    }
}

