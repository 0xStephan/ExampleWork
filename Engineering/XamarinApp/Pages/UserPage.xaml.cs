using System;
using Xamarin.Forms;
using VSpaceParkers.Helpers;
using VSpaceParkers.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using Plugin.FirebasePushNotification;

namespace VSpaceParkers.Pages
{

    public partial class UserPage : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();

        INotificationManager notificationManager;

        IWifiConnect service;


        public UserPage()
        {
            
            InitializeComponent();
            Buttons.Children.Clear();

            // Final check to ensure we should be here
            if (string.IsNullOrEmpty(Settings.AccessToken))
            {
                Navigation.PopToRootAsync();
                return;
            }

            if (string.IsNullOrEmpty(Settings.SpotID))
            {
                Navigation.PopToRootAsync();
                return;
            }

            //SupportButton.IsVisible = Settings.SupportButton;
            SupportButton.IsVisible = false;

            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };

            ErrorFrame.HasShadow = false;

            WelcomeHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Button)) * 1.5);
            FullName.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Button)) * 1.2);

            if (Device.RuntimePlatform == Device.Android)
            {
                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {
                    WelcomeHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Button)));
                    FullName.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Button)) * 1.2);
                    User.Padding = 10;
                    User.Spacing = 10;
                    Headings.Spacing = 10;
                    Buttons.Spacing = 10;
                }
            }

            try
            {
                var CurrentHour = DateTime.Now.Hour;

                // 5 AM to 11.59 AM
                if ((CurrentHour >= 5) & (CurrentHour < 12))
                {
                    WelcomeHeading.Text = "Good morning";
                }

                // 12 PM to 4.59 PM
                else if (((CurrentHour >= 12) & (CurrentHour < 17)))
                {
                    WelcomeHeading.Text = "Good afternoon";
                }

                // 5 PM to 4.59 AM
                else
                {
                    WelcomeHeading.Text = "Good evening";
                }

            }

            

            catch (Exception e)
            {
                // Error getting system time
                WelcomeHeading.Text = "Welcome back";
            }

            
            

            if (Settings.GateOpen)
            {
                var button = new Button()
                {

                    Text = "Close Gate",
                    TextColor = Color.Black,
                    BackgroundColor = Color.Orange,
                    BorderColor = Color.Orange,
                    CornerRadius = 5,
                    BorderWidth = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                    FontAttributes = FontAttributes.Bold,
                    StyleId = Settings.SpotOpen,
                    ClassId = Settings.MultipleSystems.ToString()
                };
                button.Clicked += Close_Gate;
                Buttons.Children.Add(button);

                Buttons.IsEnabled = true;
                
                ErrorConnect.Text = "Please ensure that the system is safe for operation prior to closing the gate." + Environment.NewLine + Environment.NewLine + "No persons are to be inside the system and parked cars are to be parked correctly adhering to the relevant safety requirements.";
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
                ErrorConnect.TextColor = Color.OrangeRed;
                ErrorConnect.IsVisible = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;

                FullName.Text = Settings.FullName;

                DisplayAlert("Previous Session Detected", "The system has detected that a previous session is still open. Please close the gate to end the session.", "OK");

                return;
            }


            Settings.GateOpen = false;
            Settings.SpotOpen = String.Empty;


            // Dynamically create buttons for each spot the user owns
            Debug.WriteLine(Settings.SpotID.ToString());

            string[] userSpots = Settings.SpotID.ToString().Split(',');

            if (userSpots.Length == 0)
            {
                FullName.Text = "Error loading user values.";
                return;
            }

            if (userSpots.Length > 1)
            {
                ReadyCall.Text = "Your spots are ready to be called";
            }

            else
            {
                ReadyCall.Text = "Your spot is ready to be called";
            }


            if (Settings.MainGate)
            {
                var button = new Button()
                {
                    Text = "Main Gate",
                    TextColor = Color.FromHex(Constants.ButtonColor),
                    BackgroundColor = Color.White,
                    BorderColor = Color.FromHex(Constants.ButtonColor),
                    BorderWidth = 2,
                    CornerRadius = 5,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                    FontAttributes = FontAttributes.Bold,
                };
                button.Clicked += Call_MainGate;
                Buttons.Children.Add(button);
            }

            // Quickly check if we have other systems
            foreach (var spot in userSpots)
            {
                if (Settings.MultipleSystems)
                {
                    break;
                }

                if (!(spot.Contains("_1")))
                {
                    // Spot is NOT from system 1
                    // Enable multiple systems
                    Settings.MultipleSystems = true;
                    Debug.WriteLine("multi system enabled");
                }
            }

            int i;

            // Additional check for safety in case where SpotNickNameEnabled
            // is cached to previous values in memory (ie true when it should be false)

            
            try
            {
                // Spot Nickname set
                if ((Settings.SpotNickNameEnabled))
                {
                    i = 0;
                    string[] userNicks = Settings.SpotNickName.Split(new string[] { Constants.StringSplitRegex }, StringSplitOptions.None);
                    foreach (var spot in userSpots)
                    {
                        //Debug.WriteLine(spot.ToString());

                        var button = new Button()
                        {
                            Text = (userNicks[i]),
                            TextColor = Color.White,
                            BackgroundColor = Color.FromHex(Constants.ButtonColor),
                            CornerRadius = 5,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.End,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                            FontAttributes = FontAttributes.Bold,
                            StyleId = spot,
                            ClassId = i.ToString() + Settings.MultipleSystems.ToString()
                        };
                        button.Clicked += Call_Spot;
                        Buttons.Children.Add(button);
                        i++;
                    }
                }

                // No Spot Nickname set
                else
                {
                    string[] spotnumber;
                    int systemNumber;


                    foreach (var spot in userSpots)
                    {
                        //Debug.WriteLine(spot.ToString());
                        spotnumber = spot.Split('_');

                        // Convert System 1 to System A etc
                        int.TryParse(spotnumber[1], out systemNumber);
                        

                        if (Settings.MultipleSystems)
                        {
                            var button = new Button()
                            {

                                Text = ("Spot " + spotnumber[0] + " System " + Constants.SystemLetter[systemNumber - 1]),
                                TextColor = Color.White,
                                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                                CornerRadius = 5,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.End,
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                                FontAttributes = FontAttributes.Bold,
                                StyleId = spot,
                                ClassId = Settings.MultipleSystems.ToString()
                            };
                            button.Clicked += Call_Spot;
                            Buttons.Children.Add(button);
                        }

                        else
                        {
                            var button = new Button()
                            {

                                Text = ("Spot " + spotnumber[0]),
                                TextColor = Color.White,
                                BackgroundColor = Color.FromHex(Constants.ButtonColor),
                                CornerRadius = 5,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.End,
                                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                                FontAttributes = FontAttributes.Bold,
                                StyleId = spot,
                                ClassId = Settings.MultipleSystems.ToString()
                            };
                            button.Clicked += Call_Spot;
                            Buttons.Children.Add(button);
                        }
                        
                    }
                }

                //SpotNumber.Text = Settings.SpotID.Replace(",", ", ");

                /*
                if (!(Settings.MultipleSystems))
                {
                    SpotNumber.Text = SpotNumber.Text.Replace("_1", String.Empty);
                }
                */

                // Display user full name
                FullName.Text = Settings.FullName;
            }

            catch (Exception ex)
            {
                FullName.Text = ex.Message.ToString();
            }

        }

        async void Call_MainGate(object sender, EventArgs e)
        {
            var action = await DisplayAlert("Open Main Gate", "Are you sure you want to open the Main Gate?", "Yes", "No");

            if (!action)
            {
                return;
            }

            Buttons.IsEnabled = false;

            var myButton = sender as Button;

            myButton.TextColor = Color.Black;
            myButton.BackgroundColor = Color.LightGray;
            myButton.BorderColor = Color.LightGray;
            myButton.Text = "Main Gate has been requested";

            var CalledSpot = await _apiServices.CallSpot(Settings.AccessToken, Constants.MainGate, Constants.StartCycle);

            // Network Error
            if (CalledSpot[0] == Constants.Offline)
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. You will be directed back to the homepage.", "OK");
                service = DependencyService.Get<IWifiConnect>();
                service.SetDisconnected();
                await Navigation.PopToRootAsync();

                ErrorConnect.Text = "It looks like you're offline. Please reconnect and try again.";
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
                return;
            }

            else if (CalledSpot[0] == Constants.OK)
            {
                await DisplayAlert("Great success!", "Main Gate is now opening.", "OK");
                Buttons.IsEnabled = true;

                myButton.Text = "Main Gate";
                myButton.TextColor = Color.FromHex(Constants.ButtonColor);
                myButton.BackgroundColor = Color.White;
                myButton.BorderColor = Color.FromHex(Constants.ButtonColor);
                return;
            }

            else if (CalledSpot[0] == Constants.Custom)
            {
                await DisplayAlert("Alert", CalledSpot[1], "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            else
            {
                await DisplayAlert("Error", "There was an error requesting to open the Main Gate.", "OK");
                myButton.Text = "Error calling Main Gate";
                myButton.TextColor = Color.DarkRed;
                myButton.BackgroundColor = Color.WhiteSmoke;
                myButton.BorderColor = Color.Black;
                myButton.Clicked -= Call_MainGate;

                
                ErrorConnect.Text = "Error connecting to the network. Please logout and try again.";
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;

                //Buttons.IsEnabled = true;

                return;
            }

        }

        async void Close_Gate(object sender, EventArgs e)
        {

            if (!Settings.AttentionDisabled)
            {
                await DisplayAlert("ATTENTION", "No persons are to be inside the system and parked cars are to be parked correctly adhering to the relevant safety requirements.", "I understand");
            }

            var action = await DisplayAlert("Close Gate",
                "Are you sure you want to close the gate?", "Yes", "No");

            if (!action)    // If user selects No for alert do nothing
            {
                return;
            }

            // Disable all buttons and re-enable at the end (ie after delay functions)
            Buttons.IsEnabled = false;

            var myButton = sender as Button;

            myButton.TextColor = Color.Black;
            myButton.BackgroundColor = Color.LightGray;
            myButton.BorderColor = Color.LightGray;
            myButton.Text = "Close Gate has been requested";

            // Spot information is stored into the StyleID parameter of each button
            var spotID = myButton.StyleId;
            string[] spotnumber = spotID.Split(Constants.SpotIDSeperator);
            bool multiSystem = myButton.ClassId.Contains("True");

            var CalledSpot = await _apiServices.CallSpot(Settings.AccessToken, spotID, Constants.EndCycle);

            if (CalledSpot[0] == Constants.EmergencyStopPress)
            {
                await DisplayAlert("Emergency Stop", "The Emergency Stop has been pressed. The machine will not operate until this is released.", "OK");

                
                ErrorConnect.Text = "The Emergency Stop has been pressed." + Environment.NewLine + Environment.NewLine + "The machine will not operate until this is released.";
                ErrorConnect.TextColor = Color.Red;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.FontAttributes = FontAttributes.Bold;
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;

                await Task.Delay(7000);
                myButton.Text = "Close Gate";
                myButton.BackgroundColor = Color.Orange;
                myButton.BorderColor = Color.Orange;
                myButton.BorderWidth = 2;
                myButton.TextColor = Color.Black;
                Buttons.IsEnabled = true;

                return;
            }

            // Network Error
            else if (CalledSpot[0] == Constants.Offline)
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. You will be directed back to the homepage.", "OK");
                service = DependencyService.Get<IWifiConnect>();
                service.SetDisconnected();
                await Navigation.PopToRootAsync();

                ErrorConnect.Text = "Error connecting to the network. Please logout and try again.";
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
                return;
            }

            else if (CalledSpot[0] == Constants.OK)
            {
                Settings.GateOpen = false;
                Settings.SpotOpen = String.Empty;
                await DisplayAlert("Almost done", "The gate will now start to close. Please ensure that it is fully closed before leaving.", "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            else if (CalledSpot[0] == Constants.Custom)
            {
                Settings.GateOpen = false;
                Settings.SpotOpen = String.Empty;
                await DisplayAlert(CalledSpot[2], CalledSpot[1], "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            else if (CalledSpot[0] == Constants.FrontSensorInterrupted)
            {
                await DisplayAlert("Uh oh", "The Front Sensor is interrupted." + Environment.NewLine + "This usually means you haven't parked correctly. Please ensure you park your vehicle correctly and try again.", "OK");
                
                ErrorConnect.Text = "Front Sensor is interrupted!" + Environment.NewLine + Environment.NewLine + "Please ensure you park your vehicle correctly before closing the gate.";
                ErrorConnect.TextColor = Color.White;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.FontAttributes = FontAttributes.Bold;
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Red;

                myButton.Text = "Close Gate";
                myButton.BackgroundColor = Color.Orange;
                myButton.BorderColor = Color.Orange;
                myButton.BorderWidth = 2;
                myButton.TextColor = Color.Black;
                Buttons.IsEnabled = true;

            }

            else if (CalledSpot[0] == Constants.RearSensorInterrupted)
            {
                await DisplayAlert("Uh oh", "The Rear Sensor is interrupted." + Environment.NewLine + "This usually means you haven't parked correctly. Please ensure you park your vehicle correctly and try again.", "OK");

                
                ErrorConnect.Text = "Rear Sensor is interrupted!" + Environment.NewLine + Environment.NewLine + "Please ensure you park your vehicle correctly before closing the gate.";
                ErrorConnect.TextColor = Color.White;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.FontAttributes = FontAttributes.Bold;
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Red;

                myButton.Text = "Close Gate";
                myButton.BackgroundColor = Color.Orange;
                myButton.BorderColor = Color.Orange;
                myButton.BorderWidth = 2;
                myButton.TextColor = Color.Black;
                Buttons.IsEnabled = true;
            }

            else if (CalledSpot[0] == Constants.Accepted)
            {

                await DisplayAlert("Hold on a second", "The system is still busy at the moment. Please wait and try again shortly.", "OK");
                myButton.Text = "System is busy right now";
                await Task.Delay(10000);
                myButton.Text = "Close Gate";
                myButton.BackgroundColor = Color.Orange;
                myButton.BorderColor = Color.Orange;
                myButton.BorderWidth = 2;
                myButton.TextColor = Color.Black;
                Buttons.IsEnabled = true;

            }

            else
            {
                Debug.WriteLine(CalledSpot.ToString());
                Debug.WriteLine(CalledSpot[0].ToString());
                await DisplayAlert("Error", "There was an error requesting to close the gate.", "OK");
                myButton.Text = "Close Gate";
                myButton.BackgroundColor = Color.Orange;
                myButton.BorderColor = Color.Orange;
                myButton.BorderWidth = 2;
                myButton.TextColor = Color.Black;
                Buttons.IsEnabled = true;

                return;
            }
        }

        async void Call_Spot(object sender, EventArgs e)
        {
            var myButton = sender as Button;


            // Spot information is stored into the StyleID parameter of each button
            var spotID = myButton.StyleId;
            string[] spotnumber = spotID.Split(Constants.SpotIDSeperator);
            bool multiSystem = myButton.ClassId.Contains("True");
            int systemNumber;

            // Convert System 1 to System A etc
            int.TryParse(spotnumber[1], out systemNumber);

            var callMessage = "";

            if (multiSystem)
            {
                callMessage = "Request Spot " + spotnumber[0] + " System " + Constants.SystemLetter[systemNumber - 1];
            }

            else
            {
                callMessage = "Request Spot " + spotnumber[0];
            }

            var action = await DisplayAlert(callMessage,
                "Are you sure you want to request this spot?", "Yes", "No");

            if (!action)    // If user selects No for alert do nothing
            {
                return;
            }

            if (Settings.GateOpen)
            {
                await DisplayAlert("Error", "Please finish the existing session before starting a new one.", "OK");
                return;
            }

            // Disable all buttons and re-enable at the end (ie after delay functions)
            Buttons.IsEnabled = false;

            // Update button properties
            if (Settings.SpotNickNameEnabled)
            {
                myButton.Text = myButton.Text + " has been requested";
            }

            else
            {
                myButton.Text = callMessage.Replace("Request ", "") + " has been requested";
                //myButton.Text = "Spot: " + spotnumber[0] + " System: " + spotnumber[1] + " has been called ";
            }

            // Change button properties
            myButton.TextColor = Color.Black;
            myButton.BackgroundColor = Color.LightGray;
            myButton.BorderColor = Color.LightGray;
            myButton.FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));
            myButton.HorizontalOptions = LayoutOptions.FillAndExpand;
            myButton.VerticalOptions = LayoutOptions.End;
            myButton.Clicked -= Call_Spot;
            //myButton.Clicked += Clicked_Again;

            // Call Spot API Service
            var CalledSpot = await _apiServices.CallSpot(Settings.AccessToken, spotID, Constants.StartCycle);

            System.Diagnostics.Debug.WriteLine("Calling platform");
            System.Diagnostics.Debug.WriteLine(CalledSpot[0].ToString());

            // Emergency Stop is pressed
            if (CalledSpot[0] == Constants.EmergencyStopPress)
            {
                await DisplayAlert("Emergency Stop", "The Emergency Stop has been pressed. The machine will not operate until this is released.", "OK");

                
                ErrorConnect.Text = "The Emergency Stop has been pressed." + Environment.NewLine + Environment.NewLine + "The machine will not operate until this is released.";
                ErrorConnect.TextColor = Color.Red;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorConnect.FontAttributes = FontAttributes.Bold;
                ErrorFrame.BackgroundColor = Color.Transparent;

                // Delay for 7 seconds before re-enabling button
                await Task.Delay(10000);

                // Rebuild button with nickname (if enabled)
                // Nickname index is stored in button as ClassID
                if (Settings.SpotNickNameEnabled)
                {
                    string[] userNicks = Settings.SpotNickName.Split(new string[] { Constants.StringSplitRegex }, StringSplitOptions.None);
                    myButton.Text = userNicks[Int32.Parse(myButton.ClassId.Replace("True", "").Replace("False", ""))];
                }

                else
                {
                    //myButton.Text = "Spot " + spotnumber[0] + " System " + spotnumber[1];
                    myButton.Text = callMessage.Replace("Request ", "");
                }

                myButton.Clicked -= Clicked_Again;
                myButton.Clicked += Call_Spot;
                myButton.BackgroundColor = Color.FromHex("#00ff99");
                Buttons.IsEnabled = true;

                return;
            }


            // Network Error
            else if (CalledSpot[0] == Constants.Offline)
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. You will be directed back to the homepage.", "OK");
                service = DependencyService.Get<IWifiConnect>();
                service.SetDisconnected();
                await Navigation.PopToRootAsync();

                ErrorConnect.Text = "Error connecting to the network. Please logout and try again.";
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.IsEnabled = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
                //Buttons.IsEnabled = true;
                return;
            }

            // Success code
            else if (CalledSpot[0] == Constants.OK)
            {
                if (Settings.SpotNickNameEnabled)
                {
                    await DisplayAlert("Great success!", myButton.Text.Replace("\n", String.Empty) + " and should arrive shortly." + Environment.NewLine + Environment.NewLine + "Please ensure you close the gate after system use.", "OK");
                    Settings.GateOpen = true;
                    Settings.SpotOpen = spotID;

                    // Delay for 10 seconds before re-enabling button
                    await Task.Delay(10000);
                    Buttons.Children.Clear();

                    var button = new Button()
                    {

                        Text = "Close Gate",
                        TextColor = Color.Black,
                        BackgroundColor = Color.Orange,
                        BorderColor = Color.Orange,
                        CornerRadius = 5,
                        BorderWidth = 2,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                        FontAttributes = FontAttributes.Bold,
                        StyleId = spotID,
                        ClassId = Settings.MultipleSystems.ToString()
                    };
                    button.Clicked += Close_Gate;
                    Buttons.Children.Add(button);

                    //button.Text = "Close Gate";
                    //button.StyleId = spotID;
                    //button.BackgroundColor = Color.Orange ;
                    //button.BorderColor = Color.Orange;
                    //button.Clicked += Close_Gate;
                    Buttons.IsEnabled = true;

                    
                    ErrorConnect.Text = "Please ensure that the system is safe for operation prior to closing the gate." + Environment.NewLine + Environment.NewLine + "No persons are to be inside the system and parked cars are to be parked correctly adhering to the relevant safety requirements.";
                    ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
                    ErrorConnect.TextColor = Color.OrangeRed;
                    ErrorConnect.IsVisible = true;
                    ErrorFrame.IsVisible = true;
                    ErrorFrame.BackgroundColor = Color.Transparent;

                    return;
                }
                else
                {
                    await DisplayAlert("Great success!", callMessage.Replace("Request ", "") + " has been requested and should arrive shortly." + Environment.NewLine + Environment.NewLine + "Please ensure you close the gate after system use.", "OK");
                    Settings.GateOpen = true;
                    Settings.SpotOpen = spotID;

                    // Delay for 7 seconds before re-enabling button
                    await Task.Delay(7000);

                    Buttons.Children.Clear();

                    var button = new Button()
                    {

                        Text = "Close Gate",
                        TextColor = Color.Black,
                        BackgroundColor = Color.Orange,
                        BorderColor = Color.Orange,
                        CornerRadius = 5,
                        BorderWidth = 2,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.End,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                        FontAttributes = FontAttributes.Bold,
                        StyleId = spotID,
                        ClassId = Settings.MultipleSystems.ToString()
                    };
                    button.Clicked += Close_Gate;
                    Buttons.Children.Add(button);

                    //myButton.Text = "Close Gate";
                    //myButton.BackgroundColor = Color.Orange;
                    //myButton.BorderColor = Color.Orange;
                    //myButton.Clicked += Close_Gate;


                    Buttons.IsEnabled = true;

                    
                    ErrorConnect.Text = "Please ensure that the system is safe for operation prior to closing the gate." + Environment.NewLine + Environment.NewLine + "No persons are to be inside the system and parked cars are to be parked correctly adhering to the relevant safety requirements.";
                    ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
                    ErrorConnect.TextColor = Color.OrangeRed;
                    ErrorConnect.IsVisible = true;
                    ErrorFrame.IsVisible = true;
                    ErrorFrame.BackgroundColor = Color.Transparent;

                    // notificationManager.ScheduleNotification("Close Gate", "Please ensure you close the gate");

                    return;
                }
                
            }

            else if (CalledSpot[0] == Constants.SessionClose)
            {
                await DisplayAlert(CalledSpot[2], CalledSpot[1], "OK");
                Settings.GateOpen = true;
                Settings.SpotOpen = spotID;
                Buttons.Children.Clear();

                var button = new Button()
                {

                    Text = "Close Gate",
                    TextColor = Color.Black,
                    BackgroundColor = Color.Orange,
                    BorderColor = Color.Orange,
                    CornerRadius = 5,
                    BorderWidth = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                    FontAttributes = FontAttributes.Bold,
                    StyleId = spotID,
                    ClassId = Settings.MultipleSystems.ToString()
                };
                button.Clicked += Close_Gate;
                Buttons.Children.Add(button);

                //myButton.Text = "Close Gate";
                //myButton.BackgroundColor = Color.Orange;
                //myButton.BorderColor = Color.Orange;
                //myButton.Clicked += Close_Gate;
                Buttons.IsEnabled = true;

                
                ErrorConnect.Text = "Please ensure that the system is safe for operation prior to closing the gate." + Environment.NewLine + Environment.NewLine + "No persons are to be inside the system and parked cars are to be parked correctly adhering to the relevant safety requirements.";
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
                ErrorConnect.TextColor = Color.OrangeRed;
                ErrorConnect.IsVisible = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
            }



            // Banned
            else if (CalledSpot[0] == Constants.Banned) 
            {
                await DisplayAlert("Locked", "The application administrator has locked you out from this service.", "OK");

                ErrorFrame.IsVisible = true;
                ErrorConnect.Text = "The application administrator has locked you out from this service.";
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.TextColor = Color.Red;
                ErrorConnect.IsEnabled = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
                return;
            }

            ////////////////////////////////////////////////////////////////////
            // 
            // TO ADD OTHER STATUS CODE FROM PLC FEEDBACK [ie stacker busy etc]
            //
            // "We were not able to call your spot. Please try again."
            //  else if (SpotCalled == "747")
            //
            //
            ////////////////////////////////////////////////////////////////////

            // Stacker Busy
            else if (CalledSpot[0] == Constants.Accepted)
            {
                await DisplayAlert("Hold on a second", "The system is being used at the moment. Please wait and try again shortly.", "OK");

                ErrorFrame.IsVisible = true;
                ErrorConnect.Text = "The system is busy at the moment. Please wait and try again shortly.";
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
                ErrorFrame.BackgroundColor = Color.Transparent;
                myButton.Text = "System is busy right now";

                // Delay for 7 seconds before re-enabling button
                await Task.Delay(10000);

                // Rebuild button with nickname (if enabled)
                // Nickname index is stored in button as ClassID
                if (Settings.SpotNickNameEnabled)
                {
                    string[] userNicks = Settings.SpotNickName.Split(new string[] { Constants.StringSplitRegex }, StringSplitOptions.None);
                    myButton.Text = userNicks[Int32.Parse(myButton.ClassId.Replace("True","").Replace("False",""))];
                }

                else
                {
                    //myButton.Text = "Spot " + spotnumber[0] + " System " + spotnumber[1];
                    myButton.Text = callMessage.Replace("Request ", "");
                }
                
                myButton.Clicked -= Clicked_Again;
                myButton.Clicked += Call_Spot;
                myButton.BackgroundColor = Color.FromHex("#00ff99");
                ErrorConnect.IsVisible = false;
                ErrorFrame.IsVisible = false;
                ErrorConnect.Text = "";
                ErrorFrame.BackgroundColor = Color.Transparent;
                Buttons.IsEnabled = true;
                return;

            }

            // Error connecting to host
            else if (CalledSpot[0] == Constants.Error)
            {
                await DisplayAlert("Uh oh", "An error occured during the process.", "OK");
                myButton.Text = "Stacker Error";
                myButton.BackgroundColor = Color.WhiteSmoke;
                Buttons.Children.Clear();

                
                ErrorConnect.Text = "Stacker Error" + Environment.NewLine + "Please contact support";
                ErrorConnect.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)));
                ErrorConnect.TextColor = Color.Black;
                ErrorConnect.IsVisible = true;
                ErrorFrame.IsVisible = true;
                ErrorFrame.BorderColor = Color.Red;
                SupportButton.IsEnabled = true;
                SupportButton.IsVisible = true;

                return;
            }

            else if (CalledSpot[0] == Constants.Custom)
            {
                await DisplayAlert(CalledSpot[2], CalledSpot[1], "OK");
                Buttons.IsEnabled = true;
                ErrorFrame.BackgroundColor = Color.Transparent;
                return;
            }


            // Other errors ie Access Token expired / incorrect [user must login again]
            else
            {
                await DisplayAlert("Error", "Unable to authenticate user. You will be logged out.", "OK");
                Clear_User();
                await Navigation.PopToRootAsync();
            }

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
            Settings.SubscribeTopic = "";
            Settings.SubscribeNotification = false;
            CrossFirebasePushNotification.Current.UnsubscribeAll();
            return;
        }

        async void EditProfile(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfile());
            return;
        }

        async void Clicked_Again(object sender, EventArgs e)
        {
            await DisplayAlert("Error","You have already called this spot. Please wait or log out and try again.", "OK");
            return;
        }

        async void Support_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SupportPage());
            return;

        }

        void ShowNotification(string title, string message)
        {
            
        }
    }


}
