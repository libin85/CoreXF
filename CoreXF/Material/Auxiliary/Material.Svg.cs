
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CoreXF
{
    public static class MaterialSvg
    {

        class CSSClass
        {
            public class CSSAttribute
            {
                public string Name { get; set; }
                public string Value { get; set; }
            }

            public string Name { get; set; }

            public List<CSSAttribute> Attributes { get; set; } = new List<CSSAttribute>();

        }

        // <defs><style>.cls-1{fill:#DD8496;}.fil1{fill:#fff;}</style></defs>      
        // class="fil1"
        static string RemoveCSSClasses(string svg)
        {
            if (!svg.Contains("<style>."))
                return svg;

            XDocument xdoc = XDocument.Parse(svg);

            List<XElement> descendats = xdoc.Descendants().ToList();

            var defsNode = descendats.FirstOrDefault(x => x.Name.LocalName == "defs");
            if (defsNode == null)
                return svg;

            // find css styles
            List<CSSClass> classes = new List<CSSClass>();
            var classNodes = defsNode.Descendants().Where(x => x.Name.LocalName == "style" && x.Value.StartsWith(".")).ToList();
            foreach (var node in classNodes)
            {
                var arr = node.Value.Split('.');
                for (int y = 1; y < arr.Length; y++)
                {
                    CSSClass cssclass = new CSSClass();
                    classes.Add(cssclass);

                    int openidx = arr[y].IndexOf('{');
                    if (openidx == -1)
                    {
                        continue;
                    }
                    cssclass.Name = arr[y].Substring(0, openidx);
                    int closeidx = arr[y].IndexOf('}');
                    if (closeidx == -1)
                    {
                        closeidx = arr[y].Length;
                    }
                    string values = null;
                    values = arr[y].Substring(openidx + 1, closeidx - openidx - 1);

                    var valarr = values.Split(';');
                    for (int z = 0; z < valarr.Length; z++)
                    {
                        string str1 = valarr[z];
                        if (string.IsNullOrEmpty(str1))
                            continue;

                        var valsr = str1.Split(':');
                        var vale = new CSSClass.CSSAttribute
                        {
                            Name = valsr[0],
                            Value = valsr[1]
                        };
                        cssclass.Attributes.Add(vale);
                    }
                }
                node.Remove();
            }
            if (defsNode.Descendants().Count() == 0)
            {
                defsNode.Remove();
            }

            // remove title
            descendats.FirstOrDefault(x => x.Name.LocalName == "title")?.Remove();

            foreach (var cls in classes)
            {
                var nodes = descendats.Where(x => x.Attributes().Any(y => y.Name == "class" && y.Value == cls.Name));
                foreach (var node in nodes)
                {
                    XAttribute attr = node.Attributes().FirstOrDefault(x => x.Name == "class");
                    if (attr != null)
                    {
                        foreach (var atr in cls.Attributes)
                        {
                            node.Add(new XAttribute(atr.Name, atr.Value));
                        }
                        attr.Remove();
                    }
                }
            }

            string res = xdoc.ToString();
            return res;
        }


        public static SKPicture CreateSvgFromRawString(string content)
        {
            try
            {
                string processedSvg = RemoveCSSClasses(content);

                var _svg = new SkiaSharp.Extended.Svg.SKSvg();
                using (Stream stream = processedSvg.ToStream())
                {
                    return _svg.Load(stream);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.SendError(ex, "CreateSvgFromRawString");
            }
            return null;
        }


    }
}
