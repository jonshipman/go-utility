using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoUtility.Utilities
{
    public static class TDP
    {
        // @see https://github.com/Valkirie/HandheldCompanion
        // ID enum attribution

        public enum PowerLimit
        {
            Short = 16908032,
            Long = 16973568,
            Peak = 17039104,
        }

        public enum Mode
        {
            Quiet = 1,
            Balanced = 2,
            Performance = 3,
        }

        public static void SetPowerLimit(Mode mode)
        {
            // @see https://github.com/Valkirie/HandheldCompanion
            // Using the default TDP values listed here (may change after confirming with another source)

            int[][] TDPValues = [
                new[] { 8, 8, 8 },
                new[] { 15, 15, 15 },
                new[] { 20, 20, 20 }
            ];

            if (mode == Mode.Quiet)
            {
                WMI.SetFeatureValue((int)PowerLimit.Short, (int)TDPValues[0][0]);
                WMI.SetFeatureValue((int)PowerLimit.Long, (int)TDPValues[0][1]);
                WMI.SetFeatureValue((int)PowerLimit.Peak, (int)TDPValues[0][2]);
            }
            else if (mode == Mode.Balanced)
            {
                WMI.SetFeatureValue((int)PowerLimit.Short, (int)TDPValues[1][0]);
                WMI.SetFeatureValue((int)PowerLimit.Long, (int)TDPValues[1][1]);
                WMI.SetFeatureValue((int)PowerLimit.Peak, (int)TDPValues[1][2]);
            }
            else if (mode == Mode.Performance)
            {
                WMI.SetFeatureValue((int)PowerLimit.Short, (int)TDPValues[2][0]);
                WMI.SetFeatureValue((int)PowerLimit.Long, (int)TDPValues[2][1]);
                WMI.SetFeatureValue((int)PowerLimit.Peak, (int)TDPValues[2][2]);
            }
        }

        public static int GetPowerLimitValue(PowerLimit powerLimit)
        {
            return WMI.GetFeatureValue((int)powerLimit);
        }
    }
}
