using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace CoreXF
{
    public class BoxComponent : AbstractMaterialComponent
    {

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(BoxComponent), default(Color));

        #region Properties

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        public override void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e,SKRect targetRect)
        {
            using(SKPaint paint = new SKPaint())
            {
                paint.Color = Color.ToSKColor();
                e.Surface.Canvas.DrawRect(targetRect, paint);
            }

        }

    }
}
