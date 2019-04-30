
using System;
using Xamarin.Forms;

namespace CoreXF
{
    public static class ViewHelpers
    {
        public static T FindByType<T>(this Element view) where T : Element
        {
            if (view == null)
                return null;

            if ((view as T) != null)
                return view as T;

            return FindByType<T>(view.Parent);
        }


        public static Element GetTopParent(this Element view)
        {
            Element parent = view.Parent;
            if (parent == null)
            {
                return view;
            }
            else
            {
                return parent.GetTopParent();
            }
        }

        public static void DisposeAllViews(this View view)
        {
            if (view == null)
            {
                return;
            }

            switch (view)
            {
                case ContentView cv:
                    DisposeAllViews(cv.Content);
                    break;

                case ScrollView sv:
                    DisposeAllViews(sv.Content);
                    break;

                case Layout<View> lv:
                    foreach (var elm in lv.Children)
                    {
                        DisposeAllViews(elm);
                    }
                    break;

            }



            IDisposable iDisp = view as IDisposable;
            if (iDisp != null)
            {
                //Debug.WriteLine($"DISPOSE {view}");
                iDisp.Dispose();
            }
        }

    }
}
