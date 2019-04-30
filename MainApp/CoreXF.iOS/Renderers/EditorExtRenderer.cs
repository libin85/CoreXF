using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using CoreXF;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EditorExt), typeof(CoreXF.iOS.EditorExtRenderer))]
namespace CoreXF.iOS
{
    public class EditorExtRenderer : EditorRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                SetBorder();
                SetReturnType(this.Element as EditorExt);
            }

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case nameof(EditorExt.IsBorderless):
                    SetBorder();
                    break;

                case nameof(EditorExt.ReturnType):
                    SetReturnType(this.Element as EditorExt);
                    break;
            }
        }

        private void SetReturnType(EditorExt entry)
        {
            if (entry == null)
                return;

            ReturnType type = entry.ReturnType;

            switch (type)
            {
                case ReturnType.Go:
                    Control.ReturnKeyType = UIReturnKeyType.Go;
                    break;
                case ReturnType.Next:
                    Control.ReturnKeyType = UIReturnKeyType.Next;
                    break;
                case ReturnType.Send:
                    Control.ReturnKeyType = UIReturnKeyType.Send;
                    break;
                case ReturnType.Search:
                    Control.ReturnKeyType = UIReturnKeyType.Search;
                    break;
                case ReturnType.Done:
                    Control.ReturnKeyType = UIReturnKeyType.Done;
                    break;
                default:
                    Control.ReturnKeyType = UIReturnKeyType.Default;
                    break;
            }
        }

        void SetBorder()
        {
            if (Control == null)
                return;

            bool isBorderless = (Element as EditorExt)?.IsBorderless ?? false;
            if (isBorderless)
            {
                Control.Layer.BorderWidth = 0f;
            }
            else
            {
                Control.Layer.BorderWidth = 0.25f;
                Control.Layer.BorderColor = UIColor.LightGray.CGColor;
                Control.Layer.CornerRadius = 5;
            }
        }
    }
}
