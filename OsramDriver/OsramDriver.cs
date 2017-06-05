using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThingSharp.Osram
{
    public class OsramDriver
    {
        private string mHostname = "";
        public OsramDriver(string gatewayHostName)
        {
            mHostname = gatewayHostName;
        }
        public List<OsramBulb> QueryBulbs()
        {
            byte[] response = Network.SendReceive(mHostname, QueryBulbsCommand.GetRequestData());
            List<OsramBulb> bulbs = QueryBulbsCommand.ProcessResponse(response);
            return bulbs;
        }

        public void GetBulbStatus(ref OsramBulb bulbToUpdate)
        {
            byte[] response = Network.SendReceive(mHostname, QueryBulbStatusCommand.GetRequestData(bulbToUpdate.Address));
            QueryBulbStatusCommand.ProcessResponse(response, ref bulbToUpdate);
        }

        public bool SetBulbPowerState(OsramBulb bulbToUpdate, byte status)
        {
            byte[] response = Network.SendReceive(mHostname, UpdateBulbPowerStateCommand.GetRequestData(bulbToUpdate.Address, status));
            bool writeSuccess = UpdateBulbPowerStateCommand.ProcessResponse(response, bulbToUpdate);
            return writeSuccess;
        }

        public bool SetBulbBrightness(OsramBulb bulbToUpdate, byte brightness, byte time)
        {
            byte[] response = Network.SendReceive(mHostname, UpdateBulbBrightnessCommand.GetRequestData(bulbToUpdate.Address, brightness, time));
            bool writeSuccess = UpdateBulbBrightnessCommand.ProcessResponse(response, bulbToUpdate);
            return writeSuccess;
        }

        public bool SetBulbColorTemperature(OsramBulb bulbToUpdate, ushort temperature, byte time)
        {
            byte[] response = Network.SendReceive(mHostname, UpdateBulbColorTemperatureCommand.GetRequestData(bulbToUpdate.Address, temperature, time));
            bool writeSuccess = UpdateBulbColorTemperatureCommand.ProcessResponse(response, bulbToUpdate);
            return writeSuccess;
        }

        public bool SetBulbColor(OsramBulb bulbToUpdate, string colorName, byte time)
        {
            Color myColor = ColorTranslator.FromHtml(colorName);
            SetBulbBrightness(bulbToUpdate, myColor.A, time);
            return SetBulbColor(bulbToUpdate, myColor.R, myColor.G, myColor.B, time);
        }
        public bool SetBulbColor(OsramBulb bulbToUpdate, byte red, byte green, byte blue, byte time)
        {
            byte[] response = Network.SendReceive(mHostname, UpdateBulbColorCommand.GetRequestData(bulbToUpdate.Address, red, green, blue, time));
            bool writeSuccess = UpdateBulbColorCommand.ProcessResponse(response, bulbToUpdate);
            return writeSuccess;
        }

        public bool SetBulbColorCycling(OsramBulb bulbToUpdate, bool enable, byte time)
        {
            byte[] response = Network.SendReceive(mHostname, CycleColorCommand.GetRequestData(bulbToUpdate.Address, enable, time));
            bool writeSuccess = CycleColorCommand.ProcessResponse(response, bulbToUpdate);
            return writeSuccess;
        }

    }

    /*
     * from http://sarajarvi.org/lightify-haltuun/en.php
    Command:
    0B 00 00 13 00 00 00 00 01 00 00 00 00

    Response:
    00 01 02 03 04 05 06 07 08 09 10
    ---------------------------------
    33 00 01 13 00 00 00 00 00 01 00
     
    (data per bulb starts)
    11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 44 45 46 47 48 49 50 51 52 53 54 55 56 57 58 59 60 61
    -----------------------------------------------------------------------------------------------------------------------------------------------------  
    E3 F3 hh hh hh hh hh hh hh hh 0A 01 02 03 01 02 01 00 tt bb ll ll rr gg bb FF 4F 6C 6F 68 75 6F 6E 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    (data per bulb ends)
    --
    00
    General: data length, bulb MAC backwards, command.
    As far as I know, after 13 byte header comes information in 42 or 50 byte blocks per light.
    Status (tt): 00 = off and 01 = on.
    Brightness (bb): 1-100.
    Color temperature (ll ll): Kelvin value (2200-6500), bytes backwards. For example, 8C 0A = 0A8C = 2700 K.
    Color (rr gg bb): red, green and blue values between 0-255.
    Name, presumably 16 bytes (some gateways return 8 bytes longer names) and "Olohuone" in this example.
    Status for a single light bulb
    */

    public class QueryBulbsCommand
    {
        private static byte[] mRequestData = new byte[] { 0x0B, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
        public static byte[] GetRequestData() { return mRequestData; }

        public static List<OsramBulb> ProcessResponse(byte[] data)
        {
            List<OsramBulb> bulbs = new List<OsramBulb>();
            int length = data[0] + (data[1] << 8);
            OsramBulb bulb = ParseFromData(data, 0);     //TODO: Handle multiple bulbs       
            bulbs.Add(bulb);
            return bulbs;
        }

        public static OsramBulb ParseFromData(byte[] bulbData, int offset)
        {
            OsramBulb bulb = new OsramBulb();
            byte[] mac = new byte[8];
            Array.Copy(bulbData, 13 + offset, mac, 0, 8);
            bulb.Address = mac;
            bulb.Status = bulbData[29 + offset] == 0x01 ? true : false;
            bulb.Brightness = (int)bulbData[30 + offset];
            bulb.ColorTemperature = (int)(bulbData[31 + offset] + bulbData[32 + offset] << 8);
            byte red = bulbData[33 + offset];
            byte green = bulbData[34 + offset];
            byte blue = bulbData[35 + offset];
            bulb.LightColor = Color.FromArgb(red, green, blue);
            return bulb;
        }
    }


    /*
        Command:
        0E 00 00 68 00 00 00 00 hh hh hh hh hh hh hh hh
        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
        --------------------------------------------------------------------------------------
        1B 00 01 68 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00 02 tt bb ll ll rr gg bb FF     
     */
    public class QueryBulbStatusCommand
    {
        public static byte[] GetRequestData(byte[] macId)
        {
            byte[] requestData = new byte[] { 0x0E, 0x00, 0x00, 0x68, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff};
            Array.Copy(macId, 0, requestData, 8, 8);
            return requestData;
        }

        public static void ProcessResponse(byte[] data, ref OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            bulbToUpdate.Address = mac;
            bulbToUpdate.Status = data[21] == 0x01 ? true : false;
            bulbToUpdate.Brightness = (int)data[22];
            bulbToUpdate.ColorTemperature = (int)(data[23] + data[24] << 8);
            byte red = data[25];
            byte green = data[26];
            byte blue = data[27];
            bulbToUpdate.LightColor = Color.FromArgb(red, green, blue);
        }
    }

    /*
        Command:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        0F 00 00 32 00 00 00 00 hh hh hh hh hh hh hh hh tt

        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 01 32 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00
        General: data length, bulb MAC backwards, command.
        Status (tt): 00 = off and 01 = on.
        Transition time (ss): 1/10th of a seconds
        Bulb brightness
     
     */
    class UpdateBulbPowerStateCommand
    {
        public static byte[] GetRequestData(byte[] macId, byte status)
        {
            byte[] requestData = new byte[] { 0x0f, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00};
            Array.Copy(macId, 0, requestData, 8, 8);
            requestData[16] = status;
            return requestData;
        }

        public static bool ProcessResponse(byte[] data, OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            if (Array.Equals(bulbToUpdate.Address, mac))
                return true;
            else
                return false;
        }
    }

    /*
        Command:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        11 00 00 31 00 00 00 00 hh hh hh hh hh hh hh hh bb ss 00
        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 01 31 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00
        General: data length, bulb MAC backwards, command.
        Brightness (bb): 1-100, zero turns off the light.
        Transition time (ss): 1/10th of a seconds
        Bulb color temperature     
    */
    class UpdateBulbBrightnessCommand
    {
        public static byte[] GetRequestData(byte[] macId, byte brightness, byte time)
        {
            byte[] requestData = new byte[] { 0x11, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xbb, 0xaa, 0x00 };
            Array.Copy(macId, 0, requestData, 8, 8);
            requestData[16] = brightness;
            requestData[17] = time;
            return requestData;
        }

        public static bool ProcessResponse(byte[] data, OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            if (Array.Equals(bulbToUpdate.Address, mac))
                return true;
            else
                return false;
        }
    }

    /*
        Command:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 00 33 00 00 00 00 hh hh hh hh hh hh hh hh ll ll ss 00
        
        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 01 33 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00

        General: data length, bulb MAC backwards, command.
        Color temperature (ll ll): Kelvin value (2200-6500), bytes backwards. For example, 8C 0A = 0A8C = 2700 K.
        Transition time (ss): 1/10th of a seconds
        Bulb RGB color
     
     */
    class UpdateBulbColorTemperatureCommand
    {
        public static byte[] GetRequestData(byte[] macId, ushort temperature, byte time)
        {
            byte[] requestData = new byte[] { 0x12, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xbb, 0xbb, 0xaa, 0x00 };
            Array.Copy(macId, 0, requestData, 8, 8);
            requestData[16] = (byte)(temperature & 0x00ff);
            requestData[17] = (byte)(temperature >> 8);
            requestData[18] = time;
            return requestData;
        }

        public static bool ProcessResponse(byte[] data, OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            if (Array.Equals(bulbToUpdate.Address, mac))
                return true;
            else
                return false;
        }

    }

    /*
        Command:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        14 00 00 36 00 00 00 00 hh hh hh hh hh hh hh hh rr gg bb FF ss 00

        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 01 36 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00

        General: data length, bulb MAC backwards, command.
        Color(rr gg bb): red, green and blue values between 0-255.
        Transition time(ss): 1/10th of a seconds
    */

    class UpdateBulbColorCommand
    {
        public static byte[] GetRequestData(byte[] macId, byte red, byte green, byte blue, byte time)
        {
            byte[] requestData = new byte[] { 0x14, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xaa, 0xbb, 0xcc, 0xFF, 0xaa, 0x00 };
            Array.Copy(macId, 0, requestData, 8, 8);
            requestData[16] = red;
            requestData[17] = green;
            requestData[18] = blue;
            requestData[20] = time;
            return requestData;
        }

        public static bool ProcessResponse(byte[] data, OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            if (Array.Equals(bulbToUpdate.Address, mac))
                return true;
            else
                return false;
        }
    }

    /*
        Command:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        11 00 00 D5 00 00 00 00 hh hh hh hh hh hh hh hh ss tt tt

        Response:
        00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19
        -----------------------------------------------------------
        12 00 01 D5 00 00 00 00 00 01 00 hh hh hh hh hh hh hh hh 00
        General: data length, bulb MAC backwards, command.
        Cycling status(ss): 00 = off(set speed to zero) ja 01 = on.
        Speed as seconds(tt tt): range 5 - 65535 seconds, bytes backwards.For example, 1C 02 = 021C = 900 seconds.
    */
    class CycleColorCommand
    {
        public static byte[] GetRequestData(byte[] macId, bool enable, ushort time)
        {
            byte[] requestData = new byte[] { 0x11, 0x00, 0x00, 0xD5, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xaa, 0xbb, 0xbb};
            Array.Copy(macId, 0, requestData, 8, 8);
            requestData[16] = enable?(byte)0x01 : (byte)0x00;
            requestData[17] = (byte)(time & 0x00ff);
            requestData[18] = (byte)(time >> 8);
            return requestData;
        }

        public static bool ProcessResponse(byte[] data, OsramBulb bulbToUpdate)
        {
            byte[] mac = new byte[8];
            Array.Copy(data, 11, mac, 0, 8);
            if (Array.Equals(bulbToUpdate.Address, mac))
                return true;
            else
                return false;
        }
    }


    public class OsramBulb
    {
        public string Name { get; set; }
        public int Brightness { get; set; }

        public int ColorTemperature { get; set; }

        public System.Drawing.Color LightColor { get; set; }

        public string LightColorAsString
        {
            get
            {
                if (LightColor.IsNamedColor)
                    return LightColor.Name;
                else
                    return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", LightColor.A, LightColor.R, LightColor.G, LightColor.B);
            }
        }

        public byte[] Address { get; set; }

        public bool Status { get; set; }

        static string byteArrayToMac(byte[] mac)
        {
            return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}:{6:X2}:{7:X2}", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5], mac[6], mac[7]);
        }
    }
}
