using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;

namespace ThingSharp.Container
{
    public class ThingContainer : List<Thing>
    {
        //private List<Thing> mThings = new List<Thing>();
        private Dictionary<Uri, Resource> mResolutionCache = new Dictionary<Uri, Resource>();
        public Resource ResolveUrl(Uri url)
        {
            if (mResolutionCache.ContainsKey(url))
                return mResolutionCache[url];
            else
            {
                Resource r = this.FirstOrDefault(t => t.ResolveUrl(url) != null);
                if (r != null)
                {
                    if (r is Thing)
                        r = r.ResolveUrl(url);
                    if (r != null)
                        mResolutionCache.Add(url, r);
                }

                return r;
            }
        }

        public void AddThing(Thing thing)
        {
            this.Add(thing);
        }
    }
}
