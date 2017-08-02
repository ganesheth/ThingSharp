using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ThingSharp.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Resource
    {
        [JsonObject(MemberSerialization.OptIn)]
        public class ResourceNotFoundException : Exception
        {
            [JsonProperty(PropertyName = "message")]
            public new String Message { get; set; }
        };

        public class ResourceOperationNotAllowedException : Exception { };
        public class ResourceOperationFailedException : Exception { };

        protected Uri mUri;

        protected ResourceCollection mChildren = new ResourceCollection();
        public Resource(Uri uri)
        {
            this.mUri = uri;
        }

        public virtual object Read()
        {
            return DateTime.Now;
        }

        public virtual void Write(Object val)
        {

        }

        [JsonProperty(PropertyName = "uri", Order = 2)]
        public virtual Uri Uri { get { return mUri; } }

        public virtual Resource ResolveUrl(Uri url)
        {
            if (url.Equals(mUri))
                return this;
            else
                return mChildren.FirstOrDefault(p => p.ResolveUrl(url) != null);
        }
        [JsonIgnore]
        public Object SubsystemContext { get; set; }


        public Type ValueType { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "valueType", NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public object XsdType
        {
            get
            {
                if (ValueType == null)
                    return null;
                XmlReflectionImporter importer = new XmlReflectionImporter();
                XmlTypeMapping map = importer.ImportTypeMapping(ValueType);
                return "xsd:" + map.XsdTypeName;
            }
        }
    }
}
