using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;

namespace MainBit.Projections.ClientSide.Storage
{
    public class TypedStorage : IStorage
    {
        public TypedStorage(Func<string, Type, object> getter, Action<string, Type, object> setter)
        {
            Getter = getter;
            Setter = setter;
        }

        public Func<string, Type, object> Getter { get; set; }
        public Action<string, Type, object> Setter { get; set; }

        public T Get<T>(string name)
        {
            var value = Getter(name, typeof(T));
            if (value == null)
            {
                return default(T);
            }

            var t = typeof(T);

            return (T)value;
            //return (T)Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
        }

        public void Set<T>(string name, T value)
        {
            Setter(name, typeof(T), value);
        }
    }
}