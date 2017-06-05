using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    public class Adapter
    {
        public delegate void OnThingAdded(Thing thing);
        public event OnThingAdded ThingAdded;

        public virtual void Initialize(Uri baseUri) { }
        public virtual Object Read(Resource obj) { return null; }
        public virtual Object Write(Resource obj, Object value) { return null; }

        protected void RaiseOnThingAdded(Thing thing)
        {
            if (ThingAdded != null)
                ThingAdded(thing);
        }
    }
}
