using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using VSpaceParkers.Helpers;

namespace VSpaceParkers.Pages
{
    public partial class SupportPage : ContentPage
    {
        public SupportPage()
        {
            InitializeComponent();

            Label Heading = new Label
            {
                Text = "Need help?",
                TextColor = System.Drawing.Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                FontSize = ((Device.GetNamedSize(NamedSize.Large, typeof(Label))) * 1.25),
                FontAttributes = FontAttributes.Bold,
            };
            Top.Children.Add(Heading);

            Label Message = new Label
            {
                Text = "Are you stuck or running into any issues?" + Environment.NewLine +  "Get in contact with our friendly team now.",
                TextColor = System.Drawing.Color.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                FontSize = ((Device.GetNamedSize(NamedSize.Medium, typeof(Label)))),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center
                
            };
            Top.Children.Add(Message);


            Button CallUs = new Button
            {
                Text = ("Call Us"),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold
            };
            CallUs.Clicked += CallUs_Clicked;
            Top.Children.Add(CallUs);


            
            Button OnlineForm = new Button
            {
                Text = ("Online Help"),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                BorderColor = Color.Black,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold
            };
            OnlineForm.Clicked += OnlineForm_Clicked;
            Top.Children.Add(OnlineForm);


            Label Message2 = new Label
            {
                Text = "P.S. Make sure you walk to an area with mobile reception if you plan on calling us",
                TextColor = System.Drawing.Color.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                FontSize = ((Device.GetNamedSize(NamedSize.Small, typeof(Label)))),
                HorizontalTextAlignment = TextAlignment.Center

            };
            Top.Children.Add(Message2);

            /*
            if (Device.RuntimePlatform == Device.iOS)
            {
                Button iOSB = new Button
                {
                    Text = "FaceTime",
                    TextColor = Color.White,
                    BackgroundColor = Color.FromHex(Constants.ButtonColor),
                    CornerRadius = 5,
                    FontAttributes = FontAttributes.Bold
                };
                Top.Children.Add(iOSB);
            }

            Button WhatsApp = new Button
            {
                Text = "WhatsApp",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                CornerRadius = 5,
                FontAttributes = FontAttributes.Bold
            };
            WhatsApp.Clicked += WhatsApp_Clicked;
            Top.Children.Add(WhatsApp);

            Button AppStore_WhatsApp_Button = new Button
            {
                Text = "Download WhatsApp",
                TextColor = Color.Gray,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                FontAttributes = FontAttributes.Bold
            };
            AppStore_WhatsApp_Button.Clicked += AppStore_WhatsApp_Clicked;
            Top.Children.Add(AppStore_WhatsApp_Button);
            */



        }

        /*
        async void WhatsApp_Clicked(object sender, EventArgs e)
        {
            //const string quote = "\"";
            string details = "-- Support Request --" + Environment.NewLine + 
                             "Name: " + Settings.FullName + Environment.NewLine +
                             "Username: " + Settings.Username + Environment.NewLine +
                             "Site Address: " + Settings.CurrentSite + Environment.NewLine +
                             "Apartment: " + Settings.Apartment + Environment.NewLine + 
                             "Spots: " + Settings.SpotID.Replace(",", ", ");


            var whatsappresult = await Launcher.CanOpenAsync("whatsapp://send?text=" + details + "&phone=61423883502");

            //WhatsApp is not installed or could not be opened
            if (!whatsappresult)
            {
                var result = await DisplayAlert("Application Not Found", "WhatsApp does not appear to be installed. Would you like to download it now?", "Yes", "No");

                if (result)
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await Launcher.OpenAsync("https://apps.apple.com/app/whatsapp-messenger/id310633997");
                    }

                    else
                    {
                        await Launcher.OpenAsync("https://play.google.com/store/apps/details?id=com.whatsapp");
                    }
                }

                return;
            }

            else
            {
                await Launcher.OpenAsync("whatsapp://send?text=" + details + "&phone=61423883502");
            }

            return;

        }

        void AppStore_WhatsApp_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri("https://www.vspaceparkers.com.au/contact-us/#form"));

            if(Device.RuntimePlatform == Device.iOS)
            {
                Launcher.OpenAsync("https://apps.apple.com/app/whatsapp-messenger/id310633997");
            }

            else
            {
                Launcher.OpenAsync("https://play.google.com/store/apps/details?id=com.whatsapp");
            }

            return;
        }

         */

        void CallUs_Clicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.PhoneDialer.Open("1300877223");
            return;
        }

        void OnlineForm_Clicked(object sender, EventArgs e)
        {
            //Device.OpenUri(new Uri("https://www.vspaceparkers.com.au/contact-us/#form"));
            Launcher.OpenAsync("https://www.vspaceparkers.com.au/contact-us/");

            return;
        }


    }
}
