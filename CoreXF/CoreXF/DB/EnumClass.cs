
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreXF
{
    public class EnumClass<T> where T : struct
    {
        public int EnumCode { get; set; }

        public override string ToString()
        {
            string name = Enum.GetName(typeof(T), EnumCode);
            string key = $"Enum_{typeof(T).Name}_{name}";
            return Tx.T(key);
        }

        public static EnumClass<T> GetValue(int? Id)
        {
            if (Id == null)
                return null;

            return Collection.FirstOrDefault(x => x.EnumCode == (int)Id);
        }

        public static List<EnumClass<T>> Collection { get {
                if(_Collection == null)
                {
                    _Collection = new List<EnumClass<T>>();
                    foreach (var elm in Enum.GetValues(typeof(T)))
                    {
                        _Collection.Add(new EnumClass<T> { EnumCode = (int)elm });
                    }
                }
                return _Collection;
            } }
        static List<EnumClass<T>> _Collection;
    }
}
