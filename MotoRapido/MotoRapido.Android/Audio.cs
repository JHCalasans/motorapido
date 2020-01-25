using Android.Media;
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