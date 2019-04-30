
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public class Line : SKCanvasView
    {
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(Line), default(Color));
        public static readonly BindableProperty StrokeCapProperty = BindableProperty.Create(nameof(StrokeCap), typeof(SKStrokeCap), typeof(Line), default(SKStrokeCap));
        public static readonly BindableProperty DashFillLenghtProperty = BindableProperty.Create(nameof(DashFillLenght), typeof(float), typeof(Line), default(float));
        public static readonly BindableProperty DashEmptyLenghtProperty = BindableProperty.Create(nameof(DashEmptyLenght), typeof(float), typeof(Line), default(float));


        #region Properties

        public float DashEmptyLenght
        {
            get { return (float)GetValue(DashEmptyLenghtProperty); }
            set { SetValue(DashEmptyLenghtProperty, value); }
        }

        public float DashFillLenght
        {
            get { return (float)GetValue(DashFillLenghtProperty); }
            set { SetValue(DashFillLenghtProperty, value); }
        }

        public SKStrokeCap StrokeCap
        {
            get { return (SKStrokeCap)GetValue(StrokeCapProperty); }
            set { SetValue(StrokeCapProperty, value); }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Transparent);

            SKCanvas canvas = e.Surface.Canvas;

            using(SKPaint skPaint = new SKPaint()){
                skPaint.Style = SKPaintStyle.Stroke;
                skPaint.IsAntialias = true;
                skPaint.Color = Color.ToSKColor();
                skPaint.StrokeWidth = e.Info.Height;
                skPaint.StrokeCap = StrokeCap;

                if(DashFillLenght > 0)
                {
                    skPaint.PathEffect = SKPathEffect.CreateDash(new float[] { DashFillLenght, DashEmptyLenght }, DashFillLenght + DashEmptyLenght);
                }

                canvas.DrawLine(0, e.Info.Height / 2, e.Info.Width - 1, e.Info.Height / 2, skPaint);
            }
           
        }
    }
}
