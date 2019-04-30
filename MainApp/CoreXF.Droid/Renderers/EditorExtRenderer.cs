
using System.ComponentModel;
using Android.Content;
using CoreXF;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System;
using Xamarin.Forms.Platform;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Android.Views.InputMethods;

[assembly: ExportRenderer(typeof(EditorExt), typeof(CoreXF.Droid.EditorExtRenderer))]
namespace CoreXF.Droid
{

    public class EditorExtRenderer : EditorRenderer
    {

        public EditorExtRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null && ((Element as EditorExt)?.IsBorderless ?? false))
            {
                this.Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
            SetReturnType();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case nameof(EditorExt.ReturnType):
                    SetReturnType();
                    break;
            }
        }


        private void SetReturnType()
        {
            EditorExt entry = this.Element as EditorExt;
            if (entry == null)
                return;

            ReturnType type = entry.ReturnType;

            switch (type)
            {
                case ReturnType.Go:
                    Control.ImeOptions = ImeAction.Go;
                    Control.SetImeActionLabel("Go", ImeAction.Go);
                    break;
                case ReturnType.Next:
                    Control.ImeOptions = ImeAction.Next;
                    Control.SetImeActionLabel("Next", ImeAction.Next);
                    break;
                case ReturnType.Send:
                    Control.ImeOptions = ImeAction.Send;
                    Control.SetImeActionLabel("Send", ImeAction.Send);
                    break;
                case ReturnType.Search:
                    Control.ImeOptions = ImeAction.Search;
                    Control.SetImeActionLabel("Search", ImeAction.Search);
                    break;
                default:
                    Control.ImeOptions = ImeAction.Done;
                    Control.SetImeActionLabel("Done", ImeAction.Done);
                    break;
            }
        }

    }
}