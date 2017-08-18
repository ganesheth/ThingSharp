using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Types
{
    public class ResourceCollection : List<Resource>
    {
        public Resource this[string name]
        {
            get
            {
                int index = 0;
                while (index < base.Count)
                {
                    if (this.ElementAt(index).Name == name)
                    {
                        return this.ElementAt(index);
                    }
                    index++;
                }
                return null;
            }
        }

    }
}
