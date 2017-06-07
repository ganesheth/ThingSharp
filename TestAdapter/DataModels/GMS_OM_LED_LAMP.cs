using System;
using ThingSharp.Types;

namespace ThingSharp.TestAdapter
{
    class GMS_OM_LED_LAMP : Thing
    {
        public GMS_OM_LED_LAMP(Uri uri) : base(uri)
        {
            CC_OMName = "GMS_OM_LED_LAMP";
            
            mBrightness = new Property<float>(uri, "Brightness") { Name = "Light Brightness", Units = "Lux", Writable = true, Min = 0, Max = 100 };
            AddProperty(mBrightness);

            mLightPower = new Property<bool>(uri, "Switch") { Name = "Light Switch", Writable = true };
            AddProperty(mLightPower);

            mColor = new Property<string>(uri, "Color") { Name = "Light Color in html RGB", Writable = true };
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
