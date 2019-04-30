
using System.Reflection;
using System.Text;

namespace CoreXF
{
    public static class JSonHelper
    {
        public static string SerializeShallowToJson<D>(this D source, int maxparamlength = -1) where D : class
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append("{");
            bool firstItem = true;
            foreach (PropertyInfo sprop in source.GetType().GetRuntimeProperties())
            {
                object val = sprop.GetValue(source, null);
                if (val == null)
                    continue;

                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append("\"");
                sb.Append(sprop.Name);
                sb.Append("\": \"");

                string valstr = val.ToString();
                if (maxparamlength > 0 && valstr.Length > maxparamlength)
                {
                    valstr = valstr.Substring(0, maxparamlength - 3) + "...";
                }
                sb.Append(valstr);
                sb.Append("\"");
            }
            sb.Append("}");
            return sb.ToString();
            /*
            foreach (FieldInfo sourceProp in source.GetType().GetRuntimeFields())
            {
                if (!sourceProp.IsPublic) continue;
                object sourceValue = sourceProp.GetValue(source);
                object distValue = sourceProp.GetValue(destination);
                if (sourceValue == distValue) continue;
                sourceProp.SetValue(destination, sourceValue);
            }
            */
        }


    }
}
