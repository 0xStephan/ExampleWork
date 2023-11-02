using System;
using VSpaceParkers.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using VSpaceParkers.Helpers;
using Plugin.FirebasePushNotification;

namespace VSpaceParkers.Pages
{
    public partial class EditProfile : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public EditProfile()
        {
            InitializeComponent();
            EditHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label))) * 1.25;

            if (Device.RuntimePlatform == Device.Android)
            {

                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {

                    Top.Padding = 15;
                    Top.Spacing = 10;
                    Elements.Padding = 15;
                    Elements.Spacing = 10;
                }
            }

            MyVersion.Text = "v" + AppInfo.VersionString;

            Elements.Children.Clear();

            Button NickNameButton = new Button
            {
                Text = ("Change my Spot Nicknames"),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,
            };
            NickNameButton.Clicked += Nick_Clicked;
            Elements.Children.Add(NickNameButton);

            Button PasswordButton = new Button
            {
                Text = ("Change my Password"),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,
            };
            PasswordButton.Clicked += Password_Clicked;
            Elements.Children.Add(PasswordButton);

            Button NoUpdatesButton = new Button
            {
                Text = ("Unsubscribe from Notifications"),
                TextColor = Color.White,
                BackgroundColor = Color.LightGray,
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,
            };
            NoUpdatesButton.Clicked += NoUpdates_Clicked;
            Elements.Children.Add(NoUpdatesButton);

            Button NoAttentionButton = new Button
            {
                Text = ("Enable/Disable Attention Warning"),
                TextColor = Color.White,
                BackgroundColor = Color.LightGray,
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,
            };
            NoAttentionButton.Clicked += NoAttention_Clicked;
            Elements.Children.Add(NoAttentionButton);

        }

        async void Nick_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditSpotNickName());
            return;
        }

        async void Password_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPassword());
            return;
        }

        async void NoUpdates_Clicked(object sender, EventArgs e)
        {
            CrossFirebasePushNotification.Current.UnsubscribeAll();
            await DisplayAlert("Unsubscribed", "Successfully unsubscribed from all updates", "OK");
            return;
        }

        async void NoAttention_Clicked(object sender, EventArgs e)
        {
            if (!Settings.AttentionDisabled)
            {
                Settings.AttentionDisabled = true;
                await DisplayAlert("Disabled", "The attention warning has been disabled", "OK");
            }
            else
            {
                Settings.AttentionDisabled = false;
                await DisplayAlert("Enabled", "The attention warning has been enabled", "OK");
            }

            return;
        }

        async void Logout_Clicked(object sender, EventArgs e)
        {
            Clear_User();
            await Navigation.PopToRootAsync();
            return;

        }

        private void Clear_User()
        {
            // Clear Access Token
            Settings.AccessToken = "";
            //Settings.AccessTokenExpirationDate = new DateTime(1990, 1, 1);
            Settings.SpotID = "";
            Settings.SpotNickNameEnabled = false;
            Settings.SpotNickName = "";
            Settings.MultipleSystems = false;
            Settings.MainGate = false;
            Settings.CurrentSite = "";
            return;
        }

    }
}
