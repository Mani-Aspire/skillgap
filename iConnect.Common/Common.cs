using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace iConnect.Common
{
    public static class CommonUtility
    {
        /// <summary>
        /// To sort the entity in specified format.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Sort<TEntity>(IQueryable<TEntity> source, string sortExpression)
        {
            if (source == null || string.IsNullOrEmpty(sortExpression))
            {
                return source;
            }
            Type type = typeof(TEntity);
            // Remember that for ascending order GridView just returns the column name and for descending it returns column name followed by DESC keyword  
            // Therefore we need to examine the sortExpression and separate out Column Name and order (ASC/DESC)  
            string[] expressionParts = sortExpression.Split(' ');
            // Assuming sortExpression is like [ColoumnName DESC] or [ColumnName] 
            string orderByProperty = expressionParts[0];
            string sortDirection;
            string methodName = "OrderBy";
            //if sortDirection is descending  
            if (expressionParts.Length > 1 && expressionParts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase))
            {
                sortDirection = "Descending";
                methodName += sortDirection;
                // Add sort direction at the end of Method name 
            }
            String[] orderByChildProperties = orderByProperty.Split('.');
            ParameterExpression parameter = Expression.Parameter(type, "p");
            PropertyInfo property = typeof(TEntity).GetProperty(orderByChildProperties[0]);
            MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, property);
            for (int i = 1; i < orderByChildProperties.Length; i++)
            {
                Type t = property.PropertyType;
                if (!t.IsGenericType)
                {
                    property = t.GetProperty(orderByChildProperties[i]);
                }
                else
                {
                    property = t.GetGenericArguments().First().GetProperty(orderByChildProperties[i]);
                }

                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            LambdaExpression orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                                                             new[] { type, property.PropertyType },
                                                             source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<TEntity>(resultExp);
        }
    }

    public static class UtilityCommon
    {
       

        //To support globalization rules. 
        public static IFormatProvider GetFormatProvider
        {
            get
            {
                const IFormatProvider iFormatProvider = null;
                return iFormatProvider;
            }
        }

        /// <summary>
        ///  Method to find whether the input string contains an integer value
        /// </summary>
        /// <param name="theValue"></param>
        /// <returns></returns>
        public static bool IsInteger(string theValue)
        {
            var _isNumber = new Regex(@"^\d+$");
            Match m = _isNumber.Match(theValue);
            return m.Success;
        }

        

        /// <summary>
        /// 
        /// </summary>
        public static CultureInfo GetCultureInfo
        {
            get { return CultureInfo.CurrentCulture; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static StringComparison GetStringComparisonCultureInfo
        {
            get { return StringComparison.CurrentCulture; }
        }

        /// <summary>
        /// Extension method for support add range in collection
        /// </summary>
        /// <typeparam name="T">The type on which the extension is applied</typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> values)
        {
            if (values == null || collection == null)
            {
                return;
            }
            foreach (T item in values)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// To get the property value from a object.
        /// </summary>
        /// <param name="name">name of the property</param>
        /// <param name="currentItem">current object</param>
        /// <returns>A string value</returns>
        public static Object GetPropValue(String name, Object currentItem)
        {
            if(name == null)
            {
                return null;
            }
            var parts = name.Split('.');
            for(int index=0;index<parts.Length;index++)
            {
                if (currentItem == null)
                {
                    return null;
                }

                Type type = currentItem.GetType();
                PropertyInfo info = type.GetProperty(parts[index]);
                if (info == null)
                {
                    return null;
                }
                if (type == typeof(string))
                {
                    return currentItem;
                }

                currentItem = GetCurrentItem(name, index, parts, currentItem, type, info);
            }
            return currentItem;
        }

        private static object GetCurrentItem(string name, int index, string[] parts, object currentItem, Type type, PropertyInfo info)
        {
            currentItem = info.GetValue(currentItem, null);

            //Code to remove Time in DateTime column
            if (type.GetProperty(parts[index]).PropertyType == typeof(DateTime?))
            {
                currentItem = ((DateTime?)(currentItem)).HasValue ? ((DateTime?)(currentItem)).Value.ToShortDateString() : string.Empty;
            }
            else if (type.GetProperty(parts[index]).PropertyType == typeof(DateTime))
            {
                currentItem = ((DateTime)(currentItem)).ToShortDateString();
            }
            else if((info.PropertyType).BaseType.IsGenericType)
            {
                // iterate through the collection and get the Corresponding column Name
                var items = currentItem as System.Collections.ICollection;
                string value = string.Empty;
                foreach (var item in items)
                {
                    var tempItem = GetPropValue(name.Replace(parts[index] + ".", ""), item);
                    value = (tempItem == null ? string.Empty : (tempItem + ",")) + value;
                      
                }
                if(!string.IsNullOrEmpty(value))
                {
                    value = value.Substring(0, value.Length - 1);
                }
                currentItem = value;
                    
            }
            return currentItem;
        }

        /// <summary>
        /// Returns the value of WildCard string
        /// </summary>
        /// <param name="input">name of input string</param>
        /// <returns>Wild card Sql Value</returns>

        public static string GetSqlWildcard(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }
            return input.Replace('*', '%');
        }

        /// <summary>
        /// This method is used to convert given string value to boolean value.
        /// </summary>
        /// <param name="value">Given string value</param>
        /// <returns>true or false</returns>
        public static bool ToBoolean(string value)
        {
            return ToBoolean((object)value);
        }

        /// <summary>
        /// This method is used to convert given object to boolean value.
        /// </summary>
        /// <param name="value">given object</param>
        /// <returns>true or false</returns>
        public static bool ToBoolean(object value)
        {
            try
            {
                return Convert.ToBoolean(value,GetCultureInfo);
            }
            catch(InvalidCastException)
            {
                value = value ?? string.Empty;
                string strValue = value.ToString();

                if (string.Compare(strValue, "1", true, GetCultureInfo) == 0 || string.Compare(strValue, "true", true, GetCultureInfo) == 0 || string.Compare(strValue, "t", true, GetCultureInfo) == 0 || string.Compare(strValue, "y", true, GetCultureInfo) == 0 || string.Compare(strValue, "on", true, GetCultureInfo) == 0 || string.Compare(strValue, "yes", true, GetCultureInfo) == 0 || string.Compare(strValue, "ok", true, GetCultureInfo) == 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// A common method  added to convert string to Int32 wherever required.
        /// </summary>
        /// <param name="value">String to be converted to int value</param>
        /// <returns>Appropriate int value is input is in proper format</returns>
        public static int ConvertToInt32(string value)
        {
            int result=0;
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    result = Int32.Parse(value, GetCultureInfo);
                }
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;
        }

        /// <summary>
        /// A common method  added to convert string to Int16 wherever required.
        /// </summary>
        /// <param name="value">String to be converted to short value</param>
        /// <returns>Appropriate short value is input is in proper format</returns>
        public static short ConvertToInt16(string value)
        {
            short result;
            try
            {
                result = Int16.Parse(value, GetCultureInfo);
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;
        }

        /// <summary>
        /// A common method  added to convert string to Int64 wherever required.
        /// </summary>
        /// <param name="value">String to be converted to long value</param>
        /// <returns>Appropriate long value is input is in proper format</returns>
        public static long ConvertToInt64(string value)
        {
            long result;
            try
            {
                result = Int64.Parse(value, GetCultureInfo);
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;
        }

        /// <summary>
        /// A common method  added to convert string to double wherever required.
        /// </summary>
        /// <param name="value">String to be converted to double value</param>
        /// <returns>Appropriate double value is input is in proper format</returns>
        public static double ConvertToDouble(string value)
        {
            double result;
            try
            {
                result = Double.Parse(value, GetCultureInfo);
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;

        }

        /// <summary>
        /// A common method  added to convert string to DateTime type wherever required.
        /// </summary>
        /// <param name="value">string to be converted to bool</param>
        /// <returns>true or false if string is a proper source, else exception will be thrown.</returns>
        public static bool ConvertToBoolean(string value)
        {
            bool result;
            try
            {
                result = Boolean.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;
        }

        /// <summary>
        /// A common method  added to convert string to datetime wherever required.
        /// </summary>
        /// <param name="value">String to be converted to datetime value</param>
        /// <returns>Appropriate datetime value is input is in proper format</returns>
        public static DateTime ConvertToDateTime(string value)
        {
            DateTime result;
            try
            {
                result = DateTime.Parse(value,GetCultureInfo);
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to format string :" + value);
            }
            return result;
        }

        /// <summary>
        /// A common method  added to convert string to Guid wherever required.
        /// </summary>
        /// <param name="value">string value to be converted to Guid</param>
        /// <returns>new guid if the string is in proper format.</returns>
        public static Guid ConvertToGuid(string value)
        {
            var id=new Guid();
            try
            {
                if (!String.IsNullOrEmpty(value) && value != Guid.Empty.ToString())
                {
                    id = new Guid(value);
                }
            }
            catch (FormatException exception)
            {
                throw new FormatException(exception.Message + "Unable to create Guid :" + value);
            }
            return id;
        }

       
        /// <summary>
        /// This method is used to get trimmed string. 
        /// </summary>
        /// <param name="selectedValues">Concatenated string</param>
        /// <returns>Trimmed string</returns>
        public static string TrimCommaString(string selectedValues)
        {
            selectedValues = selectedValues ?? string.Empty;
            if (selectedValues.EndsWith(",", StringComparison.CurrentCulture))
            {
                selectedValues = selectedValues.Substring(0, selectedValues.Length - 1);
            }
            return selectedValues;
        }

        
        /// <summary>
        /// This method is used to get Collection of string.
        /// </summary>
        /// <param name="selectedValues">Concatenated string</param>
        /// <returns>Collection of string</returns>
        public static Collection<string> GetStringCollection(string selectedValues)
        {
            var stringList = new Collection<string>();
            string values = selectedValues != null ? selectedValues.Trim() : selectedValues;
            if (!String.IsNullOrEmpty(values))
            {
                string[] array = TrimCommaString(selectedValues).Split(',');
                for (int count = 0; count < array.Length; count++)
                {
                    string[] centreArrayValue = TrimCommaString(array[count]).Split('~');
                    stringList.Add(centreArrayValue[0].Trim());
                }
            }
            return stringList;
        }

        public static void LogInnerException(Exception ex)
        {
            if (ex != null)
            {
                Exception innerException = ex.InnerException;
                LogInnerException(innerException);
            }
            return;
        }
        

        
    }
}
