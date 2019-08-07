using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using MotoRapido.Droid;
using MotoRapido.Interfaces;

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