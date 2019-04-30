using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace CoreXF
{
    public class RectangleComponent : AbstractMaterialComponent, IBindableRectangle
    {
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create("Radius", typeof(float), typeof(RectangleComponent), -1f);
        public static readonly BindableProperty MainColorProperty = BindableProperty.Create(nameof(MainColor), typeof(Color), typeof(RectangleComponent), Color.White);
        public static readonly BindableProperty MainDisabledColorProperty = BindableProperty.Create(nameof(MainDisabledColor), typeof(Color), typeof(RectangleComponent), Material.NoColor);

        public static readonly BindableProperty DisabledProperty = BindableProperty.Create(nameof(Disabled), typeof(bool), typeof(RectangleComponent), default(bool));

        // shadow
        public static readonly BindableProperty ShadowWidthProperty = BindableProperty.Create("ShadowWidth", typeof(float), typeof(RectangleComponent), 0f);
        public static readonly BindableProperty ShadowStyleProperty = BindableProperty.Create("ShadowStyle", typeof(ShadowStyle), typeof(RectangleComponent), ShadowStyle.RightBottom);
        public static readonly BindableProperty ShadowBlurAmountProperty = BindableProperty.Create("ShadowBlurAmount", typeof(float), typeof(RectangleComponent), 0f);
        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create("ShadowColor", typeof(Color), typeof(RectangleComponent), Color.FromHex("#60000000"));
        
        // stroke
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create("StrokeColor", typeof(Color), typeof(RectangleComponent), default(Color));
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create("StrokeWidth", typeof(float), typeof(RectangleComponent), 0f);


        #region Properties

        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }
        public Color MainDisabledColor
        {
            get { return (Color)GetValue(MainDisabledColorProperty); }
            set { SetValue(MainDisabledColorProperty, value); }
        }


        public Color MainColor
        {
            get { return (Color)GetValue(MainColorProperty); }
            set { SetValue(MainColorProperty, value); }
        }

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }


        public float StrokeWidth
        {
            get { return (float)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }


        public float Radius
        {
            get { return (float)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public float ShadowWidth
        {
            get { return (float)GetValue(ShadowWidthProperty); }
            set { SetValue(ShadowWidthProperty, value); }
        }


        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        public ShadowStyle ShadowStyle
        {
            get { return (ShadowStyle)GetValue(ShadowStyleProperty); }
            set { SetValue(ShadowStyleProperty, value); }
        }

        public float ShadowBlurAmount
        {
            get { return (float)GetValue(ShadowBlurAmountProperty); }
            set { SetValue(ShadowBlurAmountProperty, value); }
        }

        #endregion

        public override void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            Material.DrawMaterialFrame(e.Surface, targetRect, this);
        }

    }
}
