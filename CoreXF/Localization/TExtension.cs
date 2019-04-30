
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Xaml.Internals;

namespace CoreXF
{
    public static class Tx
    {
        public static string T(string key,int? Count = null)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            if(Count != null)
            {
                int i = 4;
            }

            if (!string.IsNullOrEmpty(AppConfig.ConfigPrefix))
            {
                string configurationSpecificKey = $"{key}.{AppConfig.ConfigPrefix}";
                string res = GetStringByKey(configurationSpecificKey);
                if (!string.IsNullOrEmpty(res))
                    return res;
            }
            
            string res2 = GetStringByKey(key);
            return string.IsNullOrEmpty(res2) ? key : res2;
        }

        static string GetStringByKey(string key)
        {
            for (int i = 0; i < CoreApp.StringResourceManagers.Count; i++)
            {
                var res = CoreApp.StringResourceManagers[i].GetString(key, CoreApp.Current.CurrentCulture);
                if (!String.IsNullOrEmpty(res))
                {
                    return res;
                }
                res = CoreApp.StringResourceManagers[i].GetString(key, CoreApp.Current.NeutralCulture);
                if (!String.IsNullOrEmpty(res))
                {
                    return res;
                }

                //return res;
            }
            return null;
        }
    }

    [ContentProperty(nameof(Key))]
    public class TExtension : IMarkupExtension
    {
        public string Key { get; set; }

        public int? Count { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
            => string.IsNullOrEmpty(Key) ? "" : Tx.T(Key, Count);
    }


    // This extension is applied when need to immediately update localized text
    [ContentProperty(nameof(Key))]
    public class TLocExtension : IMarkupExtension
    {
        class LocItem
        {
            public object visualElement;
            public string propertyName;
            public string key;
        }

        static List<LocItem> _local = new List<LocItem>();

        public string Key { get; set; }

        public static void UpdateLocale()
        {
            Type type;
            PropertyInfo pi;
            string strVal;
            foreach (var elm in _local)
            {
                type = elm.visualElement.GetType();
                pi = type.GetRuntimeProperty(elm.propertyName);
                strVal = Tx.T(elm.key);
                pi.SetValue(elm.visualElement, strVal);
            }
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
                return "";

            XamlServiceProvider xamls = serviceProvider as XamlServiceProvider;
            IProvideValueTarget ivt = xamls?.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            object visualElement = ivt?.TargetObject;
            BindableProperty prop = ivt?.TargetProperty as BindableProperty;
            PropertyInfo pprop = ivt?.TargetProperty as PropertyInfo;

            LocItem li = new LocItem
            {
                propertyName = prop?.PropertyName ?? pprop?.Name,
                key = Key,
                visualElement = visualElement
            };

            if (string.IsNullOrEmpty(li.propertyName) || string.IsNullOrEmpty(li.key) || li.visualElement == null)
            {
                Debug.WriteLine($"Localize extension error {li.propertyName}   {li.key}   {li.visualElement}");
            }
            else
            {
                _local.Add(li);
            }

            string local = Tx.T(Key);
            return local;
        }


    }

}
