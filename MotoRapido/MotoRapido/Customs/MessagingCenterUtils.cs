using System;
using Xamarin.Forms;

namespace MotoRapido.Customs
{
    public class MessagingCenterUtils
    {


        public static void Subscribe<TArgs>(object subscriber, string message, Action<object, TArgs> callback, object source = null)
        {
            MessagingCenter.Subscribe(subscriber, message, callback, source);
        }

        public static void Unsubscribe<TArgs>(object subscriber, string message)
        {
            MessagingCenter.Unsubscribe<object, TArgs>(subscriber, message);
        }

        public static void Send<TArgs>(object sender, string message, TArgs args)
        {
            MessagingCenter.Send(sender, message, args);
        }
    }

}
