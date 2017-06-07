using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Thing : Resource
    {
        public class DefaultContext
        {
            public String wot = "https://w3c.github.io/wot/w3c-wot-td-ontology.owl#";
            public string cc = "http://siemens.com/cc";
        }

        public Thing(Uri uri) : base(uri)
        {
            //Contexts = new List<Context>();
            //Contexts.Add(new Context("wot", @"http://wot.org"));
            context = new DefaultContext();
        }

        [JsonProperty(PropertyName = "context", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        protected virtual DefaultContext context { get; set; }
        //public void AddProperty(Property property) {
            
        //    mChildren.Add(property);
            
        //}

        public override object Read()
        {
            return "You will recieve a Thing Description";
        }

        public void AddProperty(PropertyBase property)
        {
            property.Parent = this;
            Properties.Add(property);
        }

        public void RemoveProperty(string name)
        {
            Resource r = mChildren.First(x => x.Name == name);
            if (r != null)
                mChildren.Remove(r);
        }

        public List<Context> Contexts { get; set; }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore, Required = Required.DisallowNull, Order = 5)]
        public List<String> Type { get; set; }

        [JsonProperty(PropertyName = "id", Order = 6)]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "cc:om_name", NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public String CC_OMName { get; set; }

        [JsonProperty(PropertyName ="properties", Order = 8)]
        public ResourceCollection Properties { get { return base.mChildren; } }



        public class Context
        {
            public Context(string key, string uri)
            {
                this.key = key;
                this.uri = uri;
            }
            public string key { get; set; }
            public string uri { get; set; }
        }
    }
}
