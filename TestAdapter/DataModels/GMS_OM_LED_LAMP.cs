using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;

namespace ThingSharp.TestAdapter
{
    class GMS_OM_LED_LAMP : Thing
    {
        public GMS_OM_LED_LAMP(Uri uri) : base(uri)
        {
            CC_OMName = "GMS_OM_LED_LAMP";
            
            mBrightness = new Property<float>(uri, "Brightness") { Name = "Brightness", Units = "Lux" };
            AddProperty(mBrightness);

            mLightPower = new Property<bool>(uri, "Switch") { Name = "Switch"};
            AddProperty(mLightPower);

            mColor = new Property<string>(uri, "Color") { Name = "Color" };
            AddProperty(mColor);
        }

        Property<float> mBrightness;
        public Property<float> Brightness { get { return mBrightness; } }

        Property<string> mColor;
        public Property<string> Color { get { return mColor; } }

        Property<bool> mLightPower;
        public Property<bool> LightPower { get { return mLightPower; } }
    }
}
