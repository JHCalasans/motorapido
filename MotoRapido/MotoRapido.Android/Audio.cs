using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MotoRapido.Droid;
using MotoRapido.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(Audio))]

namespace MotoRapido.Droid
{
    public class Audio : IAudio
    {
        private MediaPlayer _mediaPlayer;

        public bool PlayAudio()
        {
            _mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.alarme);
            _mediaPlayer.Start();
            return true;
        }
    }
}