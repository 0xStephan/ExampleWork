using System;
using System.Collections.Generic;
using VSpaceParkers.Services;
using VSpaceParkers.Helpers;

using Xamarin.Forms;

namespace VSpaceParkers.Pages
{
    public partial class ChangePassword : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public ChangePassword()
        {
            InitializeComponent();

            Button Submit = new Button
            {
                Text = ("Submit " + Environment.NewLine),
                TextColor = Color.Black,
                BackgroundColor = Color.FromHex("00ffff"),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,

            };
            Submit.Clicked += Submit_Clicked;

            Elements.Children.Add(Submit);

        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            // Ensure New Password and Confirm New Password are the same

            if (!(NewPassword.Text.Equals(ConfirmPassword.Text)))
            {
                await DisplayAlert("Not matching", "New Password and Confirm New Password must be the same!", "OK");
                return;
            }

            if (OldPassword.Text.Equals(NewPassword.Text))
            {
                await DisplayAlert("No change", "New password and old password are the same.", "OK");
                return;
            }

            var reponse = await _apiServices.ChangePassword(Settings.AccessToken, OldPassword.Text, NewPassword.Text);



        }
    }
}
