
using System.Runtime.CompilerServices;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using System.Windows.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using CoreXF.NavigationAbstraction;

namespace CoreXF
{
    public enum ShadowStyle { RightBottom, LeftRightBottom, AllSides }
    public enum MaterialGestures { Pressed, PressedAndReleased }


    public interface IDisposableRegistration : IDisposable
    {
        bool NeedToRegisterForDispose { get; }
        bool RegisteredForDispose { get; set; }
    }


    #region Interfaces

    public interface IBindableRectangle
    {
        float Radius { get; set; }
        Color MainColor { get; set; }
        Color MainDisabledColor { get; set; }
        float ShadowWidth { get; set; }
        ShadowStyle ShadowStyle { get; set; }
        float ShadowBlurAmount { get; set; }
        Color ShadowColor { get; set; }
        Color StrokeColor { get; set; }
        float StrokeWidth { get; set; }
        bool Disabled { get; set; }
    }

    public interface IBindableText
    {
        string  Text { get; set; }
        Color   TextColor { get; set; }
        double  TextFontSize { get; set; }
        Color   TextDisabledColor { get; set; }
        string  TextFontFamily { get; set; }
        bool    Disabled { get; set; }
        ICommand Command { get; set; }
        object CommandParameter { get; set; }
        FontAttributes TextFontAttributes { get; set; }
    }

    public class IBindableTextClass : IBindableText
    {
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public double TextFontSize { get; set; }
        public Color TextDisabledColor { get; set; }
        public string TextFontFamily { get; set; }
        public bool Disabled { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public FontAttributes TextFontAttributes { get; set; }
    }

    public interface IBindableImage 
    {
        float ImageScale { get; set; }
        Aspect ImageAspect { get; set; }
        string ImageSrc { get; set; }
        Color ImageTintColor { get; set; }
        float ImageTransX { get; set; }
        float ImageTransY { get; set; }

        MaterialRawImage RawImage { get; }

    }

    public interface IBindableCheckable
    {
        bool IsCheckable { get; set; }
        bool IsChecked { get; set; }
    }

    public interface IBindableCircle
    {
        Color CircleColor { get; set; }
        Color CircleDisabledColor { get; set; }
        Color CircleStrokeColor { get; set; }
        double CircleStrokeWidth { get; set; }
        bool Disabled { get; set; }
    }

    public class IBindableCircleClass : IBindableCircle
    {
        public Color CircleColor { get; set; }
        public Color CircleDisabledColor { get; set; }
        public Color CircleStrokeColor { get; set; }
        public double CircleStrokeWidth { get; set; }
        public bool Disabled { get; set; }
    }

    public interface IBindableGradient
    {
        Color GradientStartColor { get; set; }
        Color GradientEndColor { get; set; }
        Material.GradientOrientationEnum GradientOrientation { get; set; }
    }

    #endregion

    [ContentProperty(nameof(Components))]
    public class Material : SKCanvasView, IBindableImage, IBindableText, IBindableCircle, IBindableCheckable, IBindableGradient, IBindableRectangle, IDisposable
    {
        public static Color NoColor = Color.FromHex("#00FF00FF");

        #region Rectangle region

        public static readonly BindableProperty RadiusProperty = BindableProperty.Create("Radius", typeof(float), typeof(Material), -1f);
        public static readonly BindableProperty MainColorProperty = BindableProperty.Create(nameof(MainColor), typeof(Color), typeof(Material), Color.White);
        public static readonly BindableProperty MainDisabledColorProperty = BindableProperty.Create(nameof(MainDisabledColor), typeof(Color), typeof(Material), Material.NoColor);

        // material
        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(nameof(Elevation), typeof(float), typeof(Material), default(float));
        public static readonly BindableProperty CanvasTranslationXProperty = BindableProperty.Create(nameof(CanvasTranslationX), typeof(double), typeof(Material), default(double));
        public static readonly BindableProperty CanvasTranslationYProperty = BindableProperty.Create(nameof(CanvasTranslationY), typeof(double), typeof(Material), default(double));

        // shadow
        public static readonly BindableProperty ShadowWidthProperty = BindableProperty.Create("ShadowWidth", typeof(float), typeof(Material), 0f);
        public static readonly BindableProperty ShadowStyleProperty = BindableProperty.Create("ShadowStyle", typeof(ShadowStyle), typeof(Material), ShadowStyle.RightBottom);
        public static readonly BindableProperty ShadowBlurAmountProperty = BindableProperty.Create("ShadowBlurAmount", typeof(float), typeof(Material), 0f);
        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create("ShadowColor", typeof(Color), typeof(Material), Color.FromHex("#60000000"));
        // stroke
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create("StrokeColor", typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create("StrokeWidth", typeof(float), typeof(Material), 0f);
        // Command
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(Material), default(ICommand));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(Material), default(object));
        public static readonly BindableProperty GestureProperty = BindableProperty.Create(nameof(Gesture), typeof(MaterialGestures), typeof(Material), MaterialGestures.Pressed);
        //
        public static readonly BindableProperty DisabledProperty = BindableProperty.Create("Disabled", typeof(bool), typeof(Material), false);

        #region Properties

        public double CanvasTranslationY
        {
            get { return (double)GetValue(CanvasTranslationYProperty); }
            set { SetValue(CanvasTranslationYProperty, value); }
        }

        public double CanvasTranslationX
        {
            get { return (double)GetValue(CanvasTranslationXProperty); }
            set { SetValue(CanvasTranslationXProperty, value); }
        }

        public Color MainDisabledColor
        {
            get { return (Color)GetValue(MainDisabledColorProperty); }
            set { SetValue(MainDisabledColorProperty, value); }
        }

        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public MaterialGestures Gesture
        {
            get { return (MaterialGestures)GetValue(GestureProperty); }
            set { SetValue(GestureProperty, value); }
        }


        public float Elevation
        {
            get { return (float)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
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

        public static SKRect DrawMaterialFrame(SKSurface surface, SKRect targetRect, IBindableRectangle view)
        {
            if (view.MainColor == Color.Transparent)
                return targetRect;

            SKCanvas canvas = surface.Canvas;

            float scaleFactor = (float)Device.Info.ScalingFactor;

            float realRadius = view.Radius * scaleFactor;
            float realShadowWidth = view.ShadowWidth * scaleFactor;
            float blurMultiplier = 2.7f;
            float shadowCorrectionMultiplier = 1.6f;
            float realShadowBlurAmount = view.ShadowBlurAmount * scaleFactor;
            float realStrokeWidth = view.StrokeWidth * scaleFactor;

            float halfRealBlurAmount = (realShadowBlurAmount == 0 ? 0 : realShadowBlurAmount / 2) + (realShadowBlurAmount > realShadowWidth ? (realShadowBlurAmount - realShadowWidth) / 2 : 0);

            SKRect shadowRect = targetRect;
            switch (view.ShadowStyle)
            {
                case ShadowStyle.AllSides:
                    shadowRect = targetRect.Clone(
                        PlusLeft: halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusRight: -halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusTop: halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusBottom: -halfRealBlurAmount);
                    break;

                case ShadowStyle.LeftRightBottom:
                    shadowRect = targetRect.Clone(
                        PlusLeft: halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusRight: -halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusTop: halfRealBlurAmount + realShadowWidth, // hide shadow
                        PlusBottom: -halfRealBlurAmount);
                    break;

                case ShadowStyle.RightBottom:
                    shadowRect = targetRect.Clone(
                        PlusLeft: realShadowWidth, // hide shadow
                        PlusRight: -halfRealBlurAmount * shadowCorrectionMultiplier,
                        PlusTop: halfRealBlurAmount + realShadowWidth, // hide shadow
                        PlusBottom: -halfRealBlurAmount);
                    break;

            }

            SKRect mainRect = targetRect.Clone(DecreaseAllSide: realShadowWidth);

            // checks
            if (realRadius > mainRect.Height)
                realRadius = mainRect.Height;

            using (SKPaint paint = new SKPaint())
            {

                // Shadow
                if (realShadowWidth > 0)
                {
                    using (SKImageFilter BlurStyle = SKImageFilter.CreateBlur(realShadowBlurAmount / blurMultiplier, realShadowBlurAmount / blurMultiplier))
                    {
                        paint.Style = SKPaintStyle.Fill;
                        paint.Color = view.ShadowColor.ToSKColor();
                        paint.BlendMode = SKBlendMode.SrcOver;
                        //fillPaint.IsAntialias = true;

                        if (realShadowBlurAmount > 0)
                            paint.ImageFilter = BlurStyle;
                        canvas.DrawRoundRect(shadowRect, realRadius, realRadius, paint);
                    }
                }

                // Stroke
                if (realStrokeWidth > 0)
                {
                    SKRect strokeRect = mainRect.Clone(DecreaseAllSide: realStrokeWidth / 2);
                    mainRect = mainRect.Clone(DecreaseAllSide: realStrokeWidth);

                    paint.Color = view.StrokeColor.ToSKColor();
                    paint.StrokeWidth = realStrokeWidth;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.ImageFilter = null;
                    canvas.DrawRoundRect(strokeRect, realRadius, realRadius, paint);
                }

                // Fill
                paint.Style = SKPaintStyle.Fill;
                paint.Color = view.Disabled ? view.MainDisabledColor.ToSKColor() :  view.MainColor.ToSKColor();
                paint.BlendMode = SKBlendMode.SrcOver;
                paint.ImageFilter = null;

                canvas.DrawRoundRect(mainRect, realRadius, realRadius, paint);
            }

            return mainRect;
        }

        #endregion

        #region Image region

        /*
        enum ImageSouceRuntimeCacheStrategy
        {
            Unknown,
            GlobalCache, // for images from app resources
            PageCache    // cache inside page
        }
        */

        public static readonly BindableProperty ImageSrcProperty = BindableProperty.Create(nameof(ImageSrc), typeof(string), typeof(Image), default(string));
        public static readonly BindableProperty ImageTintColorProperty = BindableProperty.Create(nameof(ImageTintColor), typeof(Color), typeof(Image), NoColor);
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


        //static readonly GenericWeakCache<string, SKPicture> _skPictureCache = new GenericWeakCache<string, SKPicture>();
        //static readonly GenericWeakCache<string, SKBitmap> _skBitmapCache = new GenericWeakCache<string, SKPicture>();

        public MaterialRawImage RawImage => _RawImage ?? (_RawImage = new MaterialRawImage());
        MaterialRawImage _RawImage;

        HttpRequestAbstract _loadRequest {
            get {
                if (_weakLoadRequest == null)
                    return null;

                _weakLoadRequest.TryGetTarget(out HttpRequestAbstract _result);
                return _result;
            } 
            set {
                if(_weakLoadRequest != null)
                {
                    _weakLoadRequest.TryGetTarget(out HttpRequestAbstract _result);
                    _result?.Cancel();
                }

                _weakLoadRequest = new WeakReference<HttpRequestAbstract>(value);
            } }
        WeakReference<HttpRequestAbstract> _weakLoadRequest;


        string _previousImage;
        void CreateImage(string imageSrc)
        {
            string imageSource = imageSrc?.ToLower();
            if(string.Compare(imageSource,_previousImage) == 0)
            {
                return;
            }
            else
            {
                _previousImage = imageSource;
            }

            try
            {
                if (string.IsNullOrEmpty(ImageSrc))
                    return;

                // http svg
                if (imageSource.Contains("http") && imageSource.EndsWith(".svg"))
                {
                    _loadRequest = new HttpRequest(async httpClient => await httpClient.GetAsync<string>(ImageSrc))
                        .CatchException()
                        .TakePreviousResultAndRunRequest<string>(
                            key: ImageSrc,
                            applyResult: result =>
                            {
                                RawImage.Add(MaterialSvg.CreateSvgFromRawString(result as string), DisposeMode.Dispose);
                                this.InvalidateSurface();
                            },
                            ifThereIsValueInCacheDoNotExectuteHttpRequest: true);

                    _loadRequest.FireAndForget();
                }
                // http jpg or gif
                else if (imageSource.Contains("http"))
                {

                }
                // raw svg string
                else if (imageSource.Contains("<svg "))
                {
                    RawImage.Add(MaterialSvg.CreateSvgFromRawString(ImageSrc),DisposeMode.Dispose);
                    InvalidateSurface();
                }
                // svg from resources
                else if (imageSource.Contains(".svg"))
                {
                    RawImage.Add(SKSvgCache.Instance.GetOrCreateValue(imageSrc),DisposeMode.NoDispose);
                    InvalidateSurface();
                }
                // jpeg from resources
                else if (imageSource.EndsWith(".jpeg") || imageSource.EndsWith(".jpg"))
                {
                    using (var stream = ResourceLoader.GetStream(ImageSrc))
                    {
                        RawImage.Add(SKBitmap.Decode(stream),DisposeMode.Dispose);
                    }
                    InvalidateSurface();
                }
                // png from resources
                else if (imageSource.EndsWith(".png"))
                {
                    RawImage.Add(SKBitmapCache.Instance.GetOrCreateValue(ImageSrc), DisposeMode.NoDispose);
                    InvalidateSurface();
                }

                else
                {
                    throw new Exception($"Unknown image source format {ImageSrc}");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.SendError(ex);
            }

        }

        public static void DrawImage(SKSurface surface, SKRect targetRect, IBindableImage view)
        {
            if (!view.RawImage.HasImage)
                return;

            SKPicture skPicture = view.RawImage.SkPicture;
            SKBitmap skBitmap = view.RawImage.SkBitmap;

            SKCanvas canvas = surface.Canvas;

            if (view.ImageTintColor == Color.Transparent)
                return;

            bool isSvg = skPicture != null;

            IBindableCheckable iCheck = view as IBindableCheckable;

            SKRect srcRect = isSvg ? skPicture.CullRect : new SKRect(0, 0, skBitmap.Width, skBitmap.Height);
            SKMatrix matrix = MaterialImages.CalculateMatrix(view.ImageAspect, targetRect, srcRect, view.ImageScale);
            matrix.TransX += targetRect.Left + view.ImageTransX;
            matrix.TransY += targetRect.Top + view.ImageTransY;



            // blending
            // https://skia.org/user/api/SkPaint_Reference#Blend_Mode_Methods
            Color tintColor = view.ImageTintColor;
            if (tintColor != Material.NoColor)
            {
                // https://stackoverflow.com/questions/40694513/how-to-colorize-black-and-white-svg-loaded-with-skiasharp
                using (SKPaint tt = new SKPaint { ColorFilter = SKColorFilter.CreateBlendMode(tintColor.ToSKColor(), SKBlendMode.SrcIn) })
                {
                    if (isSvg)
                    {
                        canvas.DrawPicture(skPicture, ref matrix, tt);
                    }
                    else
                    {
                        canvas.SetMatrix(matrix);
                        canvas.DrawBitmap(skBitmap, 0, 0, tt);
                        canvas.ResetMatrix();
                    }

                }
            }
            else
            {
                if (isSvg)
                {
                    canvas.DrawPicture(skPicture, ref matrix);
                    
                }
                else
                {
                    canvas.SetMatrix(matrix);
                    canvas.DrawBitmap(skBitmap, 0, 0);
                    canvas.ResetMatrix();
                }
            }
        }

        #endregion

        #region Text region

        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(Material), default(string));
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(Material), Color.Black);
        public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create(nameof(TextFontSize), typeof(double), typeof(Material), 15d);
        public static readonly BindableProperty TextDisabledColorProperty = BindableProperty.Create(nameof(TextDisabledColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty TextFontFamilyProperty = BindableProperty.Create(nameof(TextFontFamily), typeof(string), typeof(Material), default(string));
        public static readonly BindableProperty TextFontAttributesProperty = BindableProperty.Create(nameof(TextFontAttributes), typeof(FontAttributes), typeof(Material), default(FontAttributes));

        #region Properties

        public FontAttributes TextFontAttributes
        {
            get { return (FontAttributes)GetValue(TextFontAttributesProperty); }
            set { SetValue(TextFontAttributesProperty, value); }
        }

        public string TextFontFamily
        {
            get { return (string)GetValue(TextFontFamilyProperty); }
            set { SetValue(TextFontFamilyProperty, value); }
        }

        public Color TextDisabledColor
        {
            get { return (Color)GetValue(TextDisabledColorProperty); }
            set { SetValue(TextDisabledColorProperty, value); }
        }
        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        public static void DrawText(SKSurface surface, SKRect targetRect, IBindableText view)
        {
            if (string.IsNullOrEmpty(view.Text))
                return;

            SKCanvas canvas = surface.Canvas;

            float scaleFactor = (float)Device.Info.ScalingFactor;

            using (var paint = new SKPaint())
            {
                paint.TextSize = (float)view.TextFontSize * scaleFactor;
                paint.IsAntialias = true;
                if ((view.TextFontAttributes & FontAttributes.Bold) > 0)
                {
                    paint.FakeBoldText = true;
                }
                paint.TextAlign = SKTextAlign.Center;
                paint.IsStroke = false;

                // Font
                if (!string.IsNullOrEmpty(view.TextFontFamily))
                {
                    paint.Typeface = SKFontCache.Instance.GetOrCreateValue(view.TextFontFamily)?.Typeface;

                }
                else
                {
                    // Hack from Andrei
                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        //view.TextFontFamily = 'Default'
                    }
                }
                paint.GetFontMetrics(out SKFontMetrics metrics);

                // Color
                Color clr = view.TextColor;
                if (view.Disabled || view.Command != null && !view.Command.CanExecute(null))
                {
                    clr = view.TextDisabledColor;
                }
                paint.Color = clr.ToSKColor();

                canvas.DrawText(view.Text,
                    targetRect.Left + targetRect.Width / 2,
                    targetRect.Top + targetRect.Height / 2 + metrics.XHeight / 2 + metrics.Descent / 2,
                    paint);

            }

        }

        #endregion

        #region Badge region

        public static readonly BindableProperty BadgeShowProperty = BindableProperty.Create(nameof(BadgeShow), typeof(bool), typeof(Material), default(bool));
        public static readonly BindableProperty BadgeColorProperty = BindableProperty.Create(nameof(BadgeColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty BadgeTextProperty = BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(Material), default(string));
        public static readonly BindableProperty BadgeTextColorProperty = BindableProperty.Create(nameof(BadgeTextColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty BadgeHeightProperty = BindableProperty.Create(nameof(BadgeHeight), typeof(double), typeof(Material), default(double));
        public static readonly BindableProperty BadgeMarginProperty = BindableProperty.Create(nameof(BadgeMargin), typeof(Thickness), typeof(Material), default(Thickness));
        public static readonly BindableProperty BadgeFontSizeProperty = BindableProperty.Create(nameof(BadgeFontSize), typeof(double), typeof(Material), 12d);
        public static readonly BindableProperty BadgeFontFamilyProperty = BindableProperty.Create(nameof(BadgeFontFamily), typeof(string), typeof(Material), default(string));
        public static readonly BindableProperty BadgeStrokeWidthProperty = BindableProperty.Create(nameof(BadgeStrokeWidth), typeof(double), typeof(Material), default(double));
        public static readonly BindableProperty BadgeStrokeColorProperty = BindableProperty.Create(nameof(BadgeStrokeColor), typeof(Color), typeof(Material), default(Color));

        public Color BadgeStrokeColor
        {
            get { return (Color)GetValue(BadgeStrokeColorProperty); }
            set { SetValue(BadgeStrokeColorProperty, value); }
        }

        public double BadgeStrokeWidth
        {
            get { return (double)GetValue(BadgeStrokeWidthProperty); }
            set { SetValue(BadgeStrokeWidthProperty, value); }
        }

        public string BadgeFontFamily
        {
            get { return (string)GetValue(BadgeFontFamilyProperty); }
            set { SetValue(BadgeFontFamilyProperty, value); }
        }

        public double BadgeFontSize
        {
            get { return (double)GetValue(BadgeFontSizeProperty); }
            set { SetValue(BadgeFontSizeProperty, value); }
        }

        public Thickness BadgeMargin
        {
            get { return (Thickness)GetValue(BadgeMarginProperty); }
            set { SetValue(BadgeMarginProperty, value); }
        }

        public double BadgeHeight
        {
            get { return (double)GetValue(BadgeHeightProperty); }
            set { SetValue(BadgeHeightProperty, value); }
        }

        public Color BadgeTextColor
        {
            get { return (Color)GetValue(BadgeTextColorProperty); }
            set { SetValue(BadgeTextColorProperty, value); }
        }

        public string BadgeText
        {
            get { return (string)GetValue(BadgeTextProperty); }
            set { SetValue(BadgeTextProperty, value); }
        }


        public Color BadgeColor
        {
            get { return (Color)GetValue(BadgeColorProperty); }
            set { SetValue(BadgeColorProperty, value); }
        }

        public bool BadgeShow
        {
            get { return (bool)GetValue(BadgeShowProperty); }
            set { SetValue(BadgeShowProperty, value); }
        }

        public static void DrawBadge(SKSurface surface, SKRect contentRect,Material view)
        {
            float scaleFactor = (float)Device.Info.ScalingFactor;
            float badgeSize = (float)view.BadgeHeight * scaleFactor;
            float marginRight = (float)view.BadgeMargin.Right * scaleFactor;
            float marginTop = (float)view.BadgeMargin.Top * scaleFactor;

            SKRect targetRect = new SKRect
            {
                Left = contentRect.Right - badgeSize - marginRight,
                Right = contentRect.Right - marginRight,
                Top = contentRect.Top + marginTop,
                Bottom = contentRect.Top + marginTop + badgeSize
            };


            // Draw circle
            Material.DrawCircle(surface, targetRect, new IBindableCircleClass
            {
                CircleColor = view.BadgeColor,
                CircleStrokeColor = view.BadgeStrokeColor,
                CircleStrokeWidth = view.BadgeStrokeWidth
            });

            // Draw text
            DrawText(surface, targetRect, new IBindableTextClass
            {
                TextColor = view.BadgeTextColor,
                Text = view.BadgeText,
                TextFontSize = view.BadgeFontSize,
                TextFontFamily = view.BadgeFontFamily
            });

        }


        #endregion

        #region Toggle region

        public static readonly BindableProperty IsCheckableProperty = BindableProperty.Create(nameof(IsCheckable), typeof(bool), typeof(Material), default(bool));
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(Material), default(bool));

        #region Properties

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }
        #endregion

        #endregion

        #region Circle region

        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create(nameof(CircleColor), typeof(Color), typeof(Material), NoColor);
        public static readonly BindableProperty CircleDisabledColorProperty = BindableProperty.Create(nameof(CircleDisabledColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty CircleStrokeWidthProperty = BindableProperty.Create(nameof(CircleStrokeWidth), typeof(double), typeof(Material), default(double));
        public static readonly BindableProperty CircleStrokeColorProperty = BindableProperty.Create(nameof(CircleStrokeColor), typeof(Color), typeof(Material), NoColor);

        #region Properties

        public Color CircleDisabledColor
        {
            get { return (Color)GetValue(CircleDisabledColorProperty); }
            set { SetValue(CircleDisabledColorProperty, value); }
        }

        public Color CircleStrokeColor
        {
            get { return (Color)GetValue(CircleStrokeColorProperty); }
            set { SetValue(CircleStrokeColorProperty, value); }
        }

        public double CircleStrokeWidth
        {
            get { return (double)GetValue(CircleStrokeWidthProperty); }
            set { SetValue(CircleStrokeWidthProperty, value); }
        }

        public Color CircleColor
        {
            get { return (Color)GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value); }
        }
        
        #endregion

        public static void DrawCircle(SKSurface surface, SKRect targetRect, IBindableCircle view)
        {
            if (view.CircleColor == NoColor && view.CircleStrokeColor == NoColor)
                return;
            
            float scaleFactor = (float)Device.Info.ScalingFactor;
            float strokeWidth = (float)view.CircleStrokeWidth * scaleFactor;

            float x = targetRect.Left + targetRect.Width / 2;
            float y = targetRect.Top  + targetRect.Height / 2;
            float radius = Math.Min((targetRect.Right - targetRect.Left) / 2, (targetRect.Bottom - targetRect.Top) / 2) - 1;

            using (SKPaint skPaint = new SKPaint())
            {
                if (view.CircleColor != Color.Transparent)
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = view.Disabled ? view.CircleDisabledColor.ToSKColor() : view.CircleColor.ToSKColor();
                    surface.Canvas.DrawCircle(x, y, radius, skPaint);
                }

                if (strokeWidth > 0)
                {
                    skPaint.Style = SKPaintStyle.Stroke;
                    skPaint.IsAntialias = true;
                    skPaint.Color = view.CircleStrokeColor.ToSKColor();
                    skPaint.StrokeWidth = strokeWidth;
                    surface.Canvas.DrawCircle(x, y, radius - strokeWidth / 2, skPaint);
                }

            }
        }

        #endregion

        #region Gradient region

        public enum GradientOrientationEnum
        {
            None, Vertical, Horizontal,FromLeftTopCornerToRightDownCorner,FromLeftDownCornerToRightTopCorner
        }

        public static readonly BindableProperty GradientStartColorProperty = BindableProperty.Create(nameof(GradientStartColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty GradientEndColorProperty = BindableProperty.Create(nameof(GradientEndColor), typeof(Color), typeof(Material), default(Color));
        public static readonly BindableProperty GradientOrientationProperty = BindableProperty.Create(nameof(GradientOrientation), typeof(GradientOrientationEnum), typeof(Material), GradientOrientationEnum.Vertical);

        #region Properties

        public GradientOrientationEnum GradientOrientation
        {
            get { return (GradientOrientationEnum)GetValue(GradientOrientationProperty); }
            set { SetValue(GradientOrientationProperty, value); }
        }

        public Color GradientEndColor
        {
            get { return (Color)GetValue(GradientEndColorProperty); }
            set { SetValue(GradientEndColorProperty, value); }
        }

        public Color GradientStartColor
        {
            get { return (Color)GetValue(GradientStartColorProperty); }
            set { SetValue(GradientStartColorProperty, value); }
        }

        #endregion

        public static void DrawGradient(SKSurface surface, SKRect targetRect, IBindableGradient view)
        {
            SKPoint startPoint = new SKPoint();
            SKPoint endPoint = new SKPoint();

            switch (view.GradientOrientation)
            {
                case GradientOrientationEnum.Vertical:
                    startPoint.X = targetRect.Left;
                    startPoint.Y = targetRect.Top;
                    endPoint.X = targetRect.Left;
                    endPoint.Y = targetRect.Height + targetRect.Top;
                    break;

                case GradientOrientationEnum.Horizontal:
                    startPoint.X = targetRect.Left;
                    startPoint.Y = targetRect.Top;
                    endPoint.X = targetRect.Width + targetRect.Left;
                    endPoint.Y = targetRect.Top;
                    break;

                case GradientOrientationEnum.FromLeftTopCornerToRightDownCorner:
                    startPoint.X = targetRect.Left;
                    startPoint.Y = targetRect.Top;
                    endPoint.X = targetRect.Width + targetRect.Left;
                    endPoint.Y = targetRect.Height + targetRect.Top;
                    break;

                case GradientOrientationEnum.FromLeftDownCornerToRightTopCorner:
                    startPoint.X = targetRect.Left;
                    startPoint.Y = targetRect.Height + targetRect.Top;
                    endPoint.X = targetRect.Width + targetRect.Left;
                    endPoint.Y = targetRect.Top;
                    break;

            }

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                using (var shader = SKShader.CreateLinearGradient(
                    startPoint,
                    endPoint,
                    new[] { view.GradientStartColor.ToSKColor(), view.GradientEndColor.ToSKColor() },
                    null,
                    SKShaderTileMode.Clamp))
                {

                    paint.Shader = shader;
                    surface.Canvas.Save();
                    surface.Canvas.ClipRect(targetRect);
                    surface.Canvas.DrawPaint(paint);
                    surface.Canvas.Restore();
                }
            }

        }

        #endregion

        #region Components

        public static readonly BindableProperty ComponentsProperty = BindableProperty.Create(
            nameof(Components), 
            typeof(ObservableCollection<AbstractMaterialComponent>), 
            typeof(Material), 
            default(ObservableCollection<AbstractMaterialComponent>),
            defaultValueCreator: bindable =>
            {
                var coll = new ObservableCollection<AbstractMaterialComponent>();
                return coll;
            }
            );

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
        }

        public ObservableCollection<AbstractMaterialComponent> Components
        {
            get { return (ObservableCollection<AbstractMaterialComponent>)GetValue(ComponentsProperty); }
            set { SetValue(ComponentsProperty, value); }
        }

        #endregion

        #region ContentLayout

        public static readonly BindableProperty ContentLayoutProperty = BindableProperty.Create(nameof(ContentLayout), typeof(Layout), typeof(Material), default(Layout));

        public Layout ContentLayout
        {
            get { return (Layout)GetValue(ContentLayoutProperty); }
            set { SetValue(ContentLayoutProperty, value); }
        }

        #endregion

        #region Command and touch

        SKTouchAction? _lastAction;
        SKPoint? _startLocation;
        protected override void OnTouch(SKTouchEventArgs e)
        {


            float deltaX = Math.Abs((_startLocation?.X ?? 0) - e.Location.X);
            float deltaY = Math.Abs((_startLocation?.Y ?? 0) - e.Location.Y);
            float maxDelta = Math.Max(deltaX, deltaY) * (float)Device.Info.ScalingFactor;
            //Debug.WriteLine($" OnTouch>> {e.ActionType} ");

            switch (Gesture)
            {
                case MaterialGestures.PressedAndReleased:

                    if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
                    {
                        e.Handled = true;
                        //Debug.WriteLine($" OnTouch handled ({e.ActionType})");
                        if (e.ActionType == SKTouchAction.Pressed)
                        {
                            _startLocation = e.Location;
                        }
                    }
                    else if (
                       e.ActionType == SKTouchAction.Released && (
                               _lastAction == SKTouchAction.Pressed
                               ||
                               _lastAction == SKTouchAction.Moved && maxDelta < 20 * Device.Info.ScalingFactor
                            )
                        )
                    {
                        e.Handled = true;
                        ExecuteCommand();
                        Debug.WriteLine($" OnTouch PressedAndReleased ExecuteCommand move delta {maxDelta}");
                    }
                    /*
                    else
                    {
                        Debug.WriteLine(" OnTouch handled ()");
                    }
                    */
                    break;

                case MaterialGestures.Pressed:
                    if (e.ActionType == SKTouchAction.Pressed)
                    {
                        e.Handled = true;
                        Debug.WriteLine(" OnTouch Pressed ExecuteCommand");
                        ExecuteCommand();
                    }
                    break;

            }

            _lastAction = e.ActionType;
        }

        void ExecuteCommand()
        {
            //Debug.WriteLine("Material: command executing");

            if (IsCheckable)
                IsChecked = !IsChecked;

            object param = CommandParameter;

            if (param == null && IsCheckable)
                param = IsChecked;


            try
            {
                if(Command != null && Command.CanExecute(param))
                    Command.Execute(param);
            }
            catch (Exception ex)
            {
                ExceptionManager.SendError(ex);
            }

        }

        #endregion

        void CheckRegistrationForDisposing()
        {
            bool needToRegister = false;
            if (_RawImage != null && _RawImage.NeedToRegisterForDispose && !_RawImage.RegisteredForDispose)
            {
                needToRegister = true;
            }

            for(int i = 0;i < Components.Count; i++)
            {
                IDisposableRegistration idr = Components[i] as IDisposableRegistration;
                if(idr != null && !idr.RegisteredForDispose && idr.NeedToRegisterForDispose)
                {
                    needToRegister = true;
                    break;
                }
            }

            if (needToRegister)
            {
                CommonPage page = this.FindByType<CommonPage>();
                if(page != null)
                {
                    page.DisposableItems += this;
                    RawImage.RegisteredForDispose = true;

                    for (int i = 0; i < Components.Count; i++)
                    {
                        IDisposableRegistration idr = Components[i] as IDisposableRegistration;
                        if (!idr.RegisteredForDispose && idr.NeedToRegisterForDispose)
                        {
                            idr.RegisteredForDispose = true;
                        }
                    }

                }
            }

        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            double ScalingFactor = Device.info.ScalingFactor;
            // Check
            CheckRegistrationForDisposing();

            // Draw components before content
            if ((Components?.Count ?? 0) > 0)
            {
                foreach (var component in Components)
                {
                    if (component.SKParentView == null)
                        component.SKParentView = this;

                    component.OnPaintSurfaceBeforeContentInternal(e);
                }
            }
            

            e.Surface.Canvas.Clear(SKColors.Transparent);

            if(CanvasTranslationX != 0 || CanvasTranslationY != 0)
            {
                e.Surface.Canvas.Translate((float)(CanvasTranslationX * ScalingFactor), (float)(CanvasTranslationY * ScalingFactor));
            }

            SKRect fullRect = new SKRect(0, 0, e.Info.Width, e.Info.Height);
            SKRect contentRect = fullRect.Clone();

            contentRect = DrawMaterialFrame(e.Surface, fullRect, this);

            if (CircleColor != NoColor || CircleStrokeWidth > 0)
            {
                DrawCircle(e.Surface, contentRect, this);
            }

            if (!string.IsNullOrEmpty(ImageSrc))
            {
                DrawImage(e.Surface, contentRect, this);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                DrawText(e.Surface, contentRect, this);
            }


            //DrawContent(e.Surface, contentRect);

            if(GradientOrientation != GradientOrientationEnum.None)
            {
                DrawGradient(e.Surface,contentRect,this);
            }

            if (BadgeShow)
            {
                Material.DrawBadge(e.Surface, contentRect, this);
            }

            // Draw components
            if((Components?.Count ?? 0) > 0)
            {
                foreach(var component in Components)
                {
                    component.OnPaintSurfaceAfterContentInternal(e);
                }
            }

            // Draw layout
            if(ContentLayout != null)
            {
                
                Rectangle rect = new Rectangle(
                    contentRect.Left    == 0 ? 0 : contentRect.Left / ScalingFactor, 
                    contentRect.Top     == 0 ? 0 : contentRect.Top / ScalingFactor, 
                    contentRect.Right   == 0 ? 0 : contentRect.Right / ScalingFactor, 
                    contentRect.Height  == 0 ? 0 : contentRect.Height / ScalingFactor);
                ContentLayout.Layout(rect);

                DrawLayout(ContentLayout,e, contentRect);
            }

        }

        void DrawLayout(Layout layout,SKPaintSurfaceEventArgs e,SKRect targetRect)
        {
            foreach (var view in layout.Children)
            {
                switch (view)
                {
                    case MCAbstractView aview:

                        if (aview.SKParentView == null)
                            aview.SKParentView = this;

                        aview.OnPaintSurfaceAfterContentInternal(e, targetRect);
                        break;

                    case Layout anotherlayout:
                        DrawLayout(anotherlayout, e,targetRect);
                        break;

                    default:
                        throw new Exception("Material : Unknown view inside Layout");

                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(ContentLayout != null)
            {
                SetInheritedBindingContext(ContentLayout, BindingContext);

                //ContentLayout.BindingContext = BindingContext;
                //ContentLayout.ForceLayout();

                //double width = Width;

                //this.Width > 0 ? this.Width : this.Parent.Width
                /*
                if(this.Width > 0)
                {
                    var size = ContentLayout.Measure(this.Width, double.PositiveInfinity);

                    Debug.WriteLine($"CCH: Height {size.Request.Height} ");

                    this.HeightRequest = size.Request.Height;
                }
                
                ViewCell cell = this.Parent as ViewCell;
                if(cell != null)
                {
                    //cell.ForceUpdateSize();
                    //cell.Height = size.Request.Height;
                }

                //HeightRequest = size.Request.Height;
                */
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Elevation):
                    ShadowWidth = Elevation;
                    ShadowBlurAmount = Elevation;
                    break;

                case nameof(Command):
                case nameof(IsCheckable):
                    EnableTouchEvents = Command != null || IsCheckable;
                    break;

                case nameof(ImageSrc):
                    CreateImage(ImageSrc);
                    break;

                case nameof(Components):
                    if(Components != null)
                    {
                        foreach(var elm in Components)
                        {
                            elm.SKParentView = this;
                        }
                    }
                    break;
            }

            InvalidateSurface();

        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (ContentLayout != null)
            {
                var size = ContentLayout.Measure(widthConstraint, heightConstraint);
                this.HeightRequest = size.Request.Height;
                return size;
            }
            else
            {
                return base.OnMeasure(widthConstraint, heightConstraint);
            }

        }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ImageSrc))
            {
                return ImageSrc;
            }
            else
            {
                return base.ToString();
            }
        }

        public void Dispose()
        {
            // These objects will be dispose by internal assignment
            _RawImage?.Dispose();
            for(int i = 0; i < Components.Count; i++)
            {
                (Components[i] as IDisposable)?.Dispose();
            }
            _loadRequest = null;
        }

    }


}
