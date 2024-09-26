using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GoUtility.Utilities
{
    public static class WMI
    {
        private static ManagementObject? Bridge(string query, string methodName)
        {
            var mos = new ManagementObjectSearcher(
                "root\\WMI",
                $"SELECT * FROM {query}");

            var managementObjects = mos.Get().Cast<ManagementBaseObject>();
            var managementObject = managementObjects.FirstOrDefault();

            if (managementObject == null)
                return null;

            return (ManagementObject)managementObject;
        }
        private static ManagementBaseObject? Get(string query, string methodName, Dictionary<string, object>? methodParams)
        {
            var mo = Bridge(query, methodName);
            if (mo == null) return null;

            var methodParamsObject = mo.GetMethodParameters(methodName);

            if (methodParams != null)
                foreach (var pair in methodParams)
                    methodParamsObject[pair.Key] = pair.Value;

            return mo.InvokeMethod(methodName, methodParamsObject, null);
        }

        private static ManagementBaseObject? Set(string query, string methodName, Dictionary<string, object> methodParams)
        {
            var mo = Bridge(query, methodName);
            if (mo == null) return null;

            var methodParamsObject = mo.GetMethodParameters(methodName);
            foreach (var pair in methodParams)
                methodParamsObject[pair.Key] = pair.Value;

            return mo.InvokeMethod(methodName, methodParamsObject, null);
        }

        public static int GetFeatureValue(int ids)
        {
            var resultProperties = Get("LENOVO_OTHER_METHOD", "GetFeatureValue", new() { { "IDs", ids } });
            if (resultProperties == null) return -1;

            var value = Convert.ToInt32(resultProperties.Properties["Value"].Value);

            return value;
        }

        public static void SetFeatureValue(int ids, int value)
        {
            Set("LENOVO_OTHER_METHOD", "SetFeatureValue", new() { { "IDs", ids }, { "value", value } });
        }

        public static void SetSmartFanMode(int value)
        {
            Set("LENOVO_GAMEZONE_DATA", "SetSmartFanMode", new() { { "Data", value } });
        }

        public static int GetSmartFanMode()
        {
            var resultProperties = Get("LENOVO_GAMEZONE_DATA", "GetSmartFanMode", null);
            if (resultProperties == null) return -1;

            var value = Convert.ToInt32(resultProperties.Properties["Data"].Value);

            return value;
        }

        public static void SetFanTable(FanTable fanTable)
        {
            Set("LENOVO_FAN_METHOD", "Fan_Set_Table", new() { { "FanTable", fanTable.GetBytes() } });
        }
    }
}
