
using CoreXF;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExceptionAlert : ContentView, IDialogXF
    {
        public CaughtExceptionModel ExceptionModel { get; set; }

        public bool SendError { get; set; } = true;
        public Command ShowDetailCommand { get; set; }
        public bool DetailVisibility { get; set; }

        public Command CloseCommand { get; set; }

        public class Parameters : DialogParameters
        {
            public CaughtExceptionModel ExceptionModel;
        }

        public ExceptionAlert() => InitializeComponent();

        public void Initialize(DialogParameters param)
        {
            ExceptionModel = (param as Parameters)?.ExceptionModel;
            ShowDetailCommand = new Command(() => DetailVisibility = !DetailVisibility);

        }
    }
}