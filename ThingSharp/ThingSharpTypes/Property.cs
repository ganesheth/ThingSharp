using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    public class PropertyBase : Resource
    {
        public PropertyBase(Uri uri)
            : base(uri)
        {

        }
        [JsonIgnore]
        public Thing Parent { get; set; }

        [JsonProperty(PropertyName = "href")]
        public override Uri Uri { get { return Parent.Uri.MakeRelativeUri(mUri); } }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class Property<T> : PropertyBase
    {
        public Property(Uri uri)
            : base(uri)
        {

        }
        public Property(Uri baseUri, String relative)
            : base(new Uri(baseUri.AbsoluteUri + "/" + relative))
        {
            Name = relative;
            ValueType = typeof(T);
        }

        public override object Read()
        {
            return Value;
        }

        public override void Write(object val)
        {
            Value = (T)val;
        }

        public T Value { get; set; }

        [JsonProperty(PropertyName = "units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; }

        [JsonProperty(PropertyName = "min", NullValueHandling = NullValueHandling.Ignore)]
        public T Min { get; set; }

        [JsonProperty(PropertyName = "max", NullValueHandling = NullValueHandling.Ignore)]
        public T Max { get; set; }

        [JsonProperty(PropertyName = "writable", NullValueHandling = NullValueHandling.Ignore)]
        public bool Writable { get; set; }

        private static readonly HashSet<Type> m_numTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

        public static bool IsNumeric(Object o)
        {
            var IsNumeric = false;

            if (o != null)
            {
                IsNumeric = m_numTypes.Contains(o.GetType());
            }

            return IsNumeric;
        }

        public bool ShouldSerializeMin()
        {
            return IsNumeric(this.Value);
        }

        public bool ShouldSerializeMax()
        {
            return IsNumeric(this.Value);
        }
    }
}
