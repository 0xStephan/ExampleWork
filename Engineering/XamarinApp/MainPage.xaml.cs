using System;
using Xamarin.Forms;
using VSpaceParkers.Pages;
using VSpaceParkers.Helpers;
using Xamarin.Essentials;
using System.Diagnostics;
using VSpaceParkers.Services;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]


namespace VSpaceParkers
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();
        int wifiResult = 0;

        IWifiConnect service;

    public MainPage()
        {
            InitializeComponent();

            CrossFirebasePushNotification.Current.OnNotificationReceived += Current_OnNotificationReceived;

            var version = Xamarin.Essentials.DeviceInfo.Version;

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (version.Major.ToString().Equals("13"))
                {
                    var minorV = (version.Minor.ToString());
                    Int32 minorVer = 0;
                    Int32.TryParse(minorV, out minorVer);

                    if (minorVer < 4)
                    {
                        DisplayAlert("Outdated Software", "Uh oh. You're using an outdated version of iOS. Please update your system software to the latest version to ensure safe operation.", "OK");
                    }

                }
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                System.Diagnostics.Debug.WriteLine("Android Screen Height");
                System.Diagnostics.Debug.WriteLine(DeviceDisplay.MainDisplayInfo.Height.ToString());
                System.Diagnostics.Debug.WriteLine("Android Screen Width");
                System.Diagnostics.Debug.WriteLine(DeviceDisplay.MainDisplayInfo.Width.ToString());
                System.Diagnostics.Debug.WriteLine("Android Screen Density");
                System.Diagnostics.Debug.WriteLine(DeviceDisplay.MainDisplayInfo.Density.ToString());
                
                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {
                    Loading.Scale = 1.0;
                    MainStack.Spacing = 20;
                    MainStack.Padding = 20;
                    ButtonStack.Spacing = 20;
                    ButtonStack.Padding = 15;
                }
            }


            // Ensure that the user has WiFi enabled
            if (Device.RuntimePlatform == Device.Android)
            {
                Loading.Scale = 1.0;
                service = DependencyService.Get<IWifiConnect>();
                var state = service.GetStatusCode();

                if (state == -1)
                {
                    DisplayAlert("Uh oh", "WiFi must be enabled to use this app.", "OK");
                    return;

                }
            }


            if (string.IsNullOrEmpty(Settings.GUID))
            {
                Settings.GUID = System.Guid.NewGuid().ToString();
            }

        }

        private void Current_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Received");
        }

        async void Connect_Clicked(object sender, EventArgs e)
        {
            ConnectButton.IsEnabled = false;

            Loading.IsEnabled = true;
            Loading.IsRunning = true;

            Debug.WriteLine("Try to call WifiConnect");

            int result = 0;

            Debug.WriteLine(result);

            // Ensure that the user has WiFi enabled
            service = DependencyService.Get<IWifiConnect>();

            if (Device.RuntimePlatform == Device.Android)
            {
                var state = service.GetStatusCode();

                if (state == -1)
                {
                    await DisplayAlert("Uh oh", "WiFi must be enabled to use this app. Please enable it and try again.", "OK");
                    Loading.IsEnabled = false;
                    Loading.IsRunning = false;
                    ConnectButton.IsEnabled = true;
                    return;

                }
            }

           ////////////////////////////////
           /// NEED TO FIX


            wifiResult = await service.ConnectToWifi(Settings.WifiName, Settings.WifiPW);

            if (Device.RuntimePlatform == Device.Android)
            {
                // Attempt wifiResult again
                if (wifiResult == -1)
                {
                    wifiResult = await service.ConnectToWifi(Settings.WifiName, Settings.WifiPW);
                }
            }

            if (wifiResult == -1)
            {
                await DisplayAlert("Uh oh", "Unable to connect to the network. Please ensure you are near by and try again.", "OK");
                Loading.IsEnabled = false;
                Loading.IsRunning = false;
                ConnectButton.IsEnabled = true;
                return;
            }

            await Task.Delay(500);


            // Double check we can connect to the host
            var APIonline = await _apiServices.OnlineCheck();

            // Attempt APIonline again
            if (!APIonline)
            {
                APIonline = await _apiServices.OnlineCheck();
            }

            if (!APIonline)
            {
                await DisplayAlert("Uh oh", "Unable to connect to the system. Please ensure you are connected to the WiFi network.", "OK");
                Loading.IsEnabled = false;
                Loading.IsRunning = false;
                ConnectButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine("IoTCheck: ");
            Debug.WriteLine(APIonline);
            Debug.WriteLine("Connect Clicked");

            //int dateCompare = DateTime.Compare(DateTime.Now, Settings.AccessTokenExpirationDate);

            if (!(Settings.AccessToken.Equals("")))  // Access token is still valid
            {
                Loading.IsEnabled = false;
                Loading.IsRunning = false;
                ConnectButton.IsEnabled = true;
                await Navigation.PushAsync(new UserPage());
            }

            else if (Settings.AccessToken.Length < 3)
            {
                Loading.IsEnabled = false;
                Loading.IsRunning = false;
                ConnectButton.IsEnabled = true;
                await Navigation.PushAsync(new LoginPage());
            }

            else // Access token is no longer valid
            {
                Loading.IsEnabled = false;
                Loading.IsRunning = false;
                ConnectButton.IsEnabled = true;
                await Navigation.PushAsync(new LoginPage());
            }


        }

        async void Support_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupportPage());
            return;
        }

        async void Register_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
            return;
        }

    }
}
