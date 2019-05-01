
using Acr.UserDialogs;
using CoreXF;
using Splat;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SkiaSharp;

namespace CoreXF
{

    public enum UnifieldType
    {
        Undefined,
        Label,
        Entry,
        TextEditor,
        Date,
        Time,
        Picker,
        Selector
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Unifield : Grid, IDisposable
    {
        public delegate string EntryChecker(object value);

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty TitleActiveColorProperty = BindableProperty.Create(nameof(TitleActiveColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty TitleFontFamilyProperty = BindableProperty.Create(nameof(TitleFontFamily), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(Unifield), Device.GetNamedSize(NamedSize.Default, typeof(Entry)));
        public static readonly BindableProperty TitleMarginProperty = BindableProperty.Create(nameof(TitleMargin), typeof(Thickness), typeof(Unifield), default(Thickness));

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Unifield), default(IComparable));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Unifield), default(object));
        public static readonly BindableProperty GestureProperty = BindableProperty.Create(nameof(Gesture), typeof(MaterialGestures), typeof(Material), MaterialGestures.Pressed);

        public static readonly BindableProperty ValueColorProperty = BindableProperty.Create(nameof(ValueColor), typeof(Color), typeof(Unifield), Color.Black);
        public static readonly BindableProperty ValueActiveColorProperty = BindableProperty.Create(nameof(ValueActiveColor), typeof(Color), typeof(Unifield), Color.Black);
        public static readonly BindableProperty ValueFontFamilyProperty = BindableProperty.Create(nameof(ValueFontFamily), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty ValueFontSizeProperty = BindableProperty.Create(nameof(ValueFontSize), typeof(double), typeof(Unifield), Device.GetNamedSize(NamedSize.Default,typeof(Entry)));
        public static readonly BindableProperty ValueMarginProperty = BindableProperty.Create(nameof(ValueMargin), typeof(Thickness), typeof(Unifield), default(Thickness));
        public static readonly BindableProperty ValueTextHorizontalAligmentProperty = BindableProperty.Create(nameof(ValueTextHorizontalAligment), typeof(TextAlignment), typeof(Unifield), default(TextAlignment));
        public static readonly BindableProperty ValueTextVerticalAligmentProperty = BindableProperty.Create(nameof(ValueTextVerticalAligment), typeof(TextAlignment), typeof(Unifield), TextAlignment.Center);

        public static readonly BindableProperty LeftImageSourceProperty = BindableProperty.Create(nameof(LeftImageSource), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty LeftImageTintColorProperty = BindableProperty.Create(nameof(LeftImageTintColor), typeof(Color), typeof(Unifield), Material.NoColor);
        public static readonly BindableProperty LeftImageHeightRequestProperty = BindableProperty.Create(nameof(LeftImageHeightRequest), typeof(double), typeof(Unifield), 24d);
        public static readonly BindableProperty LeftImageWidthRequestProperty = BindableProperty.Create(nameof(LeftImageWidthRequest), typeof(double), typeof(Unifield), 24d);
        public static readonly BindableProperty LeftImageScaleProperty = BindableProperty.Create(nameof(LeftImageScale), typeof(float), typeof(Unifield), 1f);
        public static readonly BindableProperty LeftImageTransXProperty = BindableProperty.Create(nameof(LeftImageTransX), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty LeftImageTransYProperty = BindableProperty.Create(nameof(LeftImageTransY), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty LeftImageMarginProperty = BindableProperty.Create(nameof(LeftImageMargin), typeof(Thickness), typeof(Unifield), default(Thickness));
        public static readonly BindableProperty LeftImageCommandProperty = BindableProperty.Create(nameof(LeftImageCommand), typeof(ICommand), typeof(Unifield), default(ICommand));
        public static readonly BindableProperty LeftImageCommandParameterProperty = BindableProperty.Create(nameof(LeftImageCommandParameter), typeof(object), typeof(Unifield), default(object));
        public static readonly BindableProperty LeftImageCircleColorProperty = BindableProperty.Create(nameof(LeftImageCircleColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty LeftImageCircleStrokeWidthProperty = BindableProperty.Create(nameof(LeftImageCircleStrokeWidth), typeof(double), typeof(Unifield), default(double));
        public static readonly BindableProperty LeftImageCircleStrokeColorProperty = BindableProperty.Create(nameof(LeftImageCircleStrokeColor), typeof(Color), typeof(Unifield), default(Color));

        public static readonly BindableProperty RightImageSourceProperty = BindableProperty.Create(nameof(RightImageSource), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty RightImageTintColorProperty = BindableProperty.Create(nameof(RightImageTintColor), typeof(Color), typeof(Unifield), Material.NoColor);
        public static readonly BindableProperty RightImageHeightRequestProperty = BindableProperty.Create(nameof(RightImageHeightRequest), typeof(double), typeof(Unifield), 24d);
        public static readonly BindableProperty RightImageWidthRequestProperty = BindableProperty.Create(nameof(RightImageWidthRequest), typeof(double), typeof(Unifield), 24d);
        public static readonly BindableProperty RightImageScaleProperty = BindableProperty.Create(nameof(RightImageScale), typeof(float), typeof(Unifield), 1f);
        public static readonly BindableProperty RightImageTransXProperty = BindableProperty.Create(nameof(RightImageTransX), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty RightImageTransYProperty = BindableProperty.Create(nameof(RightImageTransY), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty RightImageMarginProperty = BindableProperty.Create(nameof(RightImageMargin), typeof(Thickness), typeof(Unifield), default(Thickness));
        public static readonly BindableProperty RightImageCommandProperty = BindableProperty.Create(nameof(RightImageCommand), typeof(ICommand), typeof(Unifield), default(ICommand));
        public static readonly BindableProperty RightImageCommandParameterProperty = BindableProperty.Create(nameof(RightImageCommandParameter), typeof(object), typeof(Unifield), default(object));
        public static readonly BindableProperty RightImageCircleColorProperty = BindableProperty.Create(nameof(RightImageCircleColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty RightImageCircleStrokeWidthProperty = BindableProperty.Create(nameof(RightImageCircleStrokeWidth), typeof(double), typeof(Unifield), default(double));
        public static readonly BindableProperty RightImageCircleStrokeColorProperty = BindableProperty.Create(nameof(RightImageCircleStrokeColor), typeof(Color), typeof(Unifield), default(Color));

        public static readonly BindableProperty ShowSeparatorProperty = BindableProperty.Create(nameof(ShowSeparator), typeof(bool), typeof(Unifield), default(bool));
        public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(Unifield), Color.Gray);
        public static readonly BindableProperty SeparatorStrokeCapProperty = BindableProperty.Create(nameof(SeparatorStrokeCap), typeof(SKStrokeCap), typeof(Unifield), default(SKStrokeCap));
        public static readonly BindableProperty SeparatorDashFillLenghtProperty = BindableProperty.Create(nameof(SeparatorDashFillLenght), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty SeparatorDashEmptyLenghtProperty = BindableProperty.Create(nameof(SeparatorDashEmptyLenght), typeof(float), typeof(Unifield), default(float));

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(nameof(PlaceholderColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty PlaceholderFontFamilyProperty = BindableProperty.Create(nameof(PlaceholderFontFamily), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty PlaceHolderFontSizeProperty = BindableProperty.Create(nameof(PlaceHolderFontSize), typeof(double), typeof(Unifield), Device.GetNamedSize(NamedSize.Default, typeof(Entry)));
        public static readonly BindableProperty PlaceholderMarginProperty = BindableProperty.Create(nameof(PlaceholderMargin), typeof(Thickness), typeof(Unifield), default(Thickness));

        public static readonly BindableProperty FrameColorProperty = BindableProperty.Create(nameof(FrameColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty FrameRadiusProperty = BindableProperty.Create(nameof(FrameRadius), typeof(float), typeof(Unifield), default(float));
        public static readonly BindableProperty FrameStrokeColorProperty = BindableProperty.Create(nameof(FrameStrokeColor), typeof(Color), typeof(Unifield), default(Color));
        public static readonly BindableProperty FrameStrokeWidthProperty = BindableProperty.Create(nameof(FrameStrokeWidth), typeof(float), typeof(Unifield), default(float));

        public static readonly BindableProperty ItemDisplayPathProperty = BindableProperty.Create(nameof(ItemDisplayPath), typeof(string), typeof(Unifield), default(string));

        public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(UnifieldType), typeof(Unifield), UnifieldType.Undefined);
        public static readonly BindableProperty BorderlessProperty = BindableProperty.Create(nameof(Borderless), typeof(bool), typeof(Unifield), default(bool));
        public static readonly BindableProperty AndroidLineColorProperty = BindableProperty.Create(nameof(AndroidLineColor), typeof(Color), typeof(Unifield), Material.NoColor);

        // Value
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(object), typeof(Unifield), default(object), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(Unifield), default(bool));
        public static readonly BindableProperty IsTextPredictionEnabledProperty = BindableProperty.Create(nameof(IsTextPredictionEnabled), typeof(bool), typeof(Unifield), default(bool));
        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard), typeof(Keyboard), typeof(Unifield), default(Keyboard));
        
        public static readonly BindableProperty StringFormatProperty = BindableProperty.Create(nameof(StringFormat), typeof(string), typeof(Unifield), default(string));
        public static readonly BindableProperty ErrorConditionProperty = BindableProperty.Create(nameof(ErrorCondition), typeof(EntryChecker), typeof(Unifield), default(EntryChecker));

        // Text
        public static readonly BindableProperty CapitalizeFirstLetterProperty = BindableProperty.Create(nameof(CapitalizeFirstLetter), typeof(bool), typeof(Unifield), default(bool));
        
        // Text editor
        public static readonly BindableProperty TextEditorAutoSizeProperty = BindableProperty.Create(nameof(TextEditorAutoSize), typeof(EditorAutoSizeOption), typeof(Unifield), EditorAutoSizeOption.Disabled);
        public static readonly BindableProperty TextEditorMinimumHeightRequestProperty = BindableProperty.Create(nameof(TextEditorMinimumHeightRequest), typeof(double), typeof(Unifield), default(double));

        // Keyboard
        public static readonly BindableProperty KeyboardReturnTypeProperty = BindableProperty.Create(nameof(KeyboardReturnType), typeof(ReturnType), typeof(Unifield), default(ReturnType));

        // Date 
        public static readonly BindableProperty DateTimeProperty = BindableProperty.Create(nameof(DateTime), typeof(DateTime?), typeof(Unifield), default(DateTime?), defaultBindingMode: BindingMode.TwoWay);

        // Time
        public static readonly BindableProperty TimeSpanProperty = BindableProperty.Create(nameof(TimeSpan), typeof(TimeSpan?), typeof(Unifield), default(TimeSpan?), defaultBindingMode: BindingMode.TwoWay);

        // Picker Selector
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection), typeof(Unifield), default(ICollection), defaultBindingMode: BindingMode.OneWay);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Unifield), default(object), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create(nameof(TextChangedCommand), typeof(ICommand), typeof(Unifield), default(ICommand));
        public static readonly BindableProperty TextChangingCommandProperty = BindableProperty.Create(nameof(TextChangingCommand), typeof(ICommand), typeof(Unifield), default(ICommand));

        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(nameof(ReadOnly), typeof(bool), typeof(Unifield), default(bool));
        
        public Command ClearCommand { get
            {
                if (_clearCommand == null)
                    _clearCommand = new Command(() =>
                    {
                        Value = null;
                    });
                return _clearCommand;
            } }
        Command _clearCommand;

        Lazy<INavigationService> _navigation = new Lazy<INavigationService>(()=> Locator.CurrentMutable.GetService<INavigationService>());
        Lazy<IUserDialogs> _dialogs = new Lazy<IUserDialogs>(() => Locator.CurrentMutable.GetService<IUserDialogs>());

        #region Properties

        public Color LeftImageCircleStrokeColor
        {
            get { return (Color)GetValue(LeftImageCircleStrokeColorProperty); }
            set { SetValue(LeftImageCircleStrokeColorProperty, value); }
        }

        public double LeftImageCircleStrokeWidth
        {
            get { return (double)GetValue(LeftImageCircleStrokeWidthProperty); }
            set { SetValue(LeftImageCircleStrokeWidthProperty, value); }
        }

        public Color LeftImageCircleColor
        {
            get { return (Color)GetValue(LeftImageCircleColorProperty); }
            set { SetValue(LeftImageCircleColorProperty, value); }
        }

        public Color RightImageCircleStrokeColor
        {
            get { return (Color)GetValue(RightImageCircleStrokeColorProperty); }
            set { SetValue(RightImageCircleStrokeColorProperty, value); }
        }

        public double RightImageCircleStrokeWidth
        {
            get { return (double)GetValue(RightImageCircleStrokeWidthProperty); }
            set { SetValue(RightImageCircleStrokeWidthProperty, value); }
        }

        public Color RightImageCircleColor
        {
            get { return (Color)GetValue(RightImageCircleColorProperty); }
            set { SetValue(RightImageCircleColorProperty, value); }
        }

        public TextAlignment ValueTextVerticalAligment
        {
            get { return (TextAlignment)GetValue(ValueTextVerticalAligmentProperty); }
            set { SetValue(ValueTextVerticalAligmentProperty, value); }
        }

        public TextAlignment ValueTextHorizontalAligment
        {
            get { return (TextAlignment)GetValue(ValueTextHorizontalAligmentProperty); }
            set { SetValue(ValueTextHorizontalAligmentProperty, value); }
        }

        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        public ICommand TextChangingCommand
        {
            get { return (ICommand)GetValue(TextChangingCommandProperty); }
            set { SetValue(TextChangingCommandProperty, value); }
        }

        public ReturnType KeyboardReturnType
        {
            get { return (ReturnType)GetValue(KeyboardReturnTypeProperty); }
            set { SetValue(KeyboardReturnTypeProperty, value); }
        }

        public Thickness ValueMargin
        {
            get { return (Thickness)GetValue(ValueMarginProperty); }
            set { SetValue(ValueMarginProperty, value); }
        }

        public Color AndroidLineColor
        {
            get { return (Color)GetValue(AndroidLineColorProperty); }
            set { SetValue(AndroidLineColorProperty, value); }
        }

        public bool Borderless
        {
            get { return (bool)GetValue(BorderlessProperty); }
            set { SetValue(BorderlessProperty, value); }
        }

        public double TextEditorMinimumHeightRequest
        {
            get { return (double)GetValue(TextEditorMinimumHeightRequestProperty); }
            set { SetValue(TextEditorMinimumHeightRequestProperty, value); }
        }

        public EditorAutoSizeOption TextEditorAutoSize
        {
            get { return (EditorAutoSizeOption)GetValue(TextEditorAutoSizeProperty); }
            set { SetValue(TextEditorAutoSizeProperty, value); }
        }

        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }

        public string ItemDisplayPath
        {
            get { return (string)GetValue(ItemDisplayPathProperty); }
            set { SetValue(ItemDisplayPathProperty, value); }
        }

        public Thickness PlaceholderMargin
        {
            get { return (Thickness)GetValue(PlaceholderMarginProperty); }
            set { SetValue(PlaceholderMarginProperty, value); }
        }

        public double PlaceHolderFontSize
        {
            get { return (double)GetValue(PlaceHolderFontSizeProperty); }
            set { SetValue(PlaceHolderFontSizeProperty, value); }
        }

        public string PlaceholderFontFamily
        {
            get { return (string)GetValue(PlaceholderFontFamilyProperty); }
            set { SetValue(PlaceholderFontFamilyProperty, value); }
        }

        public float SeparatorDashEmptyLenght
        {
            get { return (float)GetValue(SeparatorDashEmptyLenghtProperty); }
            set { SetValue(SeparatorDashEmptyLenghtProperty, value); }
        }

        public float SeparatorDashFillLenght
        {
            get { return (float)GetValue(SeparatorDashFillLenghtProperty); }
            set { SetValue(SeparatorDashFillLenghtProperty, value); }
        }


        public SKStrokeCap SeparatorStrokeCap
        {
            get { return (SKStrokeCap)GetValue(SeparatorStrokeCapProperty); }
            set { SetValue(SeparatorStrokeCapProperty, value); }
        }


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

        public object LeftImageCommandParameter
        {
            get { return (object)GetValue(LeftImageCommandParameterProperty); }
            set { SetValue(LeftImageCommandParameterProperty, value); }
        }
        public ICommand LeftImageCommand
        {
            get { return (ICommand)GetValue(LeftImageCommandProperty); }
            set { SetValue(LeftImageCommandProperty, value); }
        }

        public object RightImageCommandParameter
        {
            get { return (object)GetValue(RightImageCommandParameterProperty); }
            set { SetValue(RightImageCommandParameterProperty, value); }
        }

        public ICommand RightImageCommand
        {
            get { return (ICommand)GetValue(RightImageCommandProperty); }
            set { SetValue(RightImageCommandProperty, value); }
        }


        public Thickness RightImageMargin
        {
            get { return (Thickness)GetValue(RightImageMarginProperty); }
            set { SetValue(RightImageMarginProperty, value); }
        }

        public Thickness LeftImageMargin
        {
            get { return (Thickness)GetValue(LeftImageMarginProperty); }
            set { SetValue(LeftImageMarginProperty, value); }
        }


        public float FrameStrokeWidth
        {
            get { return (float)GetValue(FrameStrokeWidthProperty); }
            set { SetValue(FrameStrokeWidthProperty, value); }
        }
        public Color FrameStrokeColor
        {
            get { return (Color)GetValue(FrameStrokeColorProperty); }
            set { SetValue(FrameStrokeColorProperty, value); }
        }

        public float FrameRadius
        {
            get { return (float)GetValue(FrameRadiusProperty); }
            set { SetValue(FrameRadiusProperty, value); }
        }

        public Color FrameColor
        {
            get { return (Color)GetValue(FrameColorProperty); }
            set { SetValue(FrameColorProperty, value); }
        }


        public float RightImageTransY
        {
            get { return (float)GetValue(RightImageTransYProperty); }
            set { SetValue(RightImageTransYProperty, value); }
        }

        public float RightImageTransX
        {
            get { return (float)GetValue(RightImageTransXProperty); }
            set { SetValue(RightImageTransXProperty, value); }
        }


        public float LeftImageTransY
        {
            get { return (float)GetValue(LeftImageTransYProperty); }
            set { SetValue(LeftImageTransYProperty, value); }
        }
        public float LeftImageTransX
        {
            get { return (float)GetValue(LeftImageTransXProperty); }
            set { SetValue(LeftImageTransXProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public Color SeparatorColor
        {
            get { return (Color)GetValue(SeparatorColorProperty); }
            set { SetValue(SeparatorColorProperty, value); }
        }

        public bool ShowSeparator
        {
            get { return (bool)GetValue(ShowSeparatorProperty); }
            set { SetValue(ShowSeparatorProperty, value); }
        }

        public double ValueFontSize
        {
            get { return (double)GetValue(ValueFontSizeProperty); }
            set { SetValue(ValueFontSizeProperty, value); }
        }

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        public float LeftImageScale
        {
            get { return (float)GetValue(LeftImageScaleProperty); }
            set { SetValue(LeftImageScaleProperty, value); }
        }

        public double LeftImageWidthRequest
        {
            get { return (double)GetValue(LeftImageWidthRequestProperty); }
            set { SetValue(LeftImageWidthRequestProperty, value); }
        }

        public double LeftImageHeightRequest
        {
            get { return (double)GetValue(LeftImageHeightRequestProperty); }
            set { SetValue(LeftImageHeightRequestProperty, value); }
        }

        public Color LeftImageTintColor
        {
            get { return (Color)GetValue(LeftImageTintColorProperty); }
            set { SetValue(LeftImageTintColorProperty, value); }
        }

        public string LeftImageSource
        {
            get { return (string)GetValue(LeftImageSourceProperty); }
            set { SetValue(LeftImageSourceProperty, value); }
        }



        public float RightImageScale
        {
            get { return (float)GetValue(RightImageScaleProperty); }
            set { SetValue(RightImageScaleProperty, value); }
        }

        public double RightImageWidthRequest
        {
            get { return (double)GetValue(RightImageWidthRequestProperty); }
            set { SetValue(RightImageWidthRequestProperty, value); }
        }

        public double RightImageHeightRequest
        {
            get { return (double)GetValue(RightImageHeightRequestProperty); }
            set { SetValue(RightImageHeightRequestProperty, value); }
        }

        public Color RightImageTintColor
        {
            get { return (Color)GetValue(RightImageTintColorProperty); }
            set { SetValue(RightImageTintColorProperty, value); }
        }

        public string RightImageSource
        {
            get { return (string)GetValue(RightImageSourceProperty); }
            set { SetValue(RightImageSourceProperty, value); }
        }


        public Color ValueActiveColor { get => (Color)GetValue(ValueActiveColorProperty); set => SetValue(ValueActiveColorProperty, value); }

        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }


        public string TitleFontFamily
        {
            get { return (string)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        public string ValueFontFamily
        {
            get { return (string)GetValue(ValueFontFamilyProperty); }
            set { SetValue(ValueFontFamilyProperty, value); }
        }

        public Color ValueColor
        {
            get { return (Color)GetValue(ValueColorProperty); }
            set { SetValue(ValueColorProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ICommand TextChangedCommand
        {
            get { return (ICommand)GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        public UnifieldType Type
        {
            get { return (UnifieldType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public ICollection ItemsSource
        {
            get { return (ICollection)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public TimeSpan? TimeSpan
        {
            get { return (TimeSpan?)GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }

        public bool CapitalizeFirstLetter
        {
            get { return (bool)GetValue(CapitalizeFirstLetterProperty); }
            set { SetValue(CapitalizeFirstLetterProperty, value); }
        }

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public EntryChecker ErrorCondition
        {
            get { return (EntryChecker)GetValue(ErrorConditionProperty); }
            set { SetValue(ErrorConditionProperty, value); }
        }

        public DateTime? DateTime
        {
            get { return (DateTime?)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public Color TitleActiveColor
        {
            get { return (Color)GetValue(TitleActiveColorProperty); }
            set { SetValue(TitleActiveColorProperty, value); }
        }


        public Keyboard Keyboard
        {
            get { return (Keyboard)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }

        public bool IsTextPredictionEnabled
        {
            get { return (bool)GetValue(IsTextPredictionEnabledProperty); }
            set { SetValue(IsTextPredictionEnabledProperty, value); }
        }

        public bool IsPassword
        {
            get { return (bool)GetValue(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }


        #endregion

        public Unifield()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;

            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        }

        /*
        protected override void RaiseTraps()
        {
            if(Type == UniFieldType.Text)
            {
                _grid.RaiseChild(_entry);
            }

            base.RaiseTraps();
        }
        */

        #region TextEntry events

        private void _entry_Unfocused(object sender, FocusEventArgs e)
        {
            SetPlaceholder();
        }

        private void _entry_Focused(object sender, FocusEventArgs e)
        {
            SetPlaceholder();
        }

        bool _innerChangind;
        private void _entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _innerChangind = true;
            if(string.IsNullOrEmpty(ValueAsString) && CapitalizeFirstLetter && !string.IsNullOrEmpty(e.NewTextValue))
            {
                __MainEntry.Text = e.NewTextValue.ToUpper();
                Value = __MainEntry.Text;
            }
            else
            {
                Value = e.NewTextValue;
            }
            _innerChangind = false;

            SetPlaceholder();

            switch (Type)
            {
                case UnifieldType.Entry:
                    if(ErrorCondition != null)
                    {
                        ErrorLabel.Text = ErrorCondition?.Invoke(__MainEntry.Text);
                        ErrorLabel.IsVisible = !string.IsNullOrEmpty(ErrorLabel.Text);
                    }
                    break;

                case UnifieldType.TextEditor:
                    if(ErrorCondition != null)
                    {
                        ErrorLabel.Text = ErrorCondition?.Invoke(__TextEditor.Text);
                        ErrorLabel.IsVisible = !string.IsNullOrEmpty(ErrorLabel.Text);
                    }
                    break;
            }

            TextChangingCommand?.Execute(Value);
        }

        #endregion

        #region Controls 

        // Frame
        public Material Frame
        {
            get
            {
                if (__Frame == null)
                {
                    __Frame = new Material()
                    {
                    };
                    __Frame.SetBinding(Material.MainColorProperty, new Binding(nameof(FrameColor), source: this));
                    __Frame.SetBinding(Material.RadiusProperty, new Binding(nameof(FrameRadius), source: this));
                    __Frame.SetBinding(Material.StrokeColorProperty, new Binding(nameof(FrameStrokeColor), source: this));
                    __Frame.SetBinding(Material.StrokeWidthProperty, new Binding(nameof(FrameStrokeWidth), source: this));
                    //__Frame.SetBinding(Material.Property, new Binding(nameof(), source: this));

                    this.AddView(__Frame,Row:1, FillColumns:true);

                    LowerChild(__Frame);
                }
                return __Frame;
            }
        }
        Material __Frame;

        // Left image
        public Material LeftImage
        {
            get
            {
                if (__LeftImage == null)
                {
                    __LeftImage = new Material()
                    {
                        MainColor = Color.Transparent,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                        
                };
                    __LeftImage.SetBinding(Material.CommandProperty, new Binding(nameof(LeftImageCommand), source: this));
                    __LeftImage.SetBinding(Material.CommandParameterProperty, new Binding(nameof(LeftImageCommandParameter), source: this));
                    __LeftImage.SetBinding(Material.MarginProperty, new Binding(nameof(LeftImageMargin), source: this));
                    __LeftImage.SetBinding(Material.ImageSrcProperty, new Binding(nameof(LeftImageSource), source: this));
                    __LeftImage.SetBinding(Material.ImageTintColorProperty, new Binding(nameof(LeftImageTintColor), source: this));
                    __LeftImage.SetBinding(Material.ImageScaleProperty, new Binding(nameof(LeftImageScale), source: this));
                    __LeftImage.SetBinding(Material.HeightRequestProperty, new Binding(nameof(LeftImageHeightRequest), source: this));
                    __LeftImage.SetBinding(Material.WidthRequestProperty, new Binding(nameof(LeftImageWidthRequest), source: this));
                    __LeftImage.SetBinding(Material.ImageTransXProperty, new Binding(nameof(LeftImageTransX), source: this));
                    __LeftImage.SetBinding(Material.ImageTransYProperty, new Binding(nameof(LeftImageTransY), source: this));
                    __LeftImage.SetBinding(Material.CircleColorProperty, new Binding(nameof(LeftImageCircleColor), source: this));
                    __LeftImage.SetBinding(Material.CircleStrokeColorProperty, new Binding(nameof(LeftImageCircleStrokeColor), source: this));
                    __LeftImage.SetBinding(Material.CircleStrokeWidthProperty, new Binding(nameof(LeftImageCircleStrokeWidth), source: this));
                    //__LeftImage.SetBinding(Material.Property, new Binding(nameof(), source: this));
                    this.AddView(__LeftImage, Row:1);
                    
                }
                return __LeftImage;
            }
        }
        Material __LeftImage;

        // Right image
        public Material RightImage
        {
            get
            {
                if (__RightImage == null)
                {
                    __RightImage = new Material()
                    {
                        MainColor = Color.Transparent,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    __RightImage.SetBinding(Material.CommandProperty, new Binding(nameof(RightImageCommand), source: this));
                    __RightImage.SetBinding(Material.CommandParameterProperty, new Binding(nameof(RightImageCommandParameter), source: this));
                    __RightImage.SetBinding(Material.MarginProperty, new Binding(nameof(RightImageMargin), source: this));
                    __RightImage.SetBinding(Material.ImageSrcProperty, new Binding(nameof(RightImageSource), source: this));
                    __RightImage.SetBinding(Material.ImageTintColorProperty, new Binding(nameof(RightImageTintColor), source: this));
                    __RightImage.SetBinding(Material.ImageScaleProperty, new Binding(nameof(RightImageScale), source: this));
                    __RightImage.SetBinding(Material.HeightRequestProperty, new Binding(nameof(RightImageHeightRequest), source: this));
                    __RightImage.SetBinding(Material.WidthRequestProperty, new Binding(nameof(RightImageWidthRequest), source: this));
                    __RightImage.SetBinding(Material.ImageTransXProperty, new Binding(nameof(RightImageTransX), source: this));
                    __RightImage.SetBinding(Material.ImageTransYProperty, new Binding(nameof(RightImageTransY), source: this));
                    __RightImage.SetBinding(Material.CircleColorProperty, new Binding(nameof(RightImageCircleColor), source: this));
                    __RightImage.SetBinding(Material.CircleStrokeColorProperty, new Binding(nameof(RightImageCircleStrokeColor), source: this));
                    __RightImage.SetBinding(Material.CircleStrokeWidthProperty, new Binding(nameof(RightImageCircleStrokeWidth), source: this));
                    //__RightImage.SetBinding(Material.Property, new Binding(nameof(), source: this));


                    this.AddView(__RightImage, Column:2, Row:1);
                }
                return __RightImage;
            }
        }
        Material __RightImage;

        // Separator
        public Line Separator
        {
            get
            {
                if (__Separator == null)
                {
                    __Separator = new Line()
                    {
                        HeightRequest = 1,
                        VerticalOptions = LayoutOptions.End,
                        Color = Color.Gray
                    };
                    __Separator.SetBinding(Line.DashEmptyLenghtProperty, new Binding(nameof(SeparatorDashEmptyLenght), source: this));
                    __Separator.SetBinding(Line.StrokeCapProperty, new Binding(nameof(SeparatorStrokeCap), source: this));
                    __Separator.SetBinding(Line.DashFillLenghtProperty, new Binding(nameof(SeparatorDashFillLenght), source: this));
                    __Separator.SetBinding(Line.ColorProperty, new Binding(nameof(SeparatorColor), source: this));
                    //__Separator.SetBinding(Line.Property, new Binding(nameof(), source: this));
                    this.AddView(__Separator, Row:1, ColumnSpan:3);
                }
                return __Separator;
            }
        }
        Line __Separator;

        // Placeholder
        public Label PlaceholderLabel
        {
            get
            {
                if (__PlaceholderLabel == null)
                {
                    __PlaceholderLabel = new Label()
                    {
                        IsVisible = false,
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0),
                        InputTransparent = true
                    };
                    __PlaceholderLabel.SetBinding(Label.MarginProperty, new Binding(nameof(PlaceholderMargin), source: this));
                    __PlaceholderLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(PlaceholderColor), source: this));
                    __PlaceholderLabel.SetBinding(Label.FontFamilyProperty, new Binding(nameof(PlaceholderFontFamily), source: this));
                    __PlaceholderLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(PlaceHolderFontSize), source: this));
                    __PlaceholderLabel.SetBinding(Label.TextProperty, new Binding(nameof(Placeholder), source: this));

                    this.AddView(__PlaceholderLabel,Row: 1,Column: 1);
                }
                return __PlaceholderLabel;
            }
        }
        protected Label __PlaceholderLabel;

        // Title
        public Label TitleLabel
        {
            get
            {
                if(__TitleLabel == null)
                {
                    __TitleLabel = new Label();

                    __TitleLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(TitleColor), source: this));
                    __TitleLabel.SetBinding(Label.FontFamilyProperty, new Binding(nameof(TitleFontFamily), source: this));
                    __TitleLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(TitleFontSize), source: this));
                    __TitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));
                    __TitleLabel.SetBinding(Label.MarginProperty, new Binding(nameof(TitleMargin), source: this));

                    this.AddView(__TitleLabel, FillColumns:true);
                }
                return __TitleLabel;
            }
        }
        Label __TitleLabel;

        // Entry
        public Entry MainEntry
        {
            get
            {
                if(__MainEntry == null)
                {
                    __MainEntry = new Entry
                    {
                        BackgroundColor = Color.Transparent,
                        IsTextPredictionEnabled = false
                    };
                    __MainEntry.SetBinding(Entry.TextColorProperty, new Binding(nameof(ValueColor), source: this));
                    __MainEntry.SetBinding(Entry.FontSizeProperty, new Binding(nameof(ValueFontSize), source: this));
                    __MainEntry.SetBinding(Entry.FontFamilyProperty, new Binding(nameof(ValueFontFamily), source: this));
                    __MainEntry.SetBinding(Entry.MarginProperty, new Binding(nameof(ValueMargin), source: this));
                    __MainEntry.SetBinding(Entry.IsPasswordProperty, new Binding(nameof(IsPassword), source: this));
                    //__MainEntry.SetBinding(Entry.ReturnCommandProperty, new Binding(nameof(TextChangedCommand   ), source: this));
                    //__MainEntry.SetBinding(Entry.ReturnCommandParameterProperty, new Binding(nameof(TextChangedCommandParameter), source: this));
                    __MainEntry.SetBinding(Entry.IsTextPredictionEnabledProperty, new Binding(nameof(IsTextPredictionEnabled), source: this));
                    __MainEntry.SetBinding(Entry.ReturnTypeProperty, new Binding(nameof(KeyboardReturnType), source: this));

                    __MainEntry.SetBinding(Entry.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
                    __MainEntry.SetBinding(Entry.IsReadOnlyProperty, new Binding(nameof(ReadOnly), source: this));
                    __MainEntry.SetBinding(Entry.HorizontalTextAlignmentProperty, new Binding(nameof(ValueTextHorizontalAligment), source: this));


                    __MainEntry.TextChanged += _entry_TextChanged;
                    __MainEntry.Focused += _entry_Focused;
                    __MainEntry.Unfocused += _entry_Unfocused;

                    this.AddView(__MainEntry, Row: 1, Column: 1);
                }
                return __MainEntry;
            }
        }
        Entry __MainEntry;

        // Text editor
        public EditorExt TextEditor
        {
            get
            {
                if (__TextEditor == null)
                {
                    __TextEditor = new EditorExt
                    {
                        BackgroundColor = Color.Transparent
                    };
                    __TextEditor.SetBinding(Editor.TextColorProperty, new Binding(nameof(ValueColor), source: this));
                    __TextEditor.SetBinding(Editor.FontSizeProperty, new Binding(nameof(ValueFontSize), source: this));
                    __TextEditor.SetBinding(Editor.FontFamilyProperty, new Binding(nameof(ValueFontFamily), source: this));
                    __TextEditor.SetBinding(InputView.KeyboardProperty, new Binding(nameof(Keyboard), source: this));
                    __TextEditor.SetBinding(Editor.AutoSizeProperty, new Binding(nameof(TextEditorAutoSize), source: this));
                    __TextEditor.SetBinding(VisualElement.MinimumHeightRequestProperty, new Binding(nameof(TextEditorMinimumHeightRequest), source: this));
                    __TextEditor.SetBinding(EditorExt.IsBorderlessProperty, new Binding(nameof(Borderless),source: this));
                    __TextEditor.SetBinding(EditorExt.ReturnTypeProperty, new Binding(nameof(KeyboardReturnType), source: this));
                    __TextEditor.SetBinding(EditorExt.CompletedCommandProperty, new Binding(nameof(TextChangedCommand), source: this));
                    __TextEditor.SetBinding(Editor.IsReadOnlyProperty, new Binding(nameof(ReadOnly), source: this));

                    __TextEditor.TextChanged += _entry_TextChanged;
                    __TextEditor.Focused += _entry_Focused;
                    __TextEditor.Unfocused += _entry_Unfocused;

                    this.AddView(__TextEditor, Row: 1, Column: 1);
                }
                return __TextEditor;
            }
        }
        EditorExt __TextEditor;

        // Main label
        public Label MainLabel
        {
            get
            {
                if (__MainLabel == null)
                {
                    __MainLabel = new Label
                    {
                        BackgroundColor = Color.Transparent,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    __MainLabel.SetBinding(Label.TextColorProperty, new Binding(nameof(ValueColor), source: this));
                    __MainLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(ValueFontSize), source: this));
                    __MainLabel.SetBinding(Label.FontFamilyProperty, new Binding(nameof(ValueFontFamily), source: this));
                    __MainLabel.SetBinding(Label.TextProperty, new Binding(nameof(Value), source: this));
                    __MainLabel.SetBinding(Label.MarginProperty, new Binding(nameof(ValueMargin), source: this));
                    __MainLabel.SetBinding(Label.VerticalTextAlignmentProperty, new Binding(nameof(ValueTextVerticalAligment), source: this));
                    __MainLabel.SetBinding(Label.HorizontalTextAlignmentProperty, new Binding(nameof(ValueTextHorizontalAligment), source: this));

                    this.AddView(__MainLabel, Row: 1, Column: 1);
                }
                return __MainLabel;
            }
        }
        Label __MainLabel;


        // Error label
        public Label ErrorLabel
        {
            get
            {
                if (__ErrorLabel == null)
                {
                    __ErrorLabel = new Label
                    {
                        IsVisible = false,
                        TextColor = Color.Red,
                        FontSize = 12,
                    };
                    this.AddView(__ErrorLabel, Row: 2, Column: 1);
                }
                return __ErrorLabel;
            }
        }
        Label __ErrorLabel;

        #endregion

        //protected void SetPlaceholder(string value, View element)
        protected void SetPlaceholder()
        {
            if (__PlaceholderLabel == null)
                return;

            View element;
            string text;
            switch (Type)
            {
                case UnifieldType.Entry:
                    element = MainEntry;
                    text = MainEntry.Text;
                    break;

                case UnifieldType.TextEditor:
                    element = TextEditor;
                    text = TextEditor.Text;
                    break;

                default:
                    throw new Exception($"SetPlaceholder Unknonw type {Type}");
            }

            if (element?.IsFocused ?? false)
            {
                __PlaceholderLabel.IsVisible = false;
                return;
            }

            __PlaceholderLabel.IsVisible = string.IsNullOrEmpty(text);

        }

        async Task EnterDateTime()
        {
            
            DateTime datetime = default(DateTime);
            TimeSpan timeSpan = default(TimeSpan);

            // Date
            if (Type == UnifieldType.Date)
            {
                var result = await _dialogs.Value.DatePromptAsync(new DatePromptConfig
                {
                    SelectedDate = DateTime,
                    OkText = Tx.T("Dialogs_Ok"),
                    CancelText = Tx.T("Dialogs_Cancel")
                });
                if (!result.Ok)
                    return;

                datetime = result.SelectedDate;
            }

            // Time
            if (Type == UnifieldType.Time)
            {
                var result = await _dialogs.Value.TimePromptAsync(new TimePromptConfig
                {
                    OkText = Tx.T("Dialogs_Ok"),
                    CancelText = Tx.T("Dialogs_Cancel")
                });
                if (!result.Ok)
                    return;

                timeSpan = result.SelectedTime;
            }

            switch (Type)
            {
                case UnifieldType.Date:
                    DateTime = datetime;
                    if(ErrorCondition != null)
                    {
                        ErrorLabel.Text = ErrorCondition?.Invoke(DateTime);
                        ErrorLabel.IsVisible = !string.IsNullOrEmpty(ErrorLabel.Text);
                    }
                    break;

                case UnifieldType.Time:
                    TimeSpan = timeSpan;
                    if(ErrorCondition != null)
                    {
                        ErrorLabel.Text = ErrorCondition?.Invoke(TimeSpan);
                        ErrorLabel.IsVisible = !string.IsNullOrEmpty(ErrorLabel.Text);
                    }
                    break;
            }
        }

        string GetPresentation(object elm)
        {
            if (elm == null)
                return "";

            if (string.IsNullOrEmpty(ItemDisplayPath))
                return elm.ToString();

            var propertyMD = elm.GetType().GetProperty(ItemDisplayPath);
            object val = propertyMD.GetValue(elm);
            if (val == null)
                return "";

            return val as string;

        }

        async Task SelectValueFromList()
        {
            if (ItemsSource == null || ItemsSource.Count == 0)
                return;

            if(Type == UnifieldType.Selector)
            {
                List<SelectorPage.SelectorItem> list = new List<SelectorPage.SelectorItem>();
                foreach (var elm in ItemsSource)
                {
                    if (elm == null)
                        continue;

                    string presentation = GetPresentation(elm);

                    list.Add(new SelectorPage.SelectorItem
                    {
                        Name = presentation,
                        SearchSubsr = presentation,
                        Value = elm
                    });
                }

                await _navigation.Value.NavigateToAsync<SelectorPage>(
                    new SelectorPage.Parameters(list.OrderBy(x => x.Name))
                    {
                        Title = Title,
                        ShowSearchBar = true,
                        OnSuccessfulCloseAsync = async x =>
                        {
                        SelectedItem = (x as SelectorPage.SelectorItem).Value;
                    }
                    });
            }
            else if (Type == UnifieldType.Picker)
            {
                List<ActionSheetOption> list = new List<ActionSheetOption>();
                foreach (var elm in ItemsSource)
                {
                    if (elm == null)
                        continue;

                    list.Add(new ActionSheetOption(GetPresentation(elm), ()=> {
                        SelectedItem = elm;
                    })
                   );
                }

                _dialogs.Value.ActionSheet(new ActionSheetConfig
                {
                    Title = Title,
                    Options = list.OrderBy(x => x.Text).ToList(),
                    Cancel = new ActionSheetOption(Tx.T("Dialogs_Cancel"))
                });
            }

        }

        protected string ValueAsString
        {
            get
            {
                if (Value == null)
                    return null;

                if (Value is string)
                    return Value as string;

                return Value.ToString();
            }
        }

        void AdujstType()
        {
            double d;
            switch (Type)
            {
                case UnifieldType.Entry:
                    d = MainEntry.AnchorX;
                    //RemoveTapGestureTrap();
                    break;

                case UnifieldType.TextEditor:
                    d = TextEditor.AnchorX;
                    break;

                case UnifieldType.Date:
                case UnifieldType.Time:
                    //AddTapGestureTrap(new Command(async () => await EnterDateTime()));
                    break;

                case UnifieldType.Picker:
                case UnifieldType.Selector:
                    //AddTapGestureTrap(new Command(async () => await SelectValueFromList()));
                    break;

                case UnifieldType.Label:
                    d = MainLabel.AnchorX;
                    break;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            double d;
            base.OnPropertyChanged(propertyName);
            
            switch (propertyName)
            {
                case nameof(Title):
                    d = TitleLabel.AnchorX;
                    break;

                case nameof(DateTime):
                    Value = DateTime?.ToString(StringFormat);
                    break;

                case nameof(SelectedItem):
                    Value = GetPresentation(SelectedItem);
                    break;

                case nameof(TimeSpan):
                    Value = TimeSpan?.ToString(StringFormat);
                    break;

                case nameof(Borderless):
                case nameof(AndroidLineColor):
                    if(__MainEntry != null)
                    {
                        if (Borderless)
                        {
                            EntryEffectsEffect eff = __MainEntry.FindEffect<EntryEffectsEffect>(AddIfThereIsNoEffect: true);
                            eff.Borderless = true;
                        }
                        if(AndroidLineColor != Material.NoColor && Device.RuntimePlatform == Device.Android)
                        {
                            EntryEffectsEffect eff = __MainEntry.FindEffect<EntryEffectsEffect>(AddIfThereIsNoEffect: true);
                            eff.AndroidLineColor = AndroidLineColor;
                        }
                    }
                    break;

                case nameof(Type):
                    AdujstType();
                    if(Type == UnifieldType.Picker || Type == UnifieldType.Selector && string.IsNullOrEmpty(RightImageSource))
                    {
                        RightImageSource = "Icons.arrow_drop_down.svg";
                    }
                    break;

                case nameof(Value):
                    if (_innerChangind)
                        break;
                    if(Type == UnifieldType.Entry)
                    {
                        MainEntry.Text = ValueAsString;
                    }
                    SetPlaceholder();
                    break;

                case nameof(Placeholder):
                    if (!string.IsNullOrEmpty(Placeholder))
                    {
                        d = PlaceholderLabel.AnchorX;
                        SetPlaceholder();
                    }
                    break;

                case nameof(ShowSeparator):
                    if (ShowSeparator)
                    {
                        d = Separator.AnchorX;
                        Separator.IsVisible = ShowSeparator;
                    }
                    break;


                case nameof(Command):
                case nameof(CommandParameter):
                    //RemoveTapGestureTrap();
                    //if (Command != null)
                        //AddTapGestureTrap(Command);
                    break;

                case nameof(LeftImageCommand):
                    RaiseChild(LeftImage);
                    break;

                case nameof(LeftImageSource):
                    d = LeftImage.AnchorX;
                    break;

                case nameof(RightImageCommand):
                    RaiseChild(RightImage);
                    break;

                case nameof(RightImageSource):
                    d = RightImage.AnchorX;
                    break;

                case nameof(FrameColor):
                case nameof(FrameRadius):
                case nameof(FrameStrokeColor):
                case nameof(FrameStrokeWidth):
                    d = Frame.AnchorX;
                    break;



            }
        }

        public void Dispose()
        {
            if(__MainEntry != null)
            {
                __MainEntry.TextChanged -= _entry_TextChanged;
                __MainEntry.Focused -= _entry_Focused;
                __MainEntry.Unfocused -= _entry_Unfocused;
                __MainEntry = null;
            }
            if(__TextEditor != null)
            {
                __TextEditor.TextChanged -= _entry_TextChanged;
                __TextEditor.Focused -= _entry_Focused;
                __TextEditor.Unfocused -= _entry_Unfocused;
                __TextEditor = null;

            }
        }
    }
}