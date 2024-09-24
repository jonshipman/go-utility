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
            var value = WMI.GetFeatureValue((int)IDS.FanFullSpeed);

            if (value == 1)
            {
                return true;
            }
            else if (value == 0)
            {
                return false;
            }

            return false;
        }

        public static void SetFanFullSpeed(bool state)
        {
            if (state)
            {
                WMI.SetFeatureValue((int)IDS.FanFullSpeed, 1);
            }
            else
            {
                WMI.SetFeatureValue((int)IDS.FanFullSpeed, 0);
            }
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

            return null;
        }
    }
}
