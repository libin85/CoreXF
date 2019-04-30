
using PropertyChanged;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CoreXF
{
    [AddINotifyPropertyChangedInterface]
    public class WeekItem
    {
        public int WeekOfYear { get; set; }
        public bool IsVisible { get; set; }
        public DayItem[] Days { get; set; } = new DayItem[7];

        public WeekItem()
        {
            for (int i = 0; i < 7; i++)
            {
                Days[i] = new DayItem();
            }
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class DayItem : ObservableObject
    {
        public bool DayOfSelectedMonth { get; set; }
        public bool IsVisible { get; set; }
        [AlsoNotifyFor(nameof(Day))]
        public DateTime DateTime { get; set; }

        public Color CircleColor { get; set; }
        public Color TextColor { get; set; }
        public Color StrokeColor { get; set; }
        public double StrokeWidth { get; set; }

        public int Day => DateTime.Day;
        public WeakReference<View> View { get; set; }

        public View GetView()
        {
            View.TryGetTarget(out View view);
            return view;
        }

        public bool InactiveDay { get; set; }
    }

    public class WeekTitleItem
    {
        public string Name { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Calendar : Grid
    {
        // Bindable properties
        public DateTime PrevMonth { get; set; }
        public DateTime NextMonth { get; set; }


        public static readonly BindableProperty IsInactiveDayFuncProperty = BindableProperty.Create(nameof(IsInactiveDayFunc), typeof(Func<DayItem,bool>), typeof(Calendar), null);
        public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(Calendar), default(DateTime?));
        public static readonly BindableProperty IsMarkedDayProperty = BindableProperty.Create(nameof(IsMarkedDay), typeof(Func<DayItem,bool>), typeof(Calendar), default(Func<DayItem,bool>));
        //public static readonly BindableProperty OnSelectionProperty = BindableProperty.Create(nameof(OnSelection), typeof(Action<DayItem>), typeof(Calendar), default(Func<DayItem,bool>));
        public static readonly BindableProperty OnSetupDayActionProperty = BindableProperty.Create(nameof(OnSetupDayAction), typeof(Action<DayItem>), typeof(Calendar), default(Action<DayItem>));
        public static readonly BindableProperty BaseDateProperty = BindableProperty.Create(nameof(BaseDate), typeof(DateTime), typeof(Calendar), DateTime.Now);
        public static readonly BindableProperty OnSelectionDayCommandProperty = BindableProperty.Create(nameof(OnSelectionDayCommand), typeof(ICommand), typeof(Calendar), default(ICommand));
        public static readonly BindableProperty OnDaySelectionFunctionProperty = BindableProperty.Create(nameof(OnDaySelectionFunction), typeof(Func<DayItem,bool>), typeof(Calendar), default(Func<DayItem,bool>));

        public Func<DayItem,bool> OnDaySelectionFunction
        {
            get { return (Func<DayItem,bool>)GetValue(OnDaySelectionFunctionProperty); }
            set { SetValue(OnDaySelectionFunctionProperty, value); }
        }


        public ICommand OnSelectionDayCommand
        {
            get { return (ICommand)GetValue(OnSelectionDayCommandProperty); }
            set { SetValue(OnSelectionDayCommandProperty, value); }
        }

        // Current day
        public static readonly BindableProperty CurrentDayTextColorProperty = BindableProperty.Create(nameof(CurrentDayTextColor), typeof(Color), typeof(Calendar), Color.White);
        public static readonly BindableProperty CurrentDayCircleColorProperty = BindableProperty.Create(nameof(CurrentDayCircleColor), typeof(Color), typeof(Calendar), Color.FromHex("#F13539"));
        public static readonly BindableProperty CurrentDayStrokeColorProperty = BindableProperty.Create(nameof(CurrentDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty CurrentDayStrokeWidthProperty = BindableProperty.Create(nameof(CurrentDayStrokeWidth), typeof(double), typeof(Calendar), default(double));

        // inactive day
        public static readonly BindableProperty InactiveDayTextColorProperty = BindableProperty.Create(nameof(InactiveDayTextColor), typeof(Color), typeof(Calendar), Color.FromHex("A8A8A8"));
        public static readonly BindableProperty InactiveDayCircleColorProperty = BindableProperty.Create(nameof(InactiveDayCircleColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty InactiveDayStrokeColorProperty = BindableProperty.Create(nameof(InactiveDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty InactiveDayStrokeWidthProperty = BindableProperty.Create(nameof(InactiveDayStrokeWidth), typeof(double), typeof(Calendar), default(double));

        // day of another month
        public static readonly BindableProperty AnotherMonthDayTextColorProperty = BindableProperty.Create(nameof(AnotherMonthDayTextColor), typeof(Color), typeof(Calendar), Color.FromHex("A8A8A8"));
        public static readonly BindableProperty AnotherMonthDayCircleColorProperty = BindableProperty.Create(nameof(AnotherMonthDayCircleColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty AnotherMonthDayStrokeColorProperty = BindableProperty.Create(nameof(AnotherMonthDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty AnotherMonthDayStrokeWidthProperty = BindableProperty.Create(nameof(AnotherMonthDayStrokeWidth), typeof(double), typeof(Calendar), default(double));

        // weekend day
        public static readonly BindableProperty WeekendDayTextColorProperty = BindableProperty.Create(nameof(WeekendDayTextColor), typeof(Color), typeof(Calendar), Color.FromHex("A8A8A8"));
        public static readonly BindableProperty WeekendDayCircleColorProperty = BindableProperty.Create(nameof(WeekendDayCircleColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty WeekendDayStrokeColorProperty = BindableProperty.Create(nameof(WeekendDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty WeekendDayStrokeWidthProperty = BindableProperty.Create(nameof(WeekendDayStrokeWidth), typeof(double), typeof(double), default(double));

        // ordinary day
        public static readonly BindableProperty OrdinaryDayTextColorProperty = BindableProperty.Create(nameof(OrdinaryDayTextColor), typeof(Color), typeof(Calendar), Color.Black);
        public static readonly BindableProperty OrdinaryDayCircleColorProperty = BindableProperty.Create(nameof(OrdinaryDayCircleColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty OrdinaryDayStrokeColorProperty = BindableProperty.Create(nameof(OrdinaryDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty OrdinaryDayStrokeWidthProperty = BindableProperty.Create(nameof(OrdinaryDayStrokeWidth), typeof(double), typeof(Calendar), default(double));

        // selected day
        public static readonly BindableProperty SelectedDayTextColorProperty = BindableProperty.Create(nameof(SelectedDayTextColor), typeof(Color), typeof(Calendar), Color.White);
        public static readonly BindableProperty SelectedDayCircleColorProperty = BindableProperty.Create(nameof(SelectedDayCircleColor), typeof(Color), typeof(Calendar), Color.Black);
        public static readonly BindableProperty SelectedDayStrokeColorProperty = BindableProperty.Create(nameof(SelectedDayStrokeColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty SelectedDayStrokeWidthProperty = BindableProperty.Create(nameof(SelectedDayStrokeWidth), typeof(double), typeof(Calendar), 0d);

        // marked day
        public static readonly BindableProperty MarkedDayTextColorProperty = BindableProperty.Create(nameof(MarkedDayTextColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty MarkedDayCircleColorProperty = BindableProperty.Create(nameof(MarkedDayCircleColor), typeof(Color), typeof(Calendar), Material.NoColor);
        public static readonly BindableProperty MarkedDayStrokeColorProperty = BindableProperty.Create(nameof(MarkedDayStrokeColor), typeof(Color), typeof(Calendar), Color.Black);
        public static readonly BindableProperty MarkedDayStrokeWidthProperty = BindableProperty.Create(nameof(MarkedDayStrokeWidth), typeof(double), typeof(Calendar), 1d);

        // Templates
        public static readonly BindableProperty HeadTemplateProperty = BindableProperty.Create(nameof(HeadTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));
        public static readonly BindableProperty DayTemplateProperty = BindableProperty.Create(nameof(DayTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));
        public static readonly BindableProperty WeekStartTemplateProperty = BindableProperty.Create(nameof(WeekStartTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));
        public static readonly BindableProperty WeekTitleTemplateProperty = BindableProperty.Create(nameof(WeekTitleTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));
        public static readonly BindableProperty WeekSeparatorTemplateProperty = BindableProperty.Create(nameof(WeekSeparatorTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));
        public static readonly BindableProperty SeparatorTemplateProperty = BindableProperty.Create(nameof(SeparatorTemplate), typeof(ControlTemplate), typeof(Calendar), default(ControlTemplate));

        // Week Title
        public static readonly BindableProperty WeekTitleColorProperty = BindableProperty.Create(nameof(WeekTitleColor), typeof(Color), typeof(Calendar), Color.Black);
        public static readonly BindableProperty WeekTitleFontSizeProperty = BindableProperty.Create(nameof(WeekTitleFontSize), typeof(double), typeof(Calendar), 15d);
        public static readonly BindableProperty WeekTitleMarginProperty = BindableProperty.Create(nameof(WeekTitleMargin), typeof(Thickness), typeof(Calendar), default(Thickness));
        public static readonly BindableProperty WeekTitleFontFamilyProperty = BindableProperty.Create(nameof(WeekTitleFontFamily), typeof(string), typeof(Calendar), default(string));

        #region Properties

        public DateTime BaseDate
        {
            get { return (DateTime)GetValue(BaseDateProperty); }
            set { SetValue(BaseDateProperty, value); }
        }

        public Action<DayItem> OnSetupDayAction
        {
            get { return (Action<DayItem>)GetValue(OnSetupDayActionProperty); }
            set { SetValue(OnSetupDayActionProperty, value); }
        }

        public string WeekTitleFontFamily
        {
            get { return (string)GetValue(WeekTitleFontFamilyProperty); }
            set { SetValue(WeekTitleFontFamilyProperty, value); }
        }

        public Thickness WeekTitleMargin
        {
            get { return (Thickness)GetValue(WeekTitleMarginProperty); }
            set { SetValue(WeekTitleMarginProperty, value); }
        }

        public double WeekTitleFontSize
        {
            get { return (double)GetValue(WeekTitleFontSizeProperty); }
            set { SetValue(WeekTitleFontSizeProperty, value); }
        }

        public Color WeekTitleColor
        {
            get { return (Color)GetValue(WeekTitleColorProperty); }
            set { SetValue(WeekTitleColorProperty, value); }
        }

        public ControlTemplate SeparatorTemplate
        {
            get { return (ControlTemplate)GetValue(SeparatorTemplateProperty); }
            set { SetValue(SeparatorTemplateProperty, value); }
        }

        public ControlTemplate WeekSeparatorTemplate
        {
            get { return (ControlTemplate)GetValue(WeekSeparatorTemplateProperty); }
            set { SetValue(WeekSeparatorTemplateProperty, value); }
        }

        public ControlTemplate WeekTitleTemplate
        {
            get { return (ControlTemplate)GetValue(WeekTitleTemplateProperty); }
            set { SetValue(WeekTitleTemplateProperty, value); }
        }

        public ControlTemplate WeekStartTemplate
        {
            get { return (ControlTemplate)GetValue(WeekStartTemplateProperty); }
            set { SetValue(WeekStartTemplateProperty, value); }
        }

        public ControlTemplate DayTemplate
        {
            get { return (ControlTemplate)GetValue(DayTemplateProperty); }
            set { SetValue(DayTemplateProperty, value); }
        }

        public ControlTemplate HeadTemplate
        {
            get { return (ControlTemplate)GetValue(HeadTemplateProperty); }
            set { SetValue(HeadTemplateProperty, value); }
        }

        /*
        public Action<DayItem> OnSelection
        {
            get { return (Action<DayItem>)GetValue(OnSelectionProperty); }
            set { SetValue(OnSelectionProperty, value); }
        }
        */
        public double MarkedDayStrokeWidth
        {
            get { return (double)GetValue(MarkedDayStrokeWidthProperty); }
            set { SetValue(MarkedDayStrokeWidthProperty, value); }
        }

        public Color MarkedDayStrokeColor
        {
            get { return (Color)GetValue(MarkedDayStrokeColorProperty); }
            set { SetValue(MarkedDayStrokeColorProperty, value); }
        }

        public Color MarkedDayCircleColor
        {
            get { return (Color)GetValue(MarkedDayCircleColorProperty); }
            set { SetValue(MarkedDayCircleColorProperty, value); }
        }

        public Color MarkedDayTextColor
        {
            get { return (Color)GetValue(MarkedDayTextColorProperty); }
            set { SetValue(MarkedDayTextColorProperty, value); }
        }

        public Func<DayItem, bool> IsMarkedDay
        {
            get { return (Func<DayItem, bool>)GetValue(IsMarkedDayProperty); }
            set { SetValue(IsMarkedDayProperty, value); }
        }


        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public double SelectedDayStrokeWidth
        {
            get { return (double)GetValue(SelectedDayStrokeWidthProperty); }
            set { SetValue(SelectedDayStrokeWidthProperty, value); }
        }

        public Color SelectedDayStrokeColor
        {
            get { return (Color)GetValue(SelectedDayStrokeColorProperty); }
            set { SetValue(SelectedDayStrokeColorProperty, value); }
        }

        public Color SelectedDayCircleColor
        {
            get { return (Color)GetValue(SelectedDayCircleColorProperty); }
            set { SetValue(SelectedDayCircleColorProperty, value); }
        }

        public Color SelectedDayTextColor
        {
            get { return (Color)GetValue(SelectedDayTextColorProperty); }
            set { SetValue(SelectedDayTextColorProperty, value); }
        }

        public double OrdinaryDayStrokeWidth
        {
            get { return (double)GetValue(OrdinaryDayStrokeWidthProperty); }
            set { SetValue(OrdinaryDayStrokeWidthProperty, value); }
        }

        public Color OrdinaryDayStrokeColor
        {
            get { return (Color)GetValue(OrdinaryDayStrokeColorProperty); }
            set { SetValue(OrdinaryDayStrokeColorProperty, value); }
        }


        public Color OrdinaryDayCircleColor
        {
            get { return (Color)GetValue(OrdinaryDayCircleColorProperty); }
            set { SetValue(OrdinaryDayCircleColorProperty, value); }
        }

        public Color OrdinaryDayTextColor
        {
            get { return (Color)GetValue(OrdinaryDayTextColorProperty); }
            set { SetValue(OrdinaryDayTextColorProperty, value); }
        }

        public double WeekendDayStrokeWidth
        {
            get { return (double)GetValue(WeekendDayStrokeWidthProperty); }
            set { SetValue(WeekendDayStrokeWidthProperty, value); }
        }

        public Color WeekendDayStrokeColor
        {
            get { return (Color)GetValue(WeekendDayStrokeColorProperty); }
            set { SetValue(WeekendDayStrokeColorProperty, value); }
        }

        public Color WeekendDayCircleColor
        {
            get { return (Color)GetValue(WeekendDayCircleColorProperty); }
            set { SetValue(WeekendDayCircleColorProperty, value); }
        }

        public Color WeekendDayTextColor
        {
            get { return (Color)GetValue(WeekendDayTextColorProperty); }
            set { SetValue(WeekendDayTextColorProperty, value); }
        }

        public Func<DayItem, bool> IsInactiveDayFunc
        {
            get { return (Func<DayItem, bool>)GetValue(IsInactiveDayFuncProperty); }
            set { SetValue(IsInactiveDayFuncProperty, value); }
        }

        public double AnotherMonthDayStrokeWidth
        {
            get { return (double)GetValue(AnotherMonthDayStrokeWidthProperty); }
            set { SetValue(AnotherMonthDayStrokeWidthProperty, value); }
        }

        public Color AnotherMonthDayStrokeColor
        {
            get { return (Color)GetValue(AnotherMonthDayStrokeColorProperty); }
            set { SetValue(AnotherMonthDayStrokeColorProperty, value); }
        }

        public Color AnotherMonthDayCircleColor
        {
            get { return (Color)GetValue(AnotherMonthDayCircleColorProperty); }
            set { SetValue(AnotherMonthDayCircleColorProperty, value); }
        }

        public Color AnotherMonthDayTextColor
        {
            get { return (Color)GetValue(AnotherMonthDayTextColorProperty); }
            set { SetValue(AnotherMonthDayTextColorProperty, value); }
        }

        public double InactiveDayStrokeWidth
        {
            get { return (double)GetValue(InactiveDayStrokeWidthProperty); }
            set { SetValue(InactiveDayStrokeWidthProperty, value); }
        }

        public Color InactiveDayStrokeColor
        {
            get { return (Color)GetValue(InactiveDayStrokeColorProperty); }
            set { SetValue(InactiveDayStrokeColorProperty, value); }
        }

        public Color InactiveDayCircleColor
        {
            get { return (Color)GetValue(InactiveDayCircleColorProperty); }
            set { SetValue(InactiveDayCircleColorProperty, value); }
        }

        public Color InactiveDayTextColor
        {
            get { return (Color)GetValue(InactiveDayTextColorProperty); }
            set { SetValue(InactiveDayTextColorProperty, value); }
        }

        public double CurrentDayStrokeWidth
        {
            get { return (double)GetValue(CurrentDayStrokeWidthProperty); }
            set { SetValue(CurrentDayStrokeWidthProperty, value); }
        }

        public Color CurrentDayStrokeColor
        {
            get { return (Color)GetValue(CurrentDayStrokeColorProperty); }
            set { SetValue(CurrentDayStrokeColorProperty, value); }
        }

        public Color CurrentDayCircleColor
        {
            get { return (Color)GetValue(CurrentDayCircleColorProperty); }
            set { SetValue(CurrentDayCircleColorProperty, value); }
        }

        public Color CurrentDayTextColor
        {
            get { return (Color)GetValue(CurrentDayTextColorProperty); }
            set { SetValue(CurrentDayTextColorProperty, value); }
        }

        #endregion
         

        WeekItem[] Weeks;
        WeekTitleItem[] WeekTitle;

        public Command TapDayCommand { get; set; }
        public Command ChangeMonthCommand { get; set; }

        //public DateTime baseDate { get; set; } = BaseDate;

        public Calendar()
        {
            InitializeComponent();
        }

        void Build()
        {
            DisableLayout = true;

            TapDayCommand = new Command<DayItem>(day =>
            {
                if (day.InactiveDay)
                    return;

                bool isDaySelectable = OnDaySelectionFunction?.Invoke(day) ?? true;
                if (!isDaySelectable)
                    return;

                SelectedDate = day.DateTime;
                FillCalendar();

                if(OnSelectionDayCommand?.CanExecute(day) ?? false)
                {
                    OnSelectionDayCommand.Execute(day);
                }
                //OnSelection?.Invoke(day);
            });

            ChangeMonthCommand = new Command<string>(param => {
                BaseDate = BaseDate.AddMonths(param == "+1" ? 1 : -1);
                FillCalendar();
            });

            var dayTemplate = DayTemplate ?? GetControlTemplate("_DayTemplate");
            var weekStartTemplate = WeekStartTemplate ?? GetControlTemplate("_weekStartTemplate");
            var separatorTemplate = SeparatorTemplate ?? GetControlTemplate("_SeparatorTemplate");
            var headTemplate = HeadTemplate ?? GetControlTemplate("_headTemplate");
            var weekTitleTemlate = WeekTitleTemplate ?? GetControlTemplate("_weekTitleTemlate");
            var weekSeparatorTemplate = WeekSeparatorTemplate ?? GetControlTemplate("_weekSeparatorTemplate");

            Weeks = new WeekItem[6];
            WeekTitle = new WeekTitleItem[7];
            DateTime firstDay = FirstDayOfWeek(DateTime.Now);
            for (int i = 0; i < 7; i++)
            {
                WeekTitle[i] = new WeekTitleItem();
                WeekTitle[i].Name = firstDay.AddDays(i).ToString("ddd");
            }


            // Title
            this.Children.Add(GetViewFromTemplate(headTemplate, this), 0, 8, 0, 1);

            // Weeks titles
            for (int i = 0; i < 7; i++)
            {
                this.Children.Add(GetViewFromTemplate(weekTitleTemlate, WeekTitle[i]), i + 1, 1);
            }
            this.Children.Add(GetViewFromTemplate(weekSeparatorTemplate, this), 0, 8, 1, 2);


            for (int week = 0; week < 6; week++)
            {
                Weeks[week] = new WeekItem();

                this.Children.Add(GetViewFromTemplate(weekStartTemplate, Weeks[week]), 0, 2 + week);
                this.Children.Add(GetViewFromTemplate(separatorTemplate, Weeks[week]), 1, 8, 2 + week, 3 + week);

                for (int day = 0; day < 7; day++)
                {
                    View view = GetViewFromTemplate(dayTemplate, Weeks[week].Days[day]);
                    Weeks[week].Days[day].View = new WeakReference<View>(view);
                    this.Children.Add(view, day + 1, 2 + week);
                }
            }

            FillCalendar();

            DisableLayout = false;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            Build();

            Device.BeginInvokeOnMainThread(async () =>
            {
                await System.Threading.Tasks.Task.Delay(50);
                FillCalendar();
            });

        }

        DateTime FirstDayOfWeek(DateTime dateTime)
        {
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
            var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = dateTime.DayOfWeek - firstDayOfWeek < 0 ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDayOfWeek;

            return dateTime.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }

        ControlTemplate GetControlTemplate(string name)
        {
            this.Resources.TryGetValue(name, out object _template);
            return _template as ControlTemplate;
        }

        View GetViewFromTemplate(ControlTemplate template, object context)
        {
            View view = template.CreateContent() as View;
            view.BindingContext = context;
            return view;
        }

        DateTime FirstDayOfMonth(DateTime current)
        {
            return SetDay(current,1);
        }

        DateTime SetDay(DateTime value, int day)
        {
            return new DateTime(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        public void FillCalendar()
        {
            if (BaseDate == default(DateTime))
                return;

            DisableLayout = true;

            DateTime startingDay =  FirstDayOfMonth(BaseDate);

            PrevMonth = FirstDayOfMonth(startingDay.AddDays(-2));
            NextMonth = FirstDayOfMonth(startingDay.AddDays(35));

            startingDay = startingDay.AddDays(-DayOfWeekInCulture(startingDay));
            foreach (var week in Weeks)
            {
                week.WeekOfYear = (int)Math.Ceiling((double)startingDay.DayOfYear / 7);
                foreach (var day in week.Days)
                {
                    day.DateTime = startingDay;
                    day.DayOfSelectedMonth = startingDay.Month == BaseDate.Month;
                    day.InactiveDay = IsInactiveDayFunc == null ? false : IsInactiveDayFunc(day);
                    SetupDay(day);

                    startingDay = startingDay.AddDays(1);

                }

                // Hide weeks from next month
                week.IsVisible = week.Days.Any(x => x.DayOfSelectedMonth);
                week.Days.ForEach(x => x.IsVisible = week.IsVisible);


            }

            DisableLayout = false;

        }

        bool isHoliday(DateTime dateTime)
        {
            if(dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
                return true;

            return false;
        }

        public virtual void SetupDay(DayItem dayItem)
        {

            OnSetupDayAction?.Invoke(dayItem);
            
            // Current day
            if (dayItem.DateTime.Date == DateTime.Now.Date)
            {
                dayItem.TextColor = CurrentDayTextColor;
                dayItem.CircleColor = CurrentDayCircleColor;
                dayItem.StrokeColor = CurrentDayStrokeColor;
                dayItem.StrokeWidth = CurrentDayStrokeWidth;
            }
            // Day from another month
            else if (!dayItem.DayOfSelectedMonth)
            {
                dayItem.TextColor = AnotherMonthDayTextColor;
                dayItem.CircleColor = AnotherMonthDayCircleColor;
                dayItem.StrokeColor = AnotherMonthDayStrokeColor;
                dayItem.StrokeWidth = AnotherMonthDayStrokeWidth;
            }
            // Holiday
            else if (isHoliday(dayItem.DateTime))
            {
                dayItem.TextColor = WeekendDayTextColor;
                dayItem.CircleColor = WeekendDayCircleColor;
                dayItem.StrokeColor = WeekendDayStrokeColor;
                dayItem.StrokeWidth = WeekendDayStrokeWidth;
            }
            // Ordinary day
            else
            {
                dayItem.TextColor = OrdinaryDayTextColor;
                dayItem.CircleColor = OrdinaryDayCircleColor;
                dayItem.StrokeColor = OrdinaryDayStrokeColor;
                dayItem.StrokeWidth = OrdinaryDayStrokeWidth;
            }


            if (dayItem.InactiveDay)
            {
                dayItem.TextColor = InactiveDayTextColor;
                dayItem.CircleColor = InactiveDayCircleColor;
                dayItem.StrokeColor = InactiveDayStrokeColor;
                dayItem.StrokeWidth = InactiveDayStrokeWidth;
            }

            // Marked day
            if(IsMarkedDay?.Invoke(dayItem) ?? false)
            {
                ApplyStyle(dayItem, MarkedDayTextColor, MarkedDayCircleColor, MarkedDayStrokeColor, MarkedDayStrokeWidth);
            }

            if (SelectedDate != null &&  dayItem.DateTime.Date == ((DateTime)SelectedDate).Date)
            {
                dayItem.TextColor = SelectedDayTextColor;
                dayItem.CircleColor = SelectedDayCircleColor;
                dayItem.StrokeColor = SelectedDayStrokeColor;
                dayItem.StrokeWidth = SelectedDayStrokeWidth;
            }

        }

        void ApplyStyle(DayItem dayItem,Color textColor, Color circleColor,Color strokeColor,double strokeWidth)
        {
            if (textColor != Material.NoColor)
                dayItem.TextColor = textColor;

            if (circleColor != Material.NoColor)
                dayItem.CircleColor = circleColor;

            if (strokeColor != Material.NoColor)
                dayItem.StrokeColor = strokeColor;

            if (strokeWidth > 0)
                dayItem.StrokeWidth = strokeWidth;

        }

        int DayOfWeekInCulture(DateTime dateTime)
        {
            int i = (int)dateTime.DayOfWeek;
            if (CoreApp.Current.CurrentCulture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Monday)
            {
                i = i == 0 ? i = 6 : i - 1;
            }
            return i;
        }


    }
}