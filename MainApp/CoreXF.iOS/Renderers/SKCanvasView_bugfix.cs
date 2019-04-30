using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using CoreGraphics;
using CoreXF;
using Foundation;
using SkiaSharp.Views.Forms;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

// bugfix for 
// https://github.com/mono/SkiaSharp/issues/759

[assembly: ExportRenderer(typeof(Material), typeof(CoreXF.iOS.MySKCanvasViewRenderer))]
namespace CoreXF.iOS
{

    public class  MySKCanvasView : SkiaSharp.Views.iOS.SKCanvasView
    {
        public override void Draw(CGRect rect)
        {
            if(rect.Height < 1 || rect.Width < 1)
            {
                Debug.WriteLine($"Canvas {rect.Height}x{rect.Width},   Bounds {Bounds.Height}x{Bounds.Width}");
                Debug.WriteLine($"Skip!");
                return;
            }

            base.Draw(rect);
        }
    }

    public class MySKCanvasViewRenderer : SKCanvasViewRenderer
    {

        public MySKCanvasViewRenderer() : base()
        {
        }
        protected override SkiaSharp.Views.iOS.SKCanvasView CreateNativeControl()
        {
            var view = new MySKCanvasView();
            view.Opaque = false;
            return view;
        }

    }
}
