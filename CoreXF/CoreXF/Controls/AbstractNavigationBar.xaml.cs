
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AbstractNavigationBar : Grid
    {
        public AbstractNavigationBar()
        {
            InitializeComponent();
        }
    }
}