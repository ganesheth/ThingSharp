using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;

namespace ThingSharp.TestAdapter
{
    class GMS_OM_OSRAM_LIGHTIFY : Thing
    {
        public GMS_OM_OSRAM_LIGHTIFY(Uri uri) : base(uri)
        {
            mBrightness = new Property<float>(uri, "Brightness") { Name = "Brightness", Units = "Lux" };
            AddProperty(mBrightness);

            mStatus = new Property<bool>(uri, "Status") { Name = "Status", Units = "Lux" };
            AddProperty(mStatus);
        }

        Property<float> mBrightness;
        public Property<float> Brightness { get { return mBrightness; } }

        Property<bool> mStatus;
        public Property<bool> Status { get { return mStatus; } }
    }
}
