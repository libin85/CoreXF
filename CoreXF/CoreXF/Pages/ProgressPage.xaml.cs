
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProgressPage : ContentPage
    {
        public ProgressPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        } 
    }
}