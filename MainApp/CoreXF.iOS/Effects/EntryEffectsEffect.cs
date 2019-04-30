
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(nameof(CoreXF))]
[assembly: ExportEffect(typeof(CoreXF.iOS.EntryEffectsEffect), nameof(CoreXF.iOS.EntryEffectsEffect))]

namespace CoreXF.iOS
{
    public class EntryEffectsEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            CoreXF.EntryEffectsEffect eff = Element?.FindEffect<CoreXF.EntryEffectsEffect>();
            if(eff == null)
            {
                return;
            }

            UITextField field = Control as UITextField;
            if(field != null)
            {
                if (eff.Borderless)
                {
                    field.BorderStyle = UITextBorderStyle.None;
                }
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
