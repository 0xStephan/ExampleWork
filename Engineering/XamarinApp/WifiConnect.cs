using System;
namespace VSpaceParkers
{
    public interface IWifiConnect
    {
        void ConnectToWifi(string ssid, string password);
    }
}
