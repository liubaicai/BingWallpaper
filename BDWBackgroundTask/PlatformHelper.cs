using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDWBackgroundTask
{
    public static class PlatformHelper
    {
        private static bool? _isMobile;
        private static bool? _hasScannerAPI;

        public static bool IsMobile
        {
            get
            {
                if (_isMobile.HasValue)
                {
                    return _isMobile.Value;
                }

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    _isMobile = true;
                }
                else
                {
                    _isMobile = false;
                }

                return _isMobile.Value;
            }
        }

        public static bool HasScannerAPI
        {
            get
            {
                if (_hasScannerAPI.HasValue)
                {
                    return _hasScannerAPI.Value;
                }

                if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Devices.Scanner.ScannerDeviceContract", 3))
                {
                    _hasScannerAPI = true;
                }
                else
                {
                    _hasScannerAPI = false;
                }

                return _hasScannerAPI.Value;
            }
        }

        public static DeviceType DeviceFamily
        {
            get
            {
                var family = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if (!string.IsNullOrEmpty(family))
                {
                    if (family.Contains("Mobile"))
                    {
                        return DeviceType.Mobile;
                    }
                    if (family.Contains("Desktop"))
                    {
                        return DeviceType.Desktop;
                    }
                    if (family.Contains("Xbox"))
                    {
                        return DeviceType.Xbox;
                    }
                    if (family.Contains("Holographic"))
                    {
                        return DeviceType.Holographic;
                    }
                    if (family.Contains("Team"))
                    {
                        return DeviceType.Team;
                    }
                    if (family.Contains("Universal"))
                    {
                        return DeviceType.Universal;
                    }
                    if (family.Contains("IoT"))
                    {
                        return DeviceType.IoT;
                    }
                }
                return DeviceType.Unknown;
            }
        }
    }

    public enum DeviceType
    {
        Unknown,
        Mobile,
        Desktop,
        Xbox,
        Holographic,
        Team,
        Universal,
        IoT
    }
}
