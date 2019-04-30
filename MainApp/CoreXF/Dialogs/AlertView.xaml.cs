
using CoreXF;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AlertView : ContentView, IDialogXF
	{
        public Parameters Params { get; set; }

        public Command CloseCommand { get; set; }
        public Command DetailsCommand { get; set; }
        public Command<string> OnNavigationFromCommand { get; set; }

        public class Parameters : DialogParameters
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string Cancel { get; set; } = Tx.T("Dialogs_Ok");
            public string HTML { get; set; }
            public string MainLink { get; set; }
        }

        public AlertView () =>InitializeComponent ();

        public void Initialize(DialogParameters param)
        {
            Params = param as Parameters;

            DetailsCommand = new Command(() =>
            {
                if(Params?.MainLink != null)
                {
                    CloseCommand?.Execute(null);
                    //App.NavigationService.NavigateToAsync<WebViewerPage>(new WebViewerPage.Parameters { Source = Params.MainLink, PreventNavigationFromStartingPage = false });
                }
            });

            OnNavigationFromCommand = new Command<string>((url) => {
                CloseCommand?.Execute(null);
                //App.NavigationService.NavigateToAsync<WebViewerPage>(new WebViewerPage.Parameters { Source = url, PreventNavigationFromStartingPage = false });
            });
        }
    }
}