

using System.Threading.Tasks;
using Xamarin.Forms;

namespace CoreXF
{
    public interface IMasterDetailController
    {
        Command OpenLeftSideMenu { get; set; }
        Task OpenFragmentAsync<T>(PageParameters param) where T : Page, IPageLifeCycle;
    }
}
