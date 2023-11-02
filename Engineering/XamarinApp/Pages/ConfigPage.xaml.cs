using System;
using VSpaceParkers.Helpers;
using Xamarin.Forms;

namespace VSpaceParkers.Pages
{
    public partial class ConfigPage : ContentPage
    {
        public ConfigPage()
        {
            InitializeComponent();


            Label SSLLabel = new Label
            {
                Text = "SSL Trusted",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            Elements.Children.Add(SSLLabel);

            Switch SSLSwitch = new Switch
            {
                HorizontalOptions = LayoutOptions.Center,
                IsToggled = Settings.SSLTrusted
            };
            SSLSwitch.Toggled += Switcher_Toggled;
            Elements.Children.Add(SSLSwitch);

            IP.Text = Settings.BaseIPAddress;
            IP.Placeholder = Settings.BaseIPAddress;

            Port.Text = Settings.BasePortNumber;
            Port.Placeholder = Settings.BasePortNumber;

            WifiName.Text = Settings.WifiName;
            WifiName.Placeholder = Settings.WifiName;

            WifiPW.Text = Settings.WifiPW;
            WifiPW.Placeholder = Settings.WifiPW;

            Label DeviceGUID = new Label
            {
                Text = "Device GUID: \n" + Settings.GUID,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            Elements.Children.Add(DeviceGUID);

            Label AccessTokenExpirary = new Label
            {
                //Text = "Access Token Expirary: \n" + Settings.AccessTokenExpirationDate.ToString(),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            Elements.Children.Add(AccessTokenExpirary);

            Button Submit = new Button
            {
                Text = "Update Settings"
            };
            Submit.Clicked += Submit_Clicked;
            Elements.Children.Add(Submit);

        }

        async void Switcher_Toggled(object sender, ToggledEventArgs e)
        {
            Settings.SSLTrusted = !(Settings.SSLTrusted);
            await DisplayAlert("SSL Trusted", "SSLTrusted setting is now: " + Settings.SSLTrusted.ToString(), "OK");
            return;
        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            Settings.BasePortNumber = Port.Text.ToString();
            Settings.BaseIPAddress = IP.Text.ToString();

            Settings.WifiName = WifiName.Text.ToString();
            Settings.WifiPW = WifiPW.Text.ToString();

            await DisplayAlert("Updated", "Settings have been updated", "OK");

            System.Diagnostics.Debug.WriteLine(Settings.BaseIPAddress);
            System.Diagnostics.Debug.WriteLine(Settings.BasePortNumber);
            System.Diagnostics.Debug.WriteLine(Settings.WifiName);
            System.Diagnostics.Debug.WriteLine(Settings.WifiPW);

            return;
        }

    }

}
