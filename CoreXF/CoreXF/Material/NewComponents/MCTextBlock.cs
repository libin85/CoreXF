
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Input;

namespace CoreXF
{

    public class MCTextBlock : MCAbstractView, IMaterialLabel
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MCTextBlock), default(string));
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MCTextBlock), default(string));
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MCTextBlock), 14d);
        public static readonly BindableProperty LineHeightProperty = BindableProperty.Create(nameof(LineHeight), typeof(double), typeof(MCTextBlock), default(double));
        public static readonly BindableProperty MaxLinesProperty = BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(MCTextBlock), default(int));
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MCTextBlock), Color.Black);
        public static readonly BindableProperty GestureProperty = BindableProperty.Create(nameof(Gesture), typeof(MaterialGestures), typeof(MaterialTextBlock), MaterialGestures.Pressed);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MaterialTextBlock), default(ICommand));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(IMaterialLabel), default(object));

        public static readonly BindableProperty WordsContainerProperty = BindableProperty.Create(nameof(WordsContainer), typeof(MaterialTextBlock.TextContainer), typeof(MaterialTextBlock), default(MaterialTextBlock.TextContainer));

        public bool EnableTouchEvents { get; set; }

        #region Properties

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public MaterialGestures Gesture
        {
            get { return (MaterialGestures)GetValue(GestureProperty); }
            set { SetValue(GestureProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public MaterialTextBlock.TextContainer WordsContainer
        {
            get { return (MaterialTextBlock.TextContainer)GetValue(WordsContainerProperty); }
            set { SetValue(WordsContainerProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            //var size = MaterialTextBlock.MeasureTextBlock(this, widthConstraint, heightConstraint);
            //return size;
            return new SizeRequest();
        }

        public override void OnPaintSurfaceAfterContent(SKPaintSurfaceEventArgs e, SKRect targetRect)
        {
            //MaterialTextBlock.DrawTextBlock(this, targetRect, e.Surface.Canvas);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Text):
                case nameof(WordsContainer):
                    //WordsContainer = null;
                    SKParentView?.InvalidateSurface();
                    break;

                    
            }
        }
    }


}
