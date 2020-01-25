using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MotoRapido.Customs
{
    [ContentProperty("Source")]
    public class EmbeddedImage : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;
            

            var imageSource = ImageSource.FromResource(Source);
            return imageSource;
        }
    }
}
