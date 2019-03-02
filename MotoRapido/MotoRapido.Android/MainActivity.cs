using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Prism.Unity;
using Unity;
using Xamarin.Forms.GoogleMaps.Android;

namespace MotoRapido.Droid
{
    [Activity(Label = "MotoRapido", Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            UserDialogs.Init(this);
            // Xamarin.FormsMaps.Init(this, bundle);
            //  Xamarin.FormsGoogleMaps.Init(this, bundle);

            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            };

            Xamarin.FormsGoogleMaps.Init(this, bundle, platformConfig); // initialize for Xamarin.Forms.GoogleMaps
            // CrossCurrentActivity.Current.Init(this, bundle);

            LocationManager mlocManager = (LocationManager)GetSystemService(LocationService); ;
            App.IsGPSEnable = mlocManager.IsProviderEnabled(LocationManager.GpsProvider);

            LoadApplication(new App(new AndroidInitializer()));

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

