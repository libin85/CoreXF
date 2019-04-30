
using SkiaSharp;
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public class MaterialImages
    {

        enum shape { horizontal, vertical }
        public static SKMatrix CalculateMatrix(Aspect Aspect, SKRect targetRect, SKRect srcRect, float ScaleImage = 1)
        {
            shape targetShape = targetRect.Height > targetRect.Width ? shape.vertical : shape.horizontal;
            shape srcShape = srcRect.Height > srcRect.Width ? shape.vertical : shape.horizontal;

            switch (Aspect)
            {
                case Aspect.AspectFit:
                    float scale = 0;
                    if ((targetShape == shape.horizontal && targetShape == shape.horizontal) || (targetShape == shape.vertical && targetShape == shape.vertical))
                    {
                        float scaleH = targetRect.Height / srcRect.Height;
                        float scaleW = targetRect.Width / srcRect.Width;
                        scale = Math.Min(scaleH, scaleW);
                    }
                    else
                    {
                        float canvasMin = Math.Min(targetRect.Width, targetRect.Height);
                        float srcMax = Math.Max(srcRect.Width, srcRect.Height);
                        scale = canvasMin / srcMax * ScaleImage;
                    }

                    scale = scale * ScaleImage;
                    SKMatrix matrix = SKMatrix.MakeScale(scale, scale);
                    SKRect targetImageRect = new SKRect(0, 0, srcRect.Width * scale, srcRect.Height * scale);
                    if (targetRect.Width > targetImageRect.Width)
                    {
                        matrix.TransX = (targetRect.Width - targetImageRect.Width) / 2;
                    }
                    if (targetRect.Height > targetImageRect.Height)
                    {
                        matrix.TransY = (targetRect.Height - targetImageRect.Height) / 2;
                    }
                    return matrix;

                case Aspect.AspectFill:
                case Aspect.Fill:
                    float scaleX = targetRect.Width / srcRect.Width * ScaleImage;
                    float scaleY = targetRect.Height / srcRect.Height * ScaleImage;
                    SKMatrix matrix2 = SKMatrix.MakeScale(scaleX, scaleY);
                    return matrix2;

                default:
                    return new SKMatrix();
            }

        }

    }

}
