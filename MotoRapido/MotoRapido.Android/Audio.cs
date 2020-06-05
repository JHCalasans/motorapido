using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
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

            _mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.new_alarms);
            _mediaPlayer.SetVolume(1f, 1f);
            _mediaPlayer.Start();
            return true;
        }

        public bool PlayAudioChat()
        {
            _mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.message_alarm);
            _mediaPlayer.SetVolume(1f, 1f);
            _mediaPlayer.Start();
            return true;
        }
    }
}