using Android.App;
using Android.Locations;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Droid;
using MotoRapido.Interfaces;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(ChecarGPSAndroid))]
namespace MotoRapido.Droid
{
    public class ChecarGPSAndroid : Activity, IChecarGPS
    {
        public void EstaHabilitado()
        {
            try
            {
                LocationManager mlocManager = (LocationManager)GetSystemService(LocationService);
                App.IsGPSEnable = mlocManager.IsProviderEnabled(LocationManager.GpsProvider);
            }catch(Exception e)
            {
                Crashes.TrackError(e);
            }
            
        }

        
    }
}