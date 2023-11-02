using System;
using Xamarin.Forms;
using VSpaceParkers.Helpers;
using System.Threading.Tasks;
using VSpaceParkers.Services;
using System.Diagnostics;
using Xamarin.Essentials;
using Plugin.FirebasePushNotification;

namespace VSpaceParkers.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public LoginPage()
        {
            // Clear Access Token if we return to this page
            Settings.AccessToken = "";
            //Settings.AccessTokenExpirationDate = new DateTime(1990, 1, 1);

            InitializeComponent();

            //LoginHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.8);
            ConnectingButton.IsVisible = false;

            if (Device.RuntimePlatform == Device.Android)
            {
                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {
                    Top.Spacing = 20;
                    Top.Padding = 15;

                }
            }

        }

        async void Register_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
            return;
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
        
            if ((string.IsNullOrWhiteSpace(Username.Text)) || (string.IsNullOrWhiteSpace(Password.Text)))
            {
                await DisplayAlert("Uh oh", "Username and/or password cannot be blank or contain whitespace", "OK");
                return;
            }

            if ((Username.Text.Equals("Config")) && (Password.Text.Equals("V$ConFiG")))
            {
                await Navigation.PushAsync(new ConfigPage());
                return;
            }

            if ((Username.Text.Equals("RESET")) && (Password.Text.Equals("RESET")))
            {
                await DisplayAlert("Connection Reset", "Connection settings reset. Please try logging in again.", "OK");
                Settings.SSLTrusted = false;
                Settings.AccessToken = "";
                Settings.SpotNickNameEnabled = false;
                Settings.BaseIPAddress = "192.168.2.200";
                Settings.BasePortNumber = "5000";
                return;
            }

            Debug.WriteLine("Login Clicked");
            Debug.WriteLine(Settings.WiFiCode);

            LoginButton.IsVisible = false;
            ConnectingButton.IsVisible = true;

            messageLabel.TextColor = Xamarin.Forms.Color.Transparent;

            var accesstoken = await _apiServices.LoginAsync(Username.Text.ToLower(), Password.Text);
            Settings.AccessToken = accesstoken;

            Debug.WriteLine(accesstoken);


            // If valid Access Token proceed to grab user details then bring
            if ((Settings.AccessToken != "") && (Settings.AccessToken != Constants.Offline) 
                && (Settings.AccessToken != Constants.Banned) && (Settings.AccessToken != Constants.Unauth))
            {

                //Sucessfully got an AccessToken so retrieve user details
                string received = await _apiServices.GetUserDetails(Settings.AccessToken);

                if (received.Equals(Constants.OK))
                {
                    Settings.GateOpen = false;

                    // Enable remote notifications
                    if (Settings.SubscribeNotification)
                    {
                        CrossFirebasePushNotification.Current.Subscribe("all");
                        CrossFirebasePushNotification.Current.Subscribe(Settings.SubscribeTopic);
                        System.Diagnostics.Debug.WriteLine(Settings.SubscribeTopic);

                        try
                        {
                            if (Settings.SpotID.Contains("_1"))
                            {
                                // Subscribe to System A notifications
                                CrossFirebasePushNotification.Current.Subscribe("A");
                            }

                            if (Settings.SpotID.Contains("_2"))
                            {
                                // Subscribe to System A notifications
                                CrossFirebasePushNotification.Current.Subscribe("B");
                            }

                            if (Settings.SpotID.Contains("_3"))
                            {
                                // Subscribe to System A notifications
                                CrossFirebasePushNotification.Current.Subscribe("C");
                            }
                        }

                        catch (Exception ex)
                        {
                            // Do nothing
                        }

                    }

                    await Navigation.PushAsync(new UserPage());
                    return;
                }

                else
                {
                    await DisplayAlert("Oh no!", "There was an error retrieving your account. Please contact support for assistance.", "OK");
                    return;
                }

            }

            else if (Settings.AccessToken == Constants.Offline)
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. Please reconnect and try again.", "OK");
                messageLabel.Text = "It looks like you're offline. Please reconnect and try again.";
                messageLabel.TextColor = Xamarin.Forms.Color.Black;
                await Task.Delay(4000);
                var result = await DisplayAlert("Go back home", "Would you like to return to the Connect page?", "Yes", "No");

                if (result)
                {
                    LoginButton.IsVisible = true;
                    ConnectingButton.IsVisible = false;
                    await Navigation.PopToRootAsync();
                    return;
                }

                return;
            }

            else if (Settings.AccessToken == Constants.Banned)
            {
                await DisplayAlert("Yikes!", "Your account has been locked out. If this is an error please contact support.", "OK");
                messageLabel.Text = "The application administrator has locked you out from this service.";
                messageLabel.TextColor = Xamarin.Forms.Color.Black;
                return;
            }

            else
            {
                messageLabel.TextColor = Xamarin.Forms.Color.Black;
                messageLabel.Text = "Login failed, incorrect credentials. Please try again.";
                await Task.Delay(4000);
                LoginButton.IsVisible = true;
                ConnectingButton.IsVisible = false;
                return;
            }

        }

        async void Forgot_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Help is on its way", "You will be directed to the Support page.", "OK");

            await Navigation.PushAsync(new SupportPage());
            return;
        }

    }
}
