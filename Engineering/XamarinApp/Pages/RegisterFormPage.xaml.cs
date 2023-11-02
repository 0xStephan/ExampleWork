using System;
using System.Collections.Generic;
using System.Diagnostics;
using VSpaceParkers.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VSpaceParkers.Pages
{
    public partial class RegisterFormPage : ContentPage
    {
        private readonly ApiServices _apiServices = new ApiServices();
        private string received_spotid;
        private string query_string;

        public RegisterFormPage(string received_spotid, string query_string)
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Android)
            {
                Loading.Scale = 1.0;

                if ((2 * DeviceDisplay.MainDisplayInfo.Width) > DeviceDisplay.MainDisplayInfo.Height)
                {
                    // Minus 3 as each Entry have a StackLayout Parent Element of Padding = 3
                    Top.Padding = 15;
                    Top.Spacing = 10;
                    Elements.Padding = 15-3;
                    Elements.Spacing = 10-6;
                }
            }

            this.received_spotid = received_spotid;
            this.query_string = query_string;

            //var spotnumber = received_spotid.Split('_');
            int systemNumber = 1;

            if (received_spotid.Contains("_1"))
            {
                systemNumber = 1;
            }

            else if (received_spotid.Contains("_2"))
            {
                systemNumber = 2;
            }

            else if (received_spotid.Contains("_3"))
            {
                systemNumber = 3;
            }

            else if (received_spotid.Contains("_4"))
            {
                systemNumber = 4;
            }

            else if (received_spotid.Contains("_5"))
            {
                systemNumber = 5;
            }


            // Now lets extract only the spot data
            string spots_string = received_spotid.Replace("_1", "").Replace("_2", "").Replace("_3", "").Replace("_4", "").Replace("_5", "");
            var spotnumber = spots_string.Split(',');

            if (received_spotid.Length < 4)
            {
                if (systemNumber == 1)
                {
                    spotid.Text = "Spot: " + spotnumber[0];
                }

                else
                {
                    spotid.Text = "Spot: " + spotnumber[0] + " System: " + Constants.SystemLetter[systemNumber - 1];
                }
            }

            else
            {
                spotid.Text = "Spots: ";
                foreach(string spot in spotnumber)
                {
                    spotid.Text = spotid.Text + spot + ", ";
                }

                spotid.Text = spotid.Text.Remove(spotid.Text.Length - 2, 1);

                if (systemNumber != 1)
                {
                    spotid.Text = spotid.Text + "System: " + Constants.SystemLetter[systemNumber - 1];
                }
            }

            // Convert System 1 to System A etc
            /*
            int.TryParse(spotnumber[1], out systemNumber);

            if (spotnumber[1].Equals("1"))
            {
                spotid.Text = "Spot: " + spotnumber[0];
            }

            else
            {
                spotid.Text = "Spot: " + spotnumber[0] + " System: " + Constants.SystemLetter[systemNumber - 1];
            }
            */

            RegisterFormHeading.FontSize = (Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.25);
            RegisterFormHeading.FontAttributes = FontAttributes.Bold;
        }

        async void Submit_Clicked(object sender, EventArgs e)
        {
            SubmitButton.IsEnabled = false;

            if (Device.RuntimePlatform == Device.iOS)
            {
                FirstName.BackgroundColor = Color.Transparent;
                FirstName.PlaceholderColor = Color.Gray;

                LastName.BackgroundColor = Color.Transparent;
                LastName.PlaceholderColor = Color.Gray;

                Username.BackgroundColor = Color.Transparent;
                Username.PlaceholderColor = Color.Gray;

                Password.BackgroundColor = Color.Transparent;
                Password.PlaceholderColor = Color.Gray;

                ConfirmPassword.BackgroundColor = Color.Transparent;
                ConfirmPassword.PlaceholderColor = Color.Gray;

                Email.BackgroundColor = Color.Transparent;
                Email.PlaceholderColor = Color.Gray;

                PhoneNumber.BackgroundColor = Color.Transparent;
                PhoneNumber.PlaceholderColor = Color.Gray;

                Address.BackgroundColor = Color.Transparent;
                Address.PlaceholderColor = Color.Gray;

                Apartment.BackgroundColor = Color.Transparent;
                Apartment.PlaceholderColor = Color.Gray;

                CarRego.BackgroundColor = Color.Transparent;
                CarRego.PlaceholderColor = Color.Gray;
            }

            else if (Device.RuntimePlatform == Device.Android)
            {
                FirstNameBox.BackgroundColor = Color.Transparent;
                LastNameBox.BackgroundColor = Color.Transparent;
                UsernameBox.BackgroundColor = Color.Transparent;
                PasswordBox.BackgroundColor = Color.Transparent;
                ConfirmPasswordBox.BackgroundColor = Color.Transparent;
                EmailBox.BackgroundColor = Color.Transparent;
                PhoneNumberBox.BackgroundColor = Color.Transparent;
                AddressBox.BackgroundColor = Color.Transparent;
                ApartmentBox.BackgroundColor = Color.Transparent;
                CarRegoBox.BackgroundColor = Color.Transparent;
            }

            // First Name Validation
            if (string.IsNullOrWhiteSpace(FirstName.Text))
            {
                await DisplayAlert("Oh no!", "First Name must be 3 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    FirstName.BackgroundColor = Color.LightCoral;
                    FirstName.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    FirstNameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(FirstName.Text.Length);

            if (((FirstName.Text.Length < Constants.MinName) | (FirstName.Text.Length > Constants.MaxName)))
            {
                await DisplayAlert("Oh no!", "First Name must be 3 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    FirstName.BackgroundColor = Color.LightCoral;
                    FirstName.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    FirstNameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Last Name Validation
            if (string.IsNullOrWhiteSpace(LastName.Text))
            {
                await DisplayAlert("Oh no!", "Last Name must be 3 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    LastName.BackgroundColor = Color.LightCoral;
                    LastName.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    LastNameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(LastName.Text.Length);

            if (((LastName.Text.Length < Constants.MinName) | (LastName.Text.Length > Constants.MaxName)))
            {
                await DisplayAlert("Oh no!", "Last Name must be 3 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    LastName.BackgroundColor = Color.LightCoral;
                    LastName.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    LastNameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Username Validation
            if (Username.Text.Contains(" "))
            {
                await DisplayAlert("Oh no!", "Username cannot contain whitespace.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Username.BackgroundColor = Color.LightCoral;
                    Username.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    UsernameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Username Validation
            if (string.IsNullOrWhiteSpace(Username.Text))
            {
                await DisplayAlert("Oh no!", "Username must be 3 to 25 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Username.BackgroundColor = Color.LightCoral;
                    Username.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    UsernameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(Username.Text.Length);

            if (((Username.Text.Length < Constants.MinUsername) | (Username.Text.Length > Constants.MaxUsername)))
            {
                await DisplayAlert("Oh no!", "Username must be 3 to 25 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Username.BackgroundColor = Color.LightCoral;
                    Username.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    UsernameBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Password Validation
            if (string.IsNullOrWhiteSpace(Password.Text))
            {
                await DisplayAlert("Oh no!", "Password must be 6 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Password.BackgroundColor = Color.LightCoral;
                    Password.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    PasswordBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(Password.Text.Length);

            if (((Password.Text.Length < Constants.MinPWLenth) | (Password.Text.Length > Constants.MaxPWLength)))
            {
                await DisplayAlert("Oh no!", "Password must be 6 to 50 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Password.BackgroundColor = Color.LightCoral;
                    Password.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    PasswordBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Confirm Password Validation
            if (string.IsNullOrWhiteSpace(ConfirmPassword.Text))
            {
                await DisplayAlert("Oh no!", "Password and Confirm Password must match.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    ConfirmPassword.BackgroundColor = Color.LightCoral;
                    ConfirmPassword.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    ConfirmPasswordBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            if (!(ConfirmPassword.Text.Equals(Password.Text)))
            {
                await DisplayAlert("Oh no!", "Password and Confirm Password must match.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    ConfirmPassword.BackgroundColor = Color.LightCoral;
                    ConfirmPassword.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    ConfirmPasswordBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Email Address Validation
            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                await DisplayAlert("Oh no!", "Email cannot be empty.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Email.BackgroundColor = Color.LightCoral;
                    Email.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    EmailBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            if (Email.Text.Length > 0)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(Email.Text);
                    if (!(addr.Address.ToString().Equals(Email.Text)))
                    {
                        await DisplayAlert("Oh no!", "Email address is not valid.", "OK");

                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            Email.BackgroundColor = Color.LightCoral;
                            Email.PlaceholderColor = Color.White;
                        }

                        else if (Device.RuntimePlatform == Device.Android)
                        {
                            EmailBox.BackgroundColor = Color.Red;
                        }

                        SubmitButton.IsEnabled = true;
                        return;
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message.ToString());
                    await DisplayAlert("Oh no!", "Email address is not valid.", "OK");

                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        Email.BackgroundColor = Color.LightCoral;
                        Email.PlaceholderColor = Color.White;
                    }

                    else if (Device.RuntimePlatform == Device.Android)
                    {
                        EmailBox.BackgroundColor = Color.Red;
                    }

                    SubmitButton.IsEnabled = true;
                    return;
                }
            }

            // Phone Number Validation
            if (string.IsNullOrWhiteSpace(PhoneNumber.Text))
            {
                await DisplayAlert("Oh no!", "Phone number must be 8 to 25 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    PhoneNumber.BackgroundColor = Color.LightCoral;
                    PhoneNumber.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    PhoneNumberBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(PhoneNumber.Text.Length);

            if (((PhoneNumber.Text.Length < Constants.MinPhoneNumber) | (PhoneNumber.Text.Length > Constants.MaxPhoneNumber)))
            {
                await DisplayAlert("Oh no!", "Phone number must be 8 to 25 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    PhoneNumber.BackgroundColor = Color.LightCoral;
                    PhoneNumber.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    PhoneNumberBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Apartment Validation
            if (string.IsNullOrWhiteSpace(Apartment.Text))
            {
                await DisplayAlert("Oh no!", "Apartment cannot be empty.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Apartment.BackgroundColor = Color.LightCoral;
                    Apartment.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    ApartmentBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(Apartment.Text.Length);

            if (((Apartment.Text.Length > Constants.MaxApartment)))
            {
                await DisplayAlert("Oh no!", "Apartment cannot be longer than 100 characters.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Apartment.BackgroundColor = Color.LightCoral;
                    Apartment.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    ApartmentBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Address Validation
            if (string.IsNullOrWhiteSpace(Address.Text))
            {
                await DisplayAlert("Oh no!", "Address must be 3 to 100 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Address.BackgroundColor = Color.LightCoral;
                    Address.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    AddressBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(Address.Text.Length);

            if (((Address.Text.Length < Constants.MinApartment) | (Address.Text.Length > Constants.MaxApartment)))
            {
                await DisplayAlert("Oh no!", "Address must be 3 to 100 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    Address.BackgroundColor = Color.LightCoral;
                    Address.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    AddressBox.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            // Car Rego Validation
            if (string.IsNullOrWhiteSpace(CarRego.Text))
            {
                await DisplayAlert("Oh no!", "Vehicle Registration cannot be empty.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    CarRego.BackgroundColor = Color.LightCoral;
                    CarRego.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    CarRego.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Debug.WriteLine(CarRego.Text.Length);

            if (((CarRego.Text.Length < Constants.MinCarrego) | (CarRego.Text.Length > Constants.MaxCarrego)))
            {
                await DisplayAlert("Oh no!", "Vehicle Registration must be 1 to 25 characters long.", "OK");

                if (Device.RuntimePlatform == Device.iOS)
                {
                    CarRego.BackgroundColor = Color.LightCoral;
                    CarRego.PlaceholderColor = Color.White;
                }

                else if (Device.RuntimePlatform == Device.Android)
                {
                    CarRego.BackgroundColor = Color.Red;
                }

                SubmitButton.IsEnabled = true;
                return;
            }

            Loading.IsEnabled = true;
            Loading.IsVisible = true;
            Loading.IsRunning = true;

            // If we got here we have valid data

            var jsonData = new Dictionary<string, string>
            {
                { "username", Username.Text.ToLower() },
                { "name", FirstName.Text + " " + LastName.Text },
                { "password", Password.Text },
                { "spotid", this.received_spotid },
                { "email", Email.Text },
                { "phonenumber", PhoneNumber.Text },
                { "address", Address.Text },
                { "apartment", Apartment.Text },
                { "carrego", CarRego.Text },
            };

            var result = await _apiServices.RegisterStep2(this.query_string, jsonData);

            // Account successfully created
            if (result == Constants.OK)
            {
                await DisplayAlert("Great success!", "Your account has been successfully created.", "OK");
                await Navigation.PopToRootAsync();
                return;

            }

            else if (result == Constants.UserExists)
            {
                await DisplayAlert("Oh no!", "The username: " + Username.Text + " is already registered. Please try a different username.", "OK");
                Loading.IsEnabled = false;
                Loading.IsVisible = false;
                Loading.IsRunning = false;
                SubmitButton.IsEnabled = true;
                return;
            }

            else if (result ==  Constants.Offline)
            {
                await DisplayAlert("You're offline", "There was an error connecting to the network. Please reconnect and try again.", "OK");
                await Navigation.PopToRootAsync();
                return;
            }

            else
            {
                await DisplayAlert("Something's not right", "There was an error during the registration process. Please contact support or try again.", "OK");
                Loading.IsEnabled = false;
                Loading.IsVisible = false;
                Loading.IsRunning = false;
                SubmitButton.IsEnabled = true;
                return;
            }

        }
    }

    
}
