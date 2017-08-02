using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    public abstract class ProtocolBinding
    {
        public abstract void StartListening();
        public abstract void StopListening();

        public abstract void AddClient(IBindingClient client);

    }
}
