using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VSpaceParkers
{
    public interface IWifiConnect
    {
        Task<int> ConnectToWifi(string ssid, string password);
        int DisconnectFromWifi(string ssid);
        int GetStatusCode();
        void SetDisconnected();
    }
}
