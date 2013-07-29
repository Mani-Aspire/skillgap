using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace iConnect.Common
{
    public class NamedValue<T>
    {
        public T Value
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }


    }
    [Serializable]
    public sealed class StringCollection : Collection<String>
    {
        
    }
    [Serializable]
    public sealed class NamedDictionary : Dictionary<string, StringCollection>
    {
        public NamedDictionary() : base()
        {
            
        }

        private NamedDictionary(SerializationInfo 
                   info, StreamingContext context) : base(info,context)
        {
            
        }
        //public Collection<StringCollection> Value
        //{
        //    get;
        //    set;
        //}
        //public string Name
        //{
        //    get;
        //    set;
        //}
        
    }

}
