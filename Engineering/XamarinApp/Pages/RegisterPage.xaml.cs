using System;
using System.Collections.Generic;
using System.Diagnostics;
using VSpaceParkers.Services;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace VSpaceParkers.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public RegisterPage()
        {
            InitializeComponent();

            RegisterHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.25);

            if (Device.RuntimePlatform == Device.Android)
            {
                Loading.Scale = 1.0;
            }
        }

        private async void Scan_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Scanned Clicked");
            var scan = new ZXingScannerPage();
            await Navigation.PushAsync(scan);
            scan.OnScanResult += (result) =>
            {
                Debug.WriteLine("OnScanResult");
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Debug.WriteLine("Invoke MainThread");
                    await Navigation.PopAsync();

                    ScanButton.IsEnabled = false;
                    Loading.IsEnabled = true;
                    Loading.IsRunning = true;

                    Debug.WriteLine("Result:");
                    Debug.WriteLine(result.Text);

                    var spotid = await _apiServices.RegisterStep1(result.Text);

                    Debug.WriteLine("Register Page");
                    Debug.WriteLine(spotid.ToString());

                    if (spotid.Equals(Constants.Offline))
                    {
                        await DisplayAlert("Network Error", "Error connecting to the network. Please reconnect and try again.", "OK");
                        await Navigation.PopToRootAsync();
                    }

                    else if (spotid.Equals(Constants.Error))
                    {
                        await DisplayAlert("Error", "The scanned QR code is invalid. Please contact support or try again.", "OK");
                        Loading.IsEnabled = false;
                        Loading.IsRunning = false;
                        ScanButton.IsEnabled = true;
                        return;
                    }

                    else if (spotid.Equals(Constants.UserExists))
                    {
                        await DisplayAlert("Error", "The scanned QR code has been used already. Please contact support or try again.", "OK");
                        Loading.IsEnabled = false;
                        Loading.IsRunning = false;
                        ScanButton.IsEnabled = true;
                        return;
                    }

                    else
                    {

                        await Navigation.PushAsync(new RegisterFormPage(spotid, result.Text));
                        Loading.IsEnabled = false;
                        Loading.IsRunning = false;
                        ScanButton.IsEnabled = true;
                        return;
                    }

                });
            };

        }
    }
}
