
using Android.App;
using Android.Content;
using Android.OS;

namespace MotoRapido.Droid
{
    [Service]
    public class LocalService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }
    }
}