using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;

namespace GoUtility.Utilities
{
    public static class Fan
    {
        // @see https://github.com/Valkirie/HandheldCompanion
        // ID enum attribution
        private enum IDS
        {
            FanFullSpeed = 67239936,
        }

        public static bool IsFanAtFullSpeed()
        {
            return WMI.GetFeatureValue((int)IDS.FanFullSpeed) == 1;
        }

        public static void SetFanFullSpeed(bool state)
        {
             WMI.SetFeatureValue((int)IDS.FanFullSpeed, state ? 1 : 0);
        }

        public static void SetSmartFanMode(TDP.Mode mode)
        {
            WMI.SetSmartFanMode((int)mode);
        }

        public static TDP.Mode? GetSmartFanMode() {
            var value = WMI.GetSmartFanMode();

            if (value == (int)TDP.Mode.Quiet) return TDP.Mode.Quiet;
            if (value == (int)TDP.Mode.Balanced) return TDP.Mode.Balanced;
            if (value == (int)TDP.Mode.Performance) return TDP.Mode.Performance;
            if (value == (int)TDP.Mode.Custom) return TDP.Mode.Custom;

            return null;
        }

        public static void SetFanTableForCustomMode() {
            FanTable fanTable = new(new ushort[] { 44, 56, 65, 75, 81, 87, 100, 100, 100, 100 });
            WMI.SetFanTable(fanTable);
        }
    }
}
