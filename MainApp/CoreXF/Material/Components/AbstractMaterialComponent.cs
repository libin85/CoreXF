
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace CoreXF
{
    public abstract class AbstractMaterialComponent : BindableObject
    {
        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(AbstractMaterialComponent), true);

        public static readonly BindableProperty VerticalOptionsProperty = BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(AbstractMaterialComponent), LayoutOptions.Fill);
        public static readonly BindableProperty HorizontalOptionsProperty = BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutOptions), typeof(AbstractMaterialComponent), LayoutOptions.Fill);
        public static readonly BindableProperty MarginProperty = BindableProperty.Create(nameof(Margin), typeof(Thickness), typeof(AbstractMaterialComponent), default(Thickness));
        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create(nameof(HeightRequest), typeof(double), typeof(AbstractMaterialComponent), -1d);
        public static readonly BindableProperty WidthRequestProperty = BindableProperty.Create(nameof(WidthRequest), typeof(double), typeof(AbstractMaterialComponent), -1d);
        public static readonly BindableProperty TranslationXProperty = BindableProperty.Create(nameof(TranslationX), typeof(double), typeof(AbstractMaterialComponent), default(double));
        public static readonly BindableProperty TranslationYProperty = BindableProperty.Create(nameof(TranslationY), typeof(double), typeof(AbstractMaterialComponent), default(double));

        #region Properties

        public double TranslationY
        {
            get { return (double)GetValue(TranslationYProperty); }
            set { SetValue(TranslationYProperty, value); }
        }

        public double TranslationX
        {
            get { return (double)GetValue(TranslationXProperty); }
            set { SetValue(TranslationXProperty, value); }
        }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public double WidthRequest
        {
            get { return (double)GetValue(WidthRequestProperty); }
            set { SetValue(WidthRequestProperty, value); }
        }

        public double HeightRequest
        {
            get { return (double)GetValue(HeightRequestProperty); }
            set { SetValue(HeightRequestProperty, value); }
        }

        public LayoutOptions HorizontalOptions
        {
            get { return (LayoutOptions)GetValue(HorizontalOptionsProperty); }
            set { SetValue(HorizontalOptionsProperty, value); }
        }

        public LayoutOptions VerticalOptions
        {
            get { return (LayoutOptions)GetValue(VerticalOptionsProperty); }
            set { SetValue(VerticalOptionsProperty, value); }
        }

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        #endregion

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

        public AbstractMaterialComponent() { }

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

        internal void OnPaintSurfaceAfterContentInternal(SKPaintSurfaceEventArgs e)
        {
            if (!IsVisible)
                return;

            float scalingFactor = (float)Device.info.ScalingFactor;

            if (TranslationX != 0 || TranslationY != 0)
            {
                e.Surface.Canvas.Translate((float)TranslationX * scalingFactor, (float)TranslationY * scalingFactor);
            }

            OnPaintSurfaceAfterContent(e, _targetRect);

            if (TranslationX != 0 || TranslationY != 0)
            {
                e.Surface.Canvas.Translate(-(float)TranslationX * scalingFactor, -(float)TranslationY * scalingFactor);
            }

        }

        public virtual void OnPaintSurfaceBeforeContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
        }
        public virtual void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(IsVisible):
                    SKParentView?.InvalidateSurface();
                    break;
            }
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
