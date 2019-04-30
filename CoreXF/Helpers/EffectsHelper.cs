using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace CoreXF
{
    public static class EffectsHelper
    {
        public static T FindEffect<T>(this Element element,bool AddIfThereIsNoEffect = false) where T : RoutingEffect, new()
        {
            T eff = element?.Effects?.FirstOrDefault(x => x is T) as T;
            if(eff == null)
            {
                eff = new T();
                element.Effects.Add(eff);
            }

            return eff;
        }
    }
}
