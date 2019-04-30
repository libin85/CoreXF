
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace CoreXF
{

    // Cache for fonts from app resources
    class SKFontCache : GenericCache<string, SKFontCache.Font>
    {
        public static SKFontCache Instance = new SKFontCache();

        public class Font
        {
            public string Name { get; set; }
            public SKTypeface Typeface { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        //
        // Summary:
        //     Enumerates font weights.
        enum UIFontWeight
        {
            UltraLight = 0,
            Thin = 100,
            Light = 200,
            Regular = 300,
            Medium = 400,
            Semibold = 500,
            Bold = 600,
            Heavy = 700,
            Black = 800
        }
        SKTypeface getIosFontWeight(string key)
        {

            if (key.Contains("BoldItalic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Bold, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }
            else if (key.Contains("Bold"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Bold, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("HeavyItalic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Heavy, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }
            else if (key.Contains("Heavy"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Heavy, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("MediumItalic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Medium, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }
            else if (key.Contains("Medium"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Medium, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("Regular"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Regular, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("SemiboldItalic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Semibold, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }
            else if (key.Contains("Semibold"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Semibold, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("LightItalic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Light, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }
            else if (key.Contains("Light"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Light, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            }
            else if (key.Contains("Italic"))
            {
                return SKTypeface.FromFamilyName("Helvetica", (int)UIFontWeight.Regular, (int)SKFontStyleWidth.Normal, SKFontStyleSlant.Italic);
            }

            throw new Exception($"Unknown font {key}");
        }


        public override Font CreateValue(string key)
        {
            if (Device.RuntimePlatform == Device.iOS && key.Contains("SFUI"))
            {
                return new Font
                {
                    Name = key,
                    Typeface = getIosFontWeight(key)
                };
            }
            // Try to get from resource
            
            string fullFontName = "";
            if(Device.RuntimePlatform == Device.Android)
            {
                fullFontName = key.Substring(0, key.IndexOf('#')).Replace("_","-");
            }
            else
            {
                fullFontName = $"Fonts/{key}.ttf";
            }
            using (Stream stream = ResourceLoader.GetStreamFromNativeResouces(fullFontName))
            {
                Font font = new Font
                {
                    Typeface = SKTypeface.FromStream(stream)
                };
                return font;
            }
        }
    }

    // Cache for images from app resources
    public class SKBitmapCache : GenericCache<string, SKBitmap>
    {
        public static readonly SKBitmapCache Instance = new SKBitmapCache();

        public override SKBitmap CreateValue(string key)
        {
            using (var stream = ResourceLoader.GetStream(key))
            {
                SKBitmap bitmap = SKBitmap.Decode(stream);
                return bitmap;
            }
        }
    }

    // Cache for images from app resources
    public class SKSvgCache : GenericCache<string, SKPicture>
    {
        public static readonly SKSvgCache Instance = new SKSvgCache();

        public override SKPicture CreateValue(string key)
        {

            SkiaSharp.Extended.Svg.SKSvg _svg = _svg = new SkiaSharp.Extended.Svg.SKSvg();
            SKPicture picture = _svg.Load(ResourceLoader.GetStream(key));
            /* 
            SKPicture picture;
            if (key.Length > 200)
            {
                using (Stream stream = key.ToStream())
                {
                    picture = _svg.Load(stream);
                }
            }
            else
            {
                picture = _svg.Load(ResourceLoader.GetStream(key));
            }
            */
            return picture;
        }

        public override void Clear()
        {
            Dictionary.ForEach(item => item.Value?.Dispose());
            base.Clear();
        }
    }

    // Cache for  SKData
    public class SKDataCache : GenericCache<string, SKData>
    {
        public static readonly SKDataCache Instance = new SKDataCache();

        public override SKData CreateValue(string key)
        {
            throw new Exception("This function can't be called");
        }

        public override void Clear()
        {
            Dictionary.ForEach(item => item.Value?.Dispose());
            base.Clear();
        }

    }
}
