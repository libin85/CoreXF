
using SkiaSharp;
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public static class SkiaHelpers
    {
        public static SKRect ToSKRect(this SKRectI rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        public static SKSize ToSKSize(this SKSizeI size) => new SKSize(size.Width,size.Height);

        public static SKRect Clone(this SKRectI rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        public static SKRect Clone(this SKRect rect)
            => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        public static SKRect Clone(this SKRect rect, float PlusLeft = 0f, float PlusTop = 0f, float PlusRight = 0f, float PlusBottom = 0f)
            => new SKRect(rect.Left + PlusLeft, rect.Top + PlusTop, rect.Right + PlusRight, rect.Bottom + PlusBottom);

        public static SKRect Clone(this SKRectI rect, float PlusLeft = 0f, float PlusTop = 0f, float PlusRight = 0f, float PlusBottom = 0f)
            => new SKRect(rect.Left + PlusLeft, rect.Top + PlusTop, rect.Right + PlusRight, rect.Bottom + PlusBottom);

        public static SKRect Clone(this SKRect rect, float DecreaseAllSide)
            => new SKRect(rect.Left + DecreaseAllSide, rect.Top + DecreaseAllSide, rect.Right - DecreaseAllSide, rect.Bottom - DecreaseAllSide);

    }

}
