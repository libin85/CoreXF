
using CoreXF;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CalendarDialog : ContentView, IDialogXF
    {
        public Parameters Params { get; set; }
        public Command CloseCommand { get; set; }

        public class Parameters : DialogTemplate.Parameters
        {
            public string Text { get; set; }
            public Action<string> OnOkAction { get; set; }
            public Action<DayItem> OnSelection { get; set; }

            public Func<DayItem, bool> IsInactiveDayFunc { get; set; }
            public Func<DayItem, bool> IsMarkedDay { get; set; }
            public bool CloseAfterSelectoin { get; set; }
        }
        
        public CalendarDialog()
        {
          
            InitializeComponent();
            _dialogTemplate.Children.Add(_calendar, 0, 1);

            vis().ConfigureAwait(false);
        }
        
        async Task vis()
        {
            await Task.Delay(50);
            _dialogTemplate.IsVisible = true;
        }

        public void Initialize(DialogParameters param)
        {
            Params = param as Parameters;

            _dialogTemplate.Initialize(param);

            _calendar.IsInactiveDayFunc = Params.IsInactiveDayFunc;
            _calendar.IsMarkedDay = Params.IsMarkedDay;

            _calendar.OnSelectionDayCommand = new Command<DayItem>(day =>
            {
                Params.OnSelection?.Invoke(day);
                if(Params.CloseAfterSelectoin)
                    CloseCommand.Execute(null);

            });
            
            _calendar.FillCalendar();

            //_dialogTemplate.HorizontalSeparator.IsVisible = false;

            _dialogTemplate.OkCommand = new Command(() =>
            {
                //Params.OnOkAction?.Invoke(_editor.Text);
                CloseCommand?.Execute(null);
            });

            _dialogTemplate.CloseCommand = CloseCommand;

        }

#if DEBUG
        void t_LiveXAML() => Initialize(null);
#endif

    }
}