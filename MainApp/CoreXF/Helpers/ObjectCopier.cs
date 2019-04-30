
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreXF
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

        public static object Clone<S>(this S source, Type type) where S : class
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source), type);
        }

        public static D Clone<S, D>(this S source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(D);
            }

            return JsonConvert.DeserializeObject<D>(JsonConvert.SerializeObject(source));
        }

        /*
        public static void Fill<S,D>(this D destination,S source){
            Type destType = destination.GetType();
            foreach (PropertyInfo sprop in source.GetType().GetRuntimeProperties()){
                PropertyInfo dprop = destType.GetRuntimeProperty(sprop.Name);
                if (dprop == null || !dprop.CanWrite) continue;
                dprop.SetValue(destination, sprop.GetValue(source,null),null);
            }
        }
        */

        /// <summary>
        /// Filling a destination object from a source object
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="destination">Destination object</param>
        /// <param name="source">Source object</param>
        public static D Fill<D>(this D destination, object source) where D : class
        {
            Type destType = destination.GetType();

            foreach (PropertyInfo sprop in source.GetType().GetRuntimeProperties().Where(x => x.CanWrite))
            {
                PropertyInfo dprop = destType.GetRuntimeProperty(sprop.Name);
                if (dprop == null || !dprop.CanWrite)
                    continue;

                dprop.SetValue(destination, sprop.GetValue(source, null), null);
            }
            foreach (FieldInfo sourceProp in source.GetType().GetRuntimeFields())
            {
                if (!sourceProp.IsPublic) continue;
                object sourceValue = sourceProp.GetValue(source);
                object distValue = sourceProp.GetValue(destination);
                if (sourceValue == distValue) continue;
                sourceProp.SetValue(destination, sourceValue);
            }
            return destination;
        }

        /// <summary>
        /// Merge one list to another list
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="destinationList"></param>
        /// <param name="sourceList"></param>
        /// <param name=""></param>
        public static void MergeLists<D, S>(this D destinationList, S sourceList, bool deleteItemsIfTheyDontExistInSource = false)
            where D : class, IList, IEnumerable<object>
            where S : class, IList
        {
            if (sourceList == null) return;

            IList extItems = null;
            if (deleteItemsIfTheyDontExistInSource)
            {
                extItems = new List<object>(destinationList);
            }

            Type type = (destinationList.GetType()).GenericTypeArguments[0];

            List<PropertyInfo> propertyList = type.GetRuntimeProperties().Where(x => x.CanWrite).ToList();
            List<FieldInfo> fieldList = type.GetRuntimeFields().Where(x => x.IsPublic).ToList();

            for (int i = 0; i < sourceList.Count; i++)
            {
                object srcVal = sourceList[i];

                // add new items
                int idx = destinationList.IndexOf(srcVal);
                if (idx == -1)
                {
                    destinationList.Add(srcVal);
                    continue;
                }

                // merge existing items
                object dstVal = destinationList[idx];

                if (deleteItemsIfTheyDontExistInSource)
                {
                    extItems.Remove(dstVal);
                }

                // copy properties
                for (int y = 0; y < propertyList.Count; y++) propertyList[y].SetValue(dstVal, propertyList[y].GetValue(srcVal, null));

                // copy fields
                for (int y = 0; y < fieldList.Count; y++) fieldList[y].SetValue(dstVal, fieldList[y].GetValue(srcVal));

            }

            if (deleteItemsIfTheyDontExistInSource)
            {
                for (int i = 0; i < extItems.Count; i++)
                {
                    destinationList.Remove(extItems[i]);
                }
            }


        }
        /// <summary>
        /// It filling a destination object from a source object, copying only changed objects
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="destination">Destination object</param>
        /// <param name="source">Source object</param>
        ///
        /*
        public static void FillShallowOnlyChanged<D>(this D destination, D source) where D : class
        {
            foreach (PropertyInfo sourceProp in source.GetType().GetRuntimeProperties())
            {
                object sourceValue = sourceProp.GetValue(source, null);
                object distValue = sourceProp.GetValue(destination, null);
                if (sourceValue == distValue) continue;
                sourceProp.SetValue(destination, sourceValue , null);
            }

            foreach (FieldInfo sourceProp in source.GetType().GetRuntimeFields())
            {
                object sourceValue = sourceProp.GetValue(source);
                object distValue = sourceProp.GetValue(destination);
                if (sourceValue == distValue) continue;
                sourceProp.SetValue(destination, sourceValue);
            }

        }
        */

    }

}
