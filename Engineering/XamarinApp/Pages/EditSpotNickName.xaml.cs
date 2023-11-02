using System;
using VSpaceParkers.Services;
using System.Diagnostics;
using VSpaceParkers.Helpers;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace VSpaceParkers.Pages
{
    public partial class EditSpotNickName : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        public EditSpotNickName()
        {
            InitializeComponent();

            NicknameFormHeading.FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.25;

            if (Device.RuntimePlatform == Device.Android)
            {
                Loading.Scale = 1.0;

                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {

                    Top.Padding = 15;
                    Top.Spacing = 10;
                    SpotNickNames.Padding = 15;
                    SpotNickNames.Spacing = 10;
                    Bottom.Padding = 15;
                    Bottom.Spacing = 10;
                }
            }

            SpotNickNames.Children.Clear();

            string[] userSpots = Settings.SpotID.ToString().Split(',');
            
            string[] userNicks = Settings.SpotNickName.Split(new string[] { Constants.StringSplitRegex }, StringSplitOptions.None);
            string entryMessage = "";
            try
            {
                string[] spotnumber;
                int systemNumber;

                // Build middle of the page
                foreach (var spot in userSpots)
                {
                    spotnumber = spot.Split('_');

                    // Convert System 1 to System A etc
                    int.TryParse(spotnumber[1], out systemNumber);

                    if (Settings.MultipleSystems)
                    {
                        entryMessage = "Set Spot " + spotnumber[0] + " System " + Constants.SystemLetter[systemNumber - 1] + " nickname";
                    }

                    else
                    {
                        entryMessage = "Set Spot " + spotnumber[0] + " nickname";
                    }

                    var entry = new Entry
                    {
                        Placeholder = entryMessage
                    };
                    SpotNickNames.Children.Add(entry);

                }
            }

            catch
            {
                SpotNickNames.Children.Clear();
                Label Disabled = new Label
                {
                    Text = "This functionality has been disabled on your account.",
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 0.8,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };
                Top.Children.Add(Disabled);

            }

            // Build bottom of the page
            /*
            Button Submit = new Button
            {
                Text = ("Submit"),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("199eca"),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,

            };
            Submit.Clicked += Submit_Clicked;
            Bottom.Children.Add(Submit);

            Button ClearAll = new Button
            {
                Text = ("Clear all"),
                TextColor = Color.Black,
                BackgroundColor = Color.Gray,
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold,

            };
            ClearAll.Clicked += ClearAll_Clicked;
            Bottom.Children.Add(ClearAll);
            */

            Label Message = new Label
            {
                Text = "Please make sure you update every nickname. Your old nicknames will not be saved." + Environment.NewLine + "Empty boxes will reset back to the spot number.",
                TextColor = Color.Gray,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
            };
            Bottom.Children.Add(Message);


        }

        async void ClearAll_Clicked(object sender, EventArgs e)
        {
            var option = await DisplayAlert("Clear All", "Are you sure you want to clear all nicknames?", "Yes", "No");

            if (option) 
            {
                Settings.SpotNickName = "none";
                Settings.SpotNickNameEnabled = false;

                Bottom.IsEnabled = false;

                Loading.IsRunning = true;
                Loading.IsEnabled = true;
                Loading.IsVisible = true;

                var response = await _apiServices.ChangeSpotNickName(Settings.AccessToken, "none");

                if (response == Constants.Offline)
                {
                    await DisplayAlert("Oh no!", "There was an error connecting to the network. Please reconnect and try again.", "OK");
                    await Navigation.PopToRootAsync();
                }

                Debug.WriteLine(response.ToString());

                await DisplayAlert("Great Success!", "All your nicknames have been cleared", "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            else
            {
                return;
            }

        }


        async void Submit_Clicked(object sender, EventArgs e)
        {
            
            var count = 0;
            string SpotNickNameStr = String.Empty;

            // Placeholder = "Set Spot: " + spotnumber[0] + " System: " + spotnumber[1] + " nickname"

            foreach (Entry entry in SpotNickNames.Children)
            {
                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    SpotNickNameStr = SpotNickNameStr + entry.Placeholder.Replace("Set ", "").Replace(" nickname", "") + Constants.StringSplitRegex;
                }
                else if ((entry.Text.Contains(Constants.StringSplitRegex)))
                {
                    await DisplayAlert("Oh no!", "A nickname entry contains disallowed text ", "OK");
                    return;
                }
                else
                {
                    SpotNickNameStr = SpotNickNameStr + entry.Text.ToString() + Constants.StringSplitRegex;
                    count = count + 1;
                }

            }

            if (count == 0)
            {
                // All entries have been left blank
                await DisplayAlert("Oh no!", "No input was detected. If you would like to remove nickname feature please press the Clear all button.", "OK");
                return;
            }

            // Disable buttons while we communicate to server
            Bottom.IsEnabled = false;

            // Push updates to server
            if (!(SpotNickNameStr.Equals("none")))
            {
                Loading.IsRunning = true;
                Loading.IsEnabled = true;
                Loading.IsVisible = true;

                var response = await _apiServices.ChangeSpotNickName(Settings.AccessToken, SpotNickNameStr.Remove(SpotNickNameStr.Length - (Constants.StringSplitRegex.Length)));
                Settings.SpotNickName = SpotNickNameStr.Remove(SpotNickNameStr.Length - Constants.StringSplitRegex.Length);
                Settings.SpotNickNameEnabled = true;
                Debug.WriteLine(response.ToString());

                if (response.Equals(Constants.Offline))
                {
                    await DisplayAlert("Oh no!", "We were unable to submit your changes to server. Nickname changes have been updated locally only.", "OK");
                    await Navigation.PopToRootAsync();
                }
            }

            else
            {
                var response = await _apiServices.ChangeSpotNickName(Settings.AccessToken, SpotNickNameStr);

                if (response.Equals(Constants.Offline) || response.Equals(Constants.Error))
                {
                    await DisplayAlert("Oh no!", "We were unable to submit your changes to server. Nickname changes have been updated locally only.", "OK");
                    await Navigation.PopToRootAsync();
                }
                Settings.SpotNickName = SpotNickNameStr;
                Debug.WriteLine(response.ToString());
            }
            await DisplayAlert("Great Success!", "Your spot nickname changes have been saved", "OK");
            Debug.WriteLine(SpotNickNameStr);

            await Navigation.PopToRootAsync();
            return;

        }
    }
}
