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

        [JsonProperty(PropertyName ="uri")]
        public virtual Uri Uri { get { return mUri; } }

        public virtual Resource ResolveUrl(Uri url)
        {
            if (url == mUri)
                return this;
            else
                return mChildren.FirstOrDefault(p => p.ResolveUrl(url) != null);
        }
        [JsonIgnore]
        public Object SubsystemContext { get; set; }

        
        public Type ValueType { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "valueType", NullValueHandling = NullValueHandling.Ignore)]
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
