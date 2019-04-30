
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using SkiaSharp;

namespace CoreXF
{
    abstract public class MCAbstractView : View
    {
        public MCAbstractView()
        {
            IsPlatformEnabled = true;
            IsNativeStateConsistent = true;
        }

        public SKCanvasView SKParentView
        {
            get
            {
                if (_skParentView == null)
                    return null;

                _skParentView.TryGetTarget(out SKCanvasView canvas);
                return canvas;
            }
            set
            {
                _skParentView = new WeakReference<SKCanvasView>(value);
                OnSetParent();
            }
        }
        WeakReference<SKCanvasView> _skParentView;

        public virtual void OnSetParent()
        {
        }

        SKRect _targetRect;
        internal void OnPaintSurfaceBeforeContentInternal(SKPaintSurfaceEventArgs e)
        {
            if (!IsVisible)
                return;

            float scalingFactor = (float)Device.info.ScalingFactor;

            if (TranslationX != 0 || TranslationY != 0)
            {
                e.Surface.Canvas.Translate((float)TranslationX * scalingFactor, (float)TranslationY * scalingFactor);
            }

            _targetRect = CalculateCoordinates(e.Info.Rect.ToSKRect());

            OnPaintSurfaceBeforeContent(e, _targetRect);

            if (TranslationX != 0 || TranslationY != 0)
            {
                e.Surface.Canvas.Translate(-(float)TranslationX * scalingFactor, -(float)TranslationY * scalingFactor);
            }

        }

        internal void OnPaintSurfaceAfterContentInternal(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            float scalingFactor = (float)Device.info.ScalingFactor;
            OnPaintSurfaceAfterContent(e, new SKRect((float)X * scalingFactor, (float)Y * scalingFactor, (float)(Width + X) * scalingFactor, (float)(Height + Y) * scalingFactor));
        }


        public virtual void OnPaintSurfaceBeforeContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
        }

        public virtual void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
        }

        public SKRect CalculateCoordinates(SKRect targetRect)
        {
            SKRect coord = targetRect.Clone();
            float scalingFactor = (float)Device.info.ScalingFactor;

            // Horizontal
            switch (HorizontalOptions.Alignment)
            {
                case LayoutAlignment.Fill:
                    break;

                case LayoutAlignment.Start:
                    if (WidthRequest > 0)
                    {
                        coord.Right = targetRect.Left + (float)WidthRequest * scalingFactor;
                    }
                    break;

                case LayoutAlignment.End:
                    if (WidthRequest > 0)
                    {
                        coord.Left = targetRect.Right - (float)WidthRequest * scalingFactor;
                    }
                    break;

                case LayoutAlignment.Center:
                    if (WidthRequest > 0)
                    {
                        float halfOfTarget = (targetRect.Right - targetRect.Left) / 2;
                        float halfOfView = (float)WidthRequest * scalingFactor / 2;
                        coord.Left = coord.Left + halfOfTarget - halfOfView;
                        coord.Right = coord.Right - halfOfTarget + halfOfView;
                    }
                    break;
            }

            // Vertical
            switch (VerticalOptions.Alignment)
            {
                case LayoutAlignment.Fill:
                    break;

                case LayoutAlignment.Start:
                    if (HeightRequest > 0)
                    {
                        coord.Bottom = targetRect.Top + (float)HeightRequest * scalingFactor;
                    }
                    break;

                case LayoutAlignment.End:
                    if (HeightRequest > 0)
                    {
                        coord.Top = targetRect.Bottom - (float)HeightRequest * scalingFactor;
                    }
                    break;

                case LayoutAlignment.Center:
                    if (HeightRequest > 0)
                    {
                        float halfOfTarget = (targetRect.Bottom - targetRect.Top) / 2;
                        float halfOfView = ((float)HeightRequest * scalingFactor) / 2;
                        coord.Top = coord.Top + halfOfTarget - halfOfView;
                        coord.Bottom = coord.Bottom - halfOfTarget + halfOfView;
                    }
                    break;
            }

            // Margin
            if (Margin.Left != 0)
            {
                coord.Left += (float)Margin.Left * scalingFactor;
            }
            if (Margin.Right != 0)
            {
                coord.Right -= (float)Margin.Right * scalingFactor;
            }
            if (Margin.Top != 0)
            {
                coord.Top += (float)Margin.Top * scalingFactor;
            }
            if (Margin.Bottom != 0)
            {
                coord.Bottom -= (float)Margin.Bottom * scalingFactor;
            }


            return coord;
        }


    }

}
