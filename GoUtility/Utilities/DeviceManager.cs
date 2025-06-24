using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GoUtility.Utilities
{
    public static class DeviceManager
    {
        private static readonly string[] TargetHardwareIds = new[]
        {
        "VID_17EF&PID_6182",
        "VID_17EF&PID_6184",
        "VID_17EF&PID_6185"
    };

        public static void DisableDevices()
        {
            SetDevicesEnabled(false);
        }

        public static void EnableDevices()
        {
            SetDevicesEnabled(true);
        }

        private static void SetDevicesEnabled(bool enable)
        {
            Guid classGuid = Guid.Empty;
            IntPtr deviceInfoSet = SetupDiGetClassDevs(IntPtr.Zero, "USB", IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);

            if (deviceInfoSet == IntPtr.Zero || deviceInfoSet == INVALID_HANDLE_VALUE)
                throw new Exception("Failed to get device info set");

            try
            {
                SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
                devInfoData.cbSize = Marshal.SizeOf(devInfoData);

                for (uint i = 0; SetupDiEnumDeviceInfo(deviceInfoSet, i, ref devInfoData); i++)
                {
                    string? hardwareId = GetDeviceProperty(deviceInfoSet, ref devInfoData, SPDRP_HARDWAREID);
                    if (string.IsNullOrWhiteSpace(hardwareId))
                        continue;

                    foreach (string targetId in TargetHardwareIds)
                    {
                        if (hardwareId.IndexOf(targetId, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            ChangeDeviceState(deviceInfoSet, ref devInfoData, enable);
                            break;
                        }
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }
        }

        private static void ChangeDeviceState(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfoData, bool enable)
        {
            SP_PROPCHANGE_PARAMS propChangeParams = new SP_PROPCHANGE_PARAMS();
            propChangeParams.classInstallHeader.cbSize = Marshal.SizeOf(typeof(SP_CLASSINSTALL_HEADER));
            propChangeParams.classInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
            propChangeParams.StateChange = enable ? DICS_ENABLE : DICS_DISABLE;
            propChangeParams.Scope = DICS_FLAG_GLOBAL;
            propChangeParams.HwProfile = 0;

            if (!SetupDiSetClassInstallParams(deviceInfoSet, ref devInfoData,
                ref propChangeParams, Marshal.SizeOf(propChangeParams)))
            {
                int errorCode = Marshal.GetLastWin32Error();
                System.Diagnostics.Debug.WriteLine($"Warning: \"Failed to set class install params\". Error code: {errorCode}");
                return;
            }

            if (!SetupDiCallClassInstaller(DIF_PROPERTYCHANGE, deviceInfoSet, ref devInfoData))
            {
                int errorCode = Marshal.GetLastWin32Error();
                System.Diagnostics.Debug.WriteLine($"Warning: Could not change device state. Error code: {errorCode}");
                return;
            }
        }

        private static string? GetDeviceProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA devInfoData, uint property)
        {
            byte[] buffer = new byte[1024];
            uint requiredSize = 0;

            if (SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devInfoData, property,
                out _, buffer, (uint)buffer.Length, out requiredSize))
            {
                return Encoding.Unicode.GetString(buffer).Trim('\0');
            }

            return null;
        }

        // Constants and Structs

        private const int DIGCF_PRESENT = 0x00000002;
        private const int DIGCF_ALLCLASSES = 0x00000004;
        private const int SPDRP_HARDWAREID = 0x00000001;
        private const int DIF_PROPERTYCHANGE = 0x12;
        private const int DICS_ENABLE = 1;
        private const int DICS_DISABLE = 2;
        private const int DICS_FLAG_GLOBAL = 1;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER classInstallHeader;
            public int StateChange;
            public int Scope;
            public int HwProfile;
        }

        // P/Invoke

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            uint Property,
            out uint PropertyRegDataType,
            byte[] PropertyBuffer,
            uint PropertyBufferSize,
            out uint RequiredSize
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiSetClassInstallParams(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            ref SP_PROPCHANGE_PARAMS ClassInstallParams,
            int ClassInstallParamsSize
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiCallClassInstaller(
            int InstallFunction,
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
    }
}