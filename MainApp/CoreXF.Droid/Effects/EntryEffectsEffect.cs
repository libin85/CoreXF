using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CoreXF.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(nameof(CoreXF))]
[assembly: ExportEffect(typeof(EntryEffectsEffect),nameof(EntryEffectsEffect))]
namespace CoreXF.Droid
{
    public class EntryEffectsEffect : PlatformEffect
    {
        bool registeredFocus;

        EditText field;
        CoreXF.EntryEffectsEffect effect;

        protected override void OnAttached()
        {
            effect = Element?.Effects?.FirstOrDefault(x => x is CoreXF.EntryEffectsEffect) as CoreXF.EntryEffectsEffect;
            if (effect == null)
                return;

            field = Control as EditText;

            if (effect.Borderless)
            {
                if (field != null)
                {
                    field.SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
            }

            if(effect.AndroidLineColor != Material.NoColor)
            {
                if(field != null)
                {
                    field.FocusChange += Field_FocusChange;
                    registeredFocus = true;

                    field.Background.Mutate().SetColorFilter(Color.Gray.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcAtop);
                }
            }
        }

        private void Field_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                field.Background.Mutate().SetColorFilter(effect.AndroidLineColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcAtop);
            }
            else
            {
                field.Background.Mutate().SetColorFilter(Color.Gray.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcAtop);
            }
        }

        protected override void OnDetached()
        {
            field.FocusChange -= Field_FocusChange;
            field = null;
        }
    }
}