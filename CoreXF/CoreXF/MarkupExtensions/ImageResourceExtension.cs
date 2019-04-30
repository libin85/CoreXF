
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreXF
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider) => ImageSourceHelper.FromResources(Source);
    }

}
