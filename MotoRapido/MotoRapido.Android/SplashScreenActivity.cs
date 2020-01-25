using Acr.UserDialogs.Infrastructure;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace MotoRapido.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashScreenActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashScreenActivity.OnCreate");
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            // Task startupWork = new Task(() => { SimulateStartup(); });
            // startupWork.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

    }
}