// Helpers/Settings.cs
using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace VSpaceParkers.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion

        public static int WiFiCode
        {
            get
            {
                return AppSettings.GetValueOrDefault("WiFiCode", 0);
            }
            set
            {
                AppSettings.AddOrUpdateValue("WiFiCode", value);
            }
        }

        public static string Apartment
        {
            get
            {
                return AppSettings.GetValueOrDefault("Apartment", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Apartment", value);
            }
        }

        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault("Username", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Username", value);
            }
        }

        public static string CurrentSite
        {
            get
            {
                return AppSettings.GetValueOrDefault("CurrentSite", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("CurrentSite", value);
            }
        }

        public static bool SupportButton
        {
            get
            {
                return AppSettings.GetValueOrDefault("SupportButton", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SupportButton", value);
            }
        }

        public static bool AttentionDisabled
        {
            get
            {
                return AppSettings.GetValueOrDefault("AttentionDisabled", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AttentionDisabled", value);
            }
        }

        public static bool MainGate
        {
            get
            {
                return AppSettings.GetValueOrDefault("MainGate", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("MainGate", value);
            }
        }

        public static bool SubscribeNotification
        {
            get
            {
                return AppSettings.GetValueOrDefault("SubscribeNotification", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SubscribeNotification", value);
            }
        }

        public static string SubscribeTopic
        {
            get
            {
                return AppSettings.GetValueOrDefault("SubscribeTopic", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("SubscribeTopic", value);
            }
        }

        public static bool MultipleSystems
        {
            get
            {
                return AppSettings.GetValueOrDefault("MultipleSystems", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("MultipleSystems", value);
            }
        }

        public static bool SpotNickNameEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault("SpotNickNameEnabled", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SpotNickNameEnabled", value);
            }
        }

        public static bool GateOpen
        {
            get
            {
                return AppSettings.GetValueOrDefault("GateOpen", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("GateOpen", value);
            }
        }

        public static string SpotOpen
        {
            get
            {
                return AppSettings.GetValueOrDefault("SpotOpen", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("SpotOpen", value);
            }
        }

        public static string SpotNickName
        {
            get
            {
                return AppSettings.GetValueOrDefault("SpotNickName", "none");
            }
            set
            {
                AppSettings.AddOrUpdateValue("SpotNickName", value);
            }
        }

        public static string WifiName
        {
            get
            {
                return AppSettings.GetValueOrDefault("WifiName", "VSpaceApp");
            }
            set
            {
                AppSettings.AddOrUpdateValue("WifiName", value);
            }
        }

        public static string WifiPW
        {
            get
            {
                return AppSettings.GetValueOrDefault("WifiPW", "WiFi4VSpace");
            }
            set
            {
                AppSettings.AddOrUpdateValue("WifiPW", value);
            }
        }

        public static string BasePortNumber
        {
            get
            {
                return AppSettings.GetValueOrDefault("BasePortNumber", "5000");
            }
            set
            {
                AppSettings.AddOrUpdateValue("BasePortNumber", value);
            }
        }


        public static string BaseIPAddress
        {
            get
            {
                return AppSettings.GetValueOrDefault("BaseIPAddress", "192.168.0.200");
            }
            set
            {
                AppSettings.AddOrUpdateValue("BaseIPAddress", value);
            }
        }


        public static bool SSLTrusted
        {
            get
            {
                return AppSettings.GetValueOrDefault("SSLTrusted", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SSLTrusted", value);
            }
        }

        public static string SSLPublicKey
        {
            get
            {
                return AppSettings.GetValueOrDefault("SSLPublicKey", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("SSLPublicKey", value);
            }
        }

        public static string GUID
        {
            get
            {
                return AppSettings.GetValueOrDefault("GUID", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("GUID", value);
            }
        }


        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessToken", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessToken", value);
            }
        }

        public static string SpotID
        {
            get
            {
                return AppSettings.GetValueOrDefault("SpotID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("SpotID", value);
            }
        }

        public static string FullName
        {
            get
            {
                return AppSettings.GetValueOrDefault("FullName", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("FullName", value);
            }
        }

        // No longer set access token expirary
        /*
        public static DateTime AccessTokenExpirationDate
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessTokenExpirationDate", DateTime.UtcNow);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessTokenExpirationDate", value);
            }

        }
        */

    }
}
