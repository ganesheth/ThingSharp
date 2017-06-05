using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;

namespace TestAdapter.DataModels
{
    class GMS_BACNET_EO_BA_AI_1 : Thing
    {
        public GMS_BACNET_EO_BA_AI_1(Uri uri) : base(uri)
        {
            mPresent_Value = new Property<float>(uri, "Present_Value") { Name = "Present Value"};
            AddProperty(mPresent_Value);

            mStatus_Flags = new Property<byte>(uri, "Status_Flags") { Name = "Status Flags"};
            AddProperty(mStatus_Flags);

            mHigh_Limit = new Property<float>(uri, "High_Limit") { Name = "High Limit" };
            AddProperty(mHigh_Limit);

            mLow_Limit = new Property<float>(uri, "Low_Limit") { Name = "Low Limit" };
            AddProperty(mLow_Limit);

            mReliability = new Property<uint>(uri, "Reliability") { Name= "Reliability" };
            AddProperty(mReliability);

            mOut_Of_Service = new Property<bool>(uri, "Out_Of_Service") { Name = "Out Of Service" };
            AddProperty(mOut_Of_Service);

        }

        Property<float> mPresent_Value;
        public Property<float> Present_Value { get { return mPresent_Value; } }

        Property<float> mHigh_Limit;
        public Property<float> High_Limit { get { return mHigh_Limit; } }

        Property<float> mLow_Limit;
        public Property<float> Low_Limit { get { return mLow_Limit; } }

        Property<uint> mReliability;
        public Property<uint> Reliability { get { return mReliability; } }

        Property<byte> mStatus_Flags;
        public Property<byte> Status { get { return mStatus_Flags; } }

        Property<bool> mOut_Of_Service;
        public Property<bool> Out_Of_Service { get { return mOut_Of_Service; } }
    }
}
