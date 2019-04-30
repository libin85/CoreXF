
using Xamarin.Forms;
using System;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Runtime.CompilerServices;

namespace CoreXF
{

    public class Rating : SKCanvasView
    {

        public int TotalStars { get; set; } = 5;

        public static readonly BindableProperty EditableProperty = BindableProperty.Create("Editable", typeof(bool), typeof(Rating), default(bool));
        public static readonly BindableProperty RateProperty = BindableProperty.Create("Rate", typeof(float), typeof(Rating), 0f);
        public static readonly BindableProperty StarMarginProperty = BindableProperty.Create("StarMargin", typeof(Thickness), typeof(Rating), default(Thickness));
        public static readonly BindableProperty GoldStarsImageProperty = BindableProperty.Create("GoldStarsImage", typeof(string), typeof(Rating), default(string));
        public static readonly BindableProperty GrayStarsImageProperty = BindableProperty.Create("GrayStarsImage", typeof(string), typeof(Rating), default(string));

        #region Variables

        public string GrayStarsImage
        {
            get { return (string)GetValue(GrayStarsImageProperty); }
            set { SetValue(GrayStarsImageProperty, value); }
        }

        public string GoldStarsImage
        {
            get { return (string)GetValue(GoldStarsImageProperty); }
            set { SetValue(GoldStarsImageProperty, value); }
        }

        public Thickness StarMargin
        {
            get { return (Thickness)GetValue(StarMarginProperty); }
            set { SetValue(StarMarginProperty, value); }
        }

        public float Rate
        {
            get => (float?)GetValue(RateProperty) ?? 0;
            set => SetValue(RateProperty, value);
        }
        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }
        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {

            if (TotalStars == 0 || string.IsNullOrEmpty(GoldStarsImage) || string.IsNullOrEmpty(GrayStarsImage) || e.Info.Width < 20)
                return;

            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            var bitmapGold = SKBitmapCache.Instance.GetOrCreateValue(GoldStarsImage);
            var bitmapGray = SKBitmapCache.Instance.GetOrCreateValue(GrayStarsImage);

            float width = e.Info.Width / TotalStars;
            float height = e.Info.Height;
            float gold_star_count = (float)Math.Floor(Rate);

            float heightCorrection_half = height > width ? (height - width) / 2 : 0;
            float widthCorrection_half = width > height ? (width - height) / 2 : 0;


            canvas.Clear();
            int i = 0;
            // gold stars
            for (; i < gold_star_count; i++)
            {
                canvas.DrawBitmap(bitmapGold, new SKRect(
                    i * width + (float)StarMargin.Left + widthCorrection_half, // left
                    0 + (float)StarMargin.Top + heightCorrection_half,  // top
                    i * width + width - (float)StarMargin.Right - widthCorrection_half, // right
                    height - (float)StarMargin.Bottom - heightCorrection_half)); // bottom
            }
            // half star
            if (Rate - gold_star_count > 0)
            {
                float leftpart = Rate - gold_star_count;
                canvas.DrawBitmap(bitmapGold,
                    new SKRect(0, 0, bitmapGray.Width * leftpart, bitmapGray.Height),
                    new SKRect(
                        i * width + (float)StarMargin.Left + widthCorrection_half, // left 
                        0 + (float)StarMargin.Top + heightCorrection_half,  // top 
                        i * width + width * leftpart,
                        height - (float)StarMargin.Bottom - heightCorrection_half)); // bottom

                canvas.DrawBitmap(bitmapGray,
                    new SKRect(bitmapGray.Width * leftpart, 0, bitmapGray.Width, bitmapGray.Height),
                    new SKRect(
                        i * width + width * leftpart,
                        0 + (float)StarMargin.Top + heightCorrection_half,  // top 
                        i * width + width - (float)StarMargin.Right - widthCorrection_half, // right 
                        height - (float)StarMargin.Bottom - heightCorrection_half)); // bottom

                i++;
            }
            // gray stars
            for (; i < TotalStars; i++)
            {
                canvas.DrawBitmap(bitmapGray, new SKRect(
                    i * width + (float)StarMargin.Left + widthCorrection_half, // left
                    0 + (float)StarMargin.Top + heightCorrection_half,  // top
                    i * width + width - (float)StarMargin.Right - widthCorrection_half, // right
                    height - (float)StarMargin.Bottom - heightCorrection_half)); // bottom
            }

        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(TotalStars):
                    InvalidateSurface();
                    break;

                case nameof(Rate):
                    InvalidateSurface();
                    break;

                case nameof(Editable):
                    EnableTouchEvents = Editable;
                    break;

                default:
                    base.OnPropertyChanged(propertyName);
                    break;
            }
        }


        protected override void OnTouch(SKTouchEventArgs e)
        {
            Rate = e.Location.X == 0 || TotalStars == 0 ? 0 : (float)Math.Ceiling(e.Location.X / (CanvasSize.Width / TotalStars));
        }
    }

}
