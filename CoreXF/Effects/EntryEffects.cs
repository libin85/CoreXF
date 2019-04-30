using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace CoreXF
{
    public class EntryEffectsEffect : RoutingEffect
    {
        public const string Name = nameof(CoreXF)+"."+nameof(EntryEffectsEffect);

        public bool Borderless { get; set; }

        // Bottom line 
        public Color AndroidLineColor { get; set; } = Material.NoColor;

        public EntryEffectsEffect(): base(Name)
        {

        }
    }
}
