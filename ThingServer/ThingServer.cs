using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Container;
using ThingSharp.Types;

namespace ThingSharp.Server
{
    public class ThingServer : IBindingClient
    {
        private ProtocolBinding mBinding;
        private ThingContainer mThings;
        private Adapter mAdapter;
        public ThingServer(ProtocolBinding binding, Adapter adapter)
        {
            mBinding = binding;
            mBinding.AddClient(this);
            mThings = new ThingContainer();
            mAdapter = adapter;
            mAdapter.ThingAdded += (t) => { this.ResourceContainer.Add(t); };
        }

        public void Start()
        {
            mBinding.StartListening();
        }

        public void Stop()
        {
            mBinding.StopListening();
        }

        public object Read(Uri uri)
        {
            if (uri.AbsolutePath == "/")
            {
                List<HypermediaLink> thingUris = new List<HypermediaLink>();
                foreach(Resource r in mThings)
                {
                    thingUris.Add(new HypermediaLink() { uri = r.Uri, rel = "td"});
                }
                return thingUris;
            }
            Resource resource = mThings.ResolveUrl(uri);

            if (resource != null)
                return mAdapter.Read(resource);
            else
                throw new Exception("Resource not found") ;
        }


        public object Write(Uri uri, object value)
        {
            Resource resource = mThings.ResolveUrl(uri);

            if (resource != null)
                return mAdapter.Write(resource,value);
            else
                throw new Exception("Resource not found");
        }

        public ThingContainer ResourceContainer {
            get { return mThings; }
        }
    }
}
