using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    //SKSurface surface = e.Surface;
    //SKImage image = e.Surface.Snapshot();

    [ContentProperty(nameof(ImageSrc))]
    public class SvgToImageSourceExtension : IMarkupExtension
    {
        public string ImageSrc { get; set; }

        public double SizeX { get; set; } = 25;
        public double SizeY { get; set; } = 25;
        public Aspect Aspect { get; set; }
        public double Scale { get; set; } = 1;


        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(ImageSrc))
                return null;

            string key = $"{ImageSrc}{SizeX}{SizeY}";

            SKData skData = SKDataCache.Instance.GetValue(key);
            if(skData == null)
            {
                double scalingFactor = Device.info.ScalingFactor;

                SKSizeI sizeI = new SKSizeI((int)(SizeX * scalingFactor), (int)(SizeY * scalingFactor));

                SKPicture svgPicture = SKSvgCache.Instance.GetOrCreateValue(ImageSrc);

                SKRect targetRect = new SKRect(0, 0, sizeI.Width - 1, sizeI.Height - 1);
                SKMatrix matrix = MaterialImages.CalculateMatrix(Aspect, targetRect, svgPicture.CullRect, (float)Scale);

                var svgImage = SKImage.FromPicture(svgPicture, sizeI, matrix);

                skData = svgImage.Encode(SKEncodedImageFormat.Png, 100);

                SKDataCache.Instance.AddValue(key, skData);
            }

            return  ImageSource.FromStream(() => skData.AsStream());
        }
    }
}
