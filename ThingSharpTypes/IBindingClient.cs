using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    public interface IBindingClient
    {
        Object Read(Uri uri);

        Object Write(Uri obj, Object value);

    }
}
