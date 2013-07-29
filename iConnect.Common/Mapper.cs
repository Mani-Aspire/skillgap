using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iConnect.Common
{
    public static class Mapper<TSource, TDestination> where TSource : class
    {
        public const string NamespaceDataClasses = "System.Data.Objects.DataClasses";
        public const string Namespace = "iConnect";
        public const string EntityReference = "EntityReference";
        public const string Array = "Array";
        public const string ListPropertyName = "List`1";
        public const string CollectionPropertyName = "Collection`1";
        public const string SquareBrackets = "[]";
        public static TDestination[] Convert(TSource[] inputs)
        {
            if (inputs == null)
            {
                return default(TDestination[]);
            }
            var values = new List<TDestination>();
            foreach (var input in inputs)
            {
                values.Add(Convert(input));
            }

            return values.ToArray();
        }
        public static Collection<TDestination> Convert(IEnumerable<TSource> inputs)
        {
            if (inputs == null)
            {
                return default(Collection<TDestination>);
            }
            var values = new Collection<TDestination>();
            foreach (var input in inputs)
            {
                values.Add(Convert(input));
            }

            return values;
        }
        public static TDestination Convert(TSource input)
        {
            if (input == null)
            {
                return default(TDestination);
            }
            return SetProperties(input);
        }

        private static TDestination SetProperties(TSource val)
        {

            Type type = typeof(TDestination);
            var obj = (TDestination)Activator.CreateInstance(type);
            PropertyInfo[] propertyInfo = type.GetProperties();

            foreach (var info in propertyInfo)
            {
                PropertyInfo numberPropertyInfo = type.GetProperty(info.Name);
                object propValue;
                try
                {
                    propValue = val.GetType().GetProperty(info.Name).GetValue(val, null);
                }
                catch (Exception)
                {
                    continue;
                }
                if (propValue == null)
                {
                    continue;
                }
                SetValue(numberPropertyInfo, obj, propValue);
            }
            return obj;
        }

        private static void SetValue<TEntity>(PropertyInfo numberPropertyInfo, TEntity obj, object val)
        {
            SetPrimitiveValue(numberPropertyInfo, obj, val);
            if (numberPropertyInfo.PropertyType.BaseType != null && ((numberPropertyInfo.PropertyType.BaseType.Namespace == NamespaceDataClasses && numberPropertyInfo.PropertyType.BaseType.Name != EntityReference) || numberPropertyInfo.PropertyType.Namespace.IndexOf(Namespace, StringComparison.CurrentCulture) != -1) && numberPropertyInfo.PropertyType.BaseType.Name != Array)
            {
                if (!numberPropertyInfo.PropertyType.IsEnum)
                {
                    numberPropertyInfo.SetValue(obj, SetPropertiesByObject(Activator.CreateInstance(numberPropertyInfo.PropertyType), val), null);
                }
            }
            else if ((numberPropertyInfo.PropertyType).Name == ListPropertyName || (numberPropertyInfo.PropertyType).Name == CollectionPropertyName || (numberPropertyInfo.PropertyType).Name.Contains(SquareBrackets))
            {
                Type[] genericDestTypes = numberPropertyInfo.PropertyType.GetGenericArguments();
                Type[] genericSourceTypes = val.GetType().GetGenericArguments();
                if (genericSourceTypes.Length == 0)
                {
                    genericSourceTypes = new Type[1];
                    genericSourceTypes[0] = val.GetType().GetElementType();
                }
                if (genericDestTypes.Length == 0)
                {
                    genericDestTypes = new Type[1];
                    genericDestTypes[0] = numberPropertyInfo.PropertyType.GetElementType();
                }

                if (genericSourceTypes[0] == typeof(TSource))
                {
                    var sourceVal = val as Collection<TSource>;
                    if (sourceVal != null && sourceVal.Count > 0)
                    {
                        var destList = Convert(sourceVal);
                        if (numberPropertyInfo.PropertyType.IsGenericType)
                        {
                            numberPropertyInfo.SetValue(obj, destList, null);
                        }
                        else
                        {
                            numberPropertyInfo.SetValue(obj, destList.ToArray(), null);
                        }
                    }
                    else
                    {
                        var sourceArrayVal = val as TSource[];
                        if (sourceArrayVal != null && sourceArrayVal.Count() > 0)
                        {
                            var destList = Convert(sourceArrayVal);
                            if (numberPropertyInfo.PropertyType.IsGenericType)
                            {

                                numberPropertyInfo.SetValue(obj, new Collection<TDestination>(destList.ToList()), null);
                            }
                            else
                            {
                                numberPropertyInfo.SetValue(obj, destList, null);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                }




            }

        }

        private static void SetPrimitiveValue<TEntity>(PropertyInfo numberPropertyInfo, TEntity obj, object val)
        {
            try
            {
                numberPropertyInfo.SetValue(obj, val, null);
            }
            catch (Exception)
            {


            }


        }

        private static object SetPropertiesByObject(object obj, object val)
        {
            PropertyInfo[] propertyInfo = obj.GetType().GetProperties();
            foreach (var info in propertyInfo)
            {
                PropertyInfo numberPropertyInfo = obj.GetType().GetProperty(info.Name);
                if (numberPropertyInfo.GetSetMethod() == null)
                {
                    continue;
                }
                SetValue(numberPropertyInfo, obj, val);

            }
            return obj;
        }

    }
}
