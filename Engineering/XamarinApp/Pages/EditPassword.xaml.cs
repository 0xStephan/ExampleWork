using System;
using VSpaceParkers.Services;
using VSpaceParkers.Helpers;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace VSpaceParkers.Pages
{
    public partial class EditPassword : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public EditPassword()
        {
            InitializeComponent();

            PasswordFormHeading.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.25;


            if (Device.RuntimePlatform == Device.Android)
            {
                Loading.Scale = 1.0;

                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {
                    
                    Top.Padding = 15;
                    Top.Spacing = 10;
                    Elements.Padding = 15;
                    Elements.Spacing = 10;
                    Bottom.Padding = 15;
                    Bottom.Spacing = 10;
                }
            }

        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            // Ensure New Password and Confirm New Password are the same
            if ((string.IsNullOrWhiteSpace(OldPassword.Text)) || (string.IsNullOrWhiteSpace(NewPassword.Text)) ||
                (string.IsNullOrWhiteSpace(ConfirmPassword.Text)))
            {
                await DisplayAlert("Oh no!", "Password fields cannot be left blank or contain whitespace", "OK");
                return;
            }


            if (!(NewPassword.Text.Equals(ConfirmPassword.Text)))
            {
                await DisplayAlert("Something's not right", "Your New Password and Confirm New Password must be the same!", "OK");
                return;
            }

            if (OldPassword.Text.Equals(NewPassword.Text))
            {
                await DisplayAlert("Nothing has changed", "Your new password and old password are the exact same. Please enter a new password that is different.", "OK");
                return;
            }

            if (NewPassword.Text.Length < Constants.MinPWLenth)
            {
                await DisplayAlert("Oh no!", "Your new password length must be " + Constants.MinPWLenth.ToString() +  " characters or more", "OK");
                return;
            }

            SubmitButton.IsEnabled = false;
            Loading.IsEnabled = true;
            Loading.IsVisible = true;
            Loading.IsRunning = true;


            var response = await _apiServices.ChangePassword(Settings.AccessToken, OldPassword.Text, NewPassword.Text);

            System.Diagnostics.Debug.WriteLine(response.ToString());

            if (response.Equals(Constants.Unauth))
            {
                await DisplayAlert("Something's not right", "Your old password is incorrect. Please try again.", "OK");
                SubmitButton.IsEnabled = true;
                Loading.IsRunning = false;
                return;
            }

            if (response.Equals(Constants.BadPassword))
            {
                await DisplayAlert("Oh no!", "Your new password is too weak. Please try a stronger password.", "OK");
                SubmitButton.IsEnabled = true;
                Loading.IsRunning = false;
                return;
            }

            if (response.Equals(Constants.OK))
            {
                await DisplayAlert("Great success!", "Your new password has been saved.", "OK");
                await Navigation.PopAsync();
                Navigation.RemovePage(this);
            }

            if (response.Equals(Constants.Error))
            {
                await DisplayAlert("Oh no!", "There was an error during the process. Please try again later or contact our team.", "OK");
                SubmitButton.IsEnabled = true;
                Loading.IsRunning = false;
                return;
            }

            if (response.Equals(Constants.Offline))
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. Please reconnect and try again.", "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            return;
        }
    }
}
