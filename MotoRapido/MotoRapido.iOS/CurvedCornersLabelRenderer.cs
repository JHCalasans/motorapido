using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MotoRapido.iOS;
using MotoRapido.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CurvedCornersLabel), typeof(CurvedCornersLabelRenderer))]
namespace MotoRapido.iOS
{
    public class CurvedCornersLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var xfViewReference = (CurvedCornersLabel)Element;

                // Radius for the curves
                this.Layer.CornerRadius = (float)xfViewReference.CurvedCornerRadius;

                this.Layer.BackgroundColor = xfViewReference.CurvedBackgroundColor.ToCGColor();
            }
        }
    }
}