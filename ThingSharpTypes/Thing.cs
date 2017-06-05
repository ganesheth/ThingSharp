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

        public Thing(Uri uri) : base(uri)
        {
            Contexts = new List<Context>();
            Contexts.Add(new Context("wot", @"http://wot.org"));
        }

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

        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public List<String> Type { get; set; }

        [JsonProperty(PropertyName = "cc:om_name")]
        public String CC_OMName { get; set; }

        [JsonProperty(PropertyName ="properties")]
        public ResourceCollection Properties { get { return base.mChildren; } }

        [JsonProperty(PropertyName = "context")]
        public List<Context> Contexts { get; set; }

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
