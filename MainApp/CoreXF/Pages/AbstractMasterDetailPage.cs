
using CoreXF.NavigationAbstraction;
using Xamarin.Forms;

namespace CoreXF
{
    public class AbstractMasterDetailPage : MasterDetailPage, IPageLifeCycle
    {
        public virtual void OnDestroyPage()
        {
            UnapplyBindings();

            ((IPageLifeCycle)Master).OnDestroyPage();
        }

        public virtual void Initialize()
        {
        }
    }
}
