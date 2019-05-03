
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreXF
{
    public class PageParameters 
    {
        public Action OnClose;

        public Func<object, Task> OnSuccessfulCloseAsync;

        public override string ToString() =>
            SerializeShallowToJson(maxparamlength: 50);

        string SerializeShallowToJson(int maxparamlength = -1)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append("{");
            bool firstItem = true;
            foreach (PropertyInfo sprop in this.GetType().GetRuntimeProperties())
            {
                object val = sprop.GetValue(this, null);
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
