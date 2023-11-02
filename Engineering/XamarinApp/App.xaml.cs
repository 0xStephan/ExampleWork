using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VSpaceParkers.Helpers;
using Plugin.FirebasePushNotification;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VSpaceParkers
{
    public partial class App : Application
    {
        DateTime closedTime;
        DateTime openTime;

        public App()
        {
            System.Diagnostics.Debug.WriteLine("Firing App()");
            DependencyService.Get<INotificationManager>().Initialize();

            //Xamarin.Forms.DependencyService.Register<IWifiConnect>();

            IWifiConnect service = DependencyService.Get<IWifiConnect>();

            // No longer disconnect from WiFi on sleep on app 1.39 or above
            //
            /*
            if (Device.RuntimePlatform == Device.iOS)
            {
                MessagingCenter.Subscribe<string>(this, "IosSleep", (str) =>
                {
                    
                    service = DependencyService.Get<IWifiConnect>();
                    var result = service.DisconnectFromWifi(Settings.WifiName);
                    System.Diagnostics.Debug.WriteLine("Disconnect Wifi");

                });
            }
            */

            if (Device.RuntimePlatform == Device.Android)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Clear All OnFire Notifications");
                    INotificationManager notificationManager;
                    notificationManager = DependencyService.Get<INotificationManager>();
                    notificationManager.ClearNotifications();
                }

                catch (Exception e)
                {

                }
            }


            //var result = service.ConnectToWifi("V-Space", "V1234567s");

            //var result = service.ConnectToWifi(Settings.WifiName, Settings.WifiPW);


            MainPage = new NavigationPage(new MainPage());

            //CrossFirebasePushNotification.Current.Subscribe("all");
            //CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;


        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Notification Token: {e.Token}");
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            closedTime = DateTime.UtcNow;

            System.Diagnostics.Debug.WriteLine("Firing OnSleep()");
            if (Settings.GateOpen)
            {
                INotificationManager notificationManager;
                notificationManager = DependencyService.Get<INotificationManager>();
                //notificationManager.ScheduleNotification("Scheduled Work", "The stacker system will have maintenance work performed tomorrow from 6am to 6.30am");
                notificationManager.ScheduleNotification("WARNING", "Ensure you close the system gate to avoid penalties!");
            }


            //var result = service.DisconnectFromWifi("V-Space");

            //
            // No longer disconnect from WiFi on sleep on app 1.39 or above
            //
            /*
            if (Device.RuntimePlatform == Device.Android)
            {
                IWifiConnect service = DependencyService.Get<IWifiConnect>();
                var result = service.DisconnectFromWifi(Settings.WifiName);
            }
            */


            // Handle when your app sleeps

       

            return;
        }

        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine("Firing OnResume()");

            openTime = DateTime.UtcNow;
            
            if (Settings.GateOpen)
            {
                System.Diagnostics.Debug.WriteLine("Clear All OnResume Notifications");
                INotificationManager notificationManager;
                notificationManager = DependencyService.Get<INotificationManager>();
                notificationManager.ClearNotifications();
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Clear All OnFire Notifications");
                    INotificationManager notificationManager;
                    notificationManager = DependencyService.Get<INotificationManager>();
                    notificationManager.ClearNotifications();
                }

                catch (Exception e)
                {

                }
            }

            // If App has been in background for more than 30 minutes
            // Push back to Connect page
            if (closedTime.AddMinutes(30) < openTime)
            {
                Application.Current.MainPage.Navigation.PopToRootAsync();
            }

            // Force user back to Root Page
            //
            // No longer force to root page on app 1.39 and above
            //
            // Application.Current.MainPage.Navigation.PopToRootAsync();
            
        }
    }
}
