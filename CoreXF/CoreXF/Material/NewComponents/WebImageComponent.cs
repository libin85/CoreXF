
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace CoreXF
{
    public class WebImageComponent : MCAbstractView, IBindableImage
    {
        UriImageSource Source;

        public MaterialRawImage RawImage { get; } = new MaterialRawImage();

        public static readonly BindableProperty ImageSrcProperty = BindableProperty.Create(nameof(ImageSrc), typeof(string), typeof(Image), default(string));
        public static readonly BindableProperty ImageTintColorProperty = BindableProperty.Create(nameof(ImageTintColor), typeof(Color), typeof(Image), Material.NoColor);
        public static readonly BindableProperty ImageAspectProperty = BindableProperty.Create(nameof(ImageAspect), typeof(Aspect), typeof(Image), Aspect.AspectFit);
        public static readonly BindableProperty ImageScaleProperty = BindableProperty.Create(nameof(ImageScale), typeof(float), typeof(Image), 1f);
        public static readonly BindableProperty ImageTransXProperty = BindableProperty.Create(nameof(ImageTransX), typeof(float), typeof(Material), default(float));
        public static readonly BindableProperty ImageTransYProperty = BindableProperty.Create(nameof(ImageTransY), typeof(float), typeof(Material), default(float));

        #region Properties

        public float ImageTransY
        {
            get { return (float)GetValue(ImageTransYProperty); }
            set { SetValue(ImageTransYProperty, value); }
        }

        public float ImageTransX
        {
            get { return (float)GetValue(ImageTransXProperty); }
            set { SetValue(ImageTransXProperty, value); }
        }


        public float ImageScale
        {
            get { return (float)GetValue(ImageScaleProperty); }
            set { SetValue(ImageScaleProperty, value); }
        }

        public Aspect ImageAspect
        {
            get { return (Aspect)GetValue(ImageAspectProperty); }
            set { SetValue(ImageAspectProperty, value); }
        }

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        public Color ImageTintColor
        {
            get { return (Color)GetValue(ImageTintColorProperty); }
            set { SetValue(ImageTintColorProperty, value); }
        }

        #endregion

        public WebImageComponent()
        {
            Source = new UriImageSource
            {
                CachingEnabled = true
            };
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(ImageSrc):
                    Task.Run(()=>SetImageSrcAsync());
                    break;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            RawImage.Reset();

        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            double width = this.WidthRequest;
            if (width <= 0 || width == double.PositiveInfinity)
                width = 50;

            double height = this.HeightRequest;
            if (height <= 0 || height == double.PositiveInfinity)
                height = 50;

            Size size = new Size { Width = width, Height = height };
            return new SizeRequest(size, size);
        }

        async void SetImageSrcAsync()
        {
            try
            {
                

                if (string.IsNullOrEmpty(ImageSrc) || !IsVisible)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        RawImage.Reset();
                        SKParentView?.InvalidateSurface();
                    });
                    return;
                }

                Source.Uri = new Uri(ImageSrc);
                using (var stream = await Source.GetStreamAsync())
                {
                    if (stream == null)
                        return;

                    RawImage.Add(SKBitmap.Decode(stream), DisposeMode.Dispose);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        SKParentView?.InvalidateSurface();
                    });
                }
            }
            catch (TaskCanceledException ex)
            {
            }
        }

        public override void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            if (RawImage.HasImage)
            {
                Material.DrawImage(e.Surface, targetRect, this);
            }
            else
            {
                e.Surface.Canvas.Clear(BackgroundColor.ToSKColor());
            }
        }

    }
}
