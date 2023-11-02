using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VSpaceParkers.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using System.Text;
using Xamarin.Forms;
using VSpaceParkers;

namespace VSpaceParkers.Services
{
    internal class ApiServices
    {
        private readonly Constants _constants = new Constants();
        private int recallcount = 0;

        public async Task<bool> OnlineCheck()
        {
            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);
            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";
            var timeout = Task.Delay(10000);
            var send = client.GetAsync(BaseApiAddress + "online");

            await Task.WhenAny(timeout, send);

            if (timeout.IsCompleted)
            {
                Debug.WriteLine("Time out 1 complete");

                // Try 2nd Attempt
                var timeout2 = Task.Delay(5000);
                send = client.GetAsync(BaseApiAddress + "online");

                await Task.WhenAny(timeout2, send);

                if (timeout2.IsCompleted)
                {
                    var service = DependencyService.Get<IWifiConnect>();
                    service.SetDisconnected();
                    return false;
                }

                try
                {
                    var response2 = await send;

                    if (response2.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }

                catch (Exception e2)
                {
                    Debug.WriteLine(e2.Message.ToString());
                    return false;
                }

                
            }

            try
            {
                var response = await send;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.Message.ToString());
                return false;
            }

        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            Debug.WriteLine("Start Login");
            var authData = string.Format("{0}:{1}", userName.ToLower(), password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            
            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            Debug.WriteLine("Device GUID: ");
            Debug.WriteLine(Settings.GUID.ToString());
            Debug.WriteLine(browser.ToString());

            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";

            Debug.WriteLine(BaseApiAddress);

            var timeout = Task.Delay(15000);
            var send = client.GetAsync(BaseApiAddress + "login");

            await Task.WhenAny(timeout, send);

            if (timeout.IsCompleted)
            {
                Debug.WriteLine("Time out complete");
                return Constants.Offline;
            }

            try
            {
                var response = await send;

                if (response.StatusCode.GetHashCode() == 401)
                {
                    Debug.WriteLine("Unauth");
                    return Constants.Unauth;
                }

                else if (response.StatusCode.GetHashCode() == 403)
                {
                    return Constants.Banned;
                }

                else if (!(response.IsSuccessStatusCode))
                {
                    Debug.WriteLine(response.StatusCode.ToString());
                    return Constants.Offline;
                }

                Debug.WriteLine(response.IsSuccessStatusCode);

                var data = await response.Content.ReadAsStringAsync();

                JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(data);
                var accessToken = jwtDynamic.Value<string>("token");
                Debug.WriteLine("Before Token");
                Debug.WriteLine(accessToken);
                Debug.WriteLine("After Token");
                Debug.WriteLine(DateTime.Now.ToString());
                Settings.AccessToken = accessToken;
                //Settings.AccessTokenExpirationDate = DateTime.Now.AddDays(13);
                return accessToken;
            }

            /*
            catch (HttpRequestException)
            {
                Debug.WriteLine("Request Exception");
                return Constants.Offline;
                
            }
            */

            catch (Exception e)
            {
                Debug.WriteLine(e.Message.ToString());
                Debug.WriteLine(e.InnerException.Message.ToString());
                Debug.WriteLine("Other Issue");
                return Constants.Offline;
            }

        }

        public async Task<string> GetUserDetails(string accessToken)
        {
            // Allow self signed SSL
            //var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("x-access-token", accessToken);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            // ADD HTTPS
            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";
            var data = await client.GetStringAsync(BaseApiAddress + "api/values");

            Debug.WriteLine("Device GUID: ");
            Debug.WriteLine(Settings.GUID.ToString());
            Debug.WriteLine(browser.ToString());

            Debug.WriteLine("GetSpotID");
            Debug.WriteLine(data);

            try
            {
                JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(data);
                var receivedSpotID = jwtDynamic.Value<string>("SpotID");
                var receivedName = jwtDynamic.Value<string>("Name");
                var receivedSpotNickName = jwtDynamic.Value<string>("SpotNickName");
                var mainGate = jwtDynamic.Value<string>("MainGate");
                var supportButton = jwtDynamic.Value<bool>("Support");
                var receivedSite = jwtDynamic.Value<string>("Site");
                var receivedUsername = jwtDynamic.Value<string>("Username");
                var receivedApartment = jwtDynamic.Value<string>("Apartment");

                Debug.WriteLine("#----------------------------#");
                Debug.WriteLine("MainGate Value:");
                Debug.WriteLine(mainGate.ToString());

                // Google Pixel was throwing error trying
                //          to string split SPACE on name

                try
                {
                    string[] firstName = receivedName.ToString().Split(' ');

                    if (firstName.Length == 0)
                    {
                        Settings.FullName = "";
                    }

                    else
                    {
                        Settings.FullName = firstName[0];
                    }
                    
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message.ToString());
                    Settings.FullName = "";
                }
                
               

                Settings.SpotID = receivedSpotID;

                // Show firstname instead of fullname 
                //Settings.FullName = receivedName;

                Settings.SpotNickName = receivedSpotNickName;
                Settings.SpotNickNameEnabled = false;
                Settings.SupportButton = supportButton;
                //Settings.MainGate = mainGate;
                Settings.CurrentSite = receivedSite;
                Settings.Username = receivedUsername;
                Settings.Apartment = receivedApartment;
                
                if (!(receivedSpotNickName.Equals("none")))
                {
                    Settings.SpotNickNameEnabled = true;
                }

                
                if (mainGate.Equals("True") | mainGate.Equals("true"))
                {
                    Settings.MainGate = true;
                }
                

                else
                {
                    Settings.MainGate = false;
                }

                // Catch for sites that do not have remote notifications enabled
                try
                {
                    
                    var receivedTopic = jwtDynamic.Value<string>("Topic");

                    if (String.IsNullOrEmpty(receivedTopic))
                    {
                        Settings.SubscribeNotification = false;
                        System.Diagnostics.Debug.WriteLine("This site DOES NOT has remote notifications enabled");
                    }

                    else
                    {
                        Settings.SubscribeNotification = true;
                        Settings.SubscribeTopic = receivedTopic;
                        System.Diagnostics.Debug.WriteLine("This site has remote notifications enabled");
                        System.Diagnostics.Debug.WriteLine("Notification Topic: " + receivedTopic);
                    }
                    
                }

                catch
                {
                    Settings.SubscribeNotification = false;
                    System.Diagnostics.Debug.WriteLine("This site DOES NOT has remote notifications enabled");
                }


                return Constants.OK;
            }

            catch
            {
                return Constants.Error;
            }

            
        }

        public async Task<string[]> CallSpot(string accessToken, string spotID, int cycleType)
        {
            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }


            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("x-access-token", accessToken);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);


            //client.DefaultRequestHeaders.UserAgent.ParseAdd(browser);

            Debug.WriteLine("Device GUID: ");
            Debug.WriteLine(Settings.GUID.ToString());
            Debug.WriteLine(browser.ToString());

            // HTTPS
            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";
            //var send = client.GetStringAsync(BaseApiAddress + "api/values/" + spotID + "/" + cycleType);
            var send = client.GetAsync(BaseApiAddress + "api/values/" + spotID + "/" + cycleType);

            var timeout = Task.Delay(15000);
            await Task.WhenAny(timeout, send);

            string[] output = { "", "", "" };

            if (timeout.IsCompleted)
            {
                Debug.WriteLine("Timeout Complete");
                output[0] = Constants.Offline;
                return output;
            }


            try
            {
                var response = await send;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Our token has expired. Force user to logout.
                    output[0] = Constants.Unauth;
                    return output;

                }

                var json = await response.Content.ReadAsStringAsync();

                /*
                Debug.WriteLine("Inside Try");
                Debug.WriteLine("StatusCode:");
                Debug.WriteLine(response.StatusCode.ToString());
                Debug.WriteLine("HashCode");
                Debug.WriteLine(send.Status.GetHashCode().ToString());
                Debug.WriteLine(send.ToString());
                Debug.WriteLine(json.ToString());
                */


                JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(json);
                Debug.WriteLine(jwtDynamic.ToString());

                var Status = jwtDynamic.Value<string>("Status");

                output[0] = Status;

                if ((Status == Constants.Custom) | (Status == Constants.SessionClose))
                {
                    output[1] = jwtDynamic.Value<string>("Message");
                    output[2] = jwtDynamic.Value<string>("MessageTitle");
                }

                return output;
            }

            catch
            {
                Debug.WriteLine("Inside Catch");
                Debug.WriteLine(send.Status.GetHashCode().ToString());
                Debug.WriteLine(send.ToString());
                Debug.WriteLine("Status:");
                Debug.WriteLine(send.Status.ToString());

                try
                {
                    if (recallcount >= 5)
                    {
                        output[0] = Constants.Offline;
                        recallcount = 0;
                        return output;
                    }
                    recallcount++;

                    output = await CallSpot(accessToken, spotID, cycleType);
                    

                }

                catch
                {
                    output[0] = Constants.Offline;
                }
                
                return output;
            }

        }

        public async Task<string> ChangePassword(string accessToken, string oldPassword, string newPassword)
        {
            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("x-access-token", accessToken);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString); 
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            var jsonData = new Dictionary<string, string>
            {
                { "old_password", oldPassword },
                { "new_password", newPassword }
            };

            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";
            var request = new HttpRequestMessage(
                HttpMethod.Post, BaseApiAddress + "api/Account/ChangePassword");

            var json = JsonConvert.SerializeObject(jsonData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var send = client.SendAsync(request);

            var timeout = Task.Delay(10000);
            await Task.WhenAny(timeout, send);

            if (timeout.IsCompleted)
            {
                return Constants.Offline;
            }

            try
            {
                var response = await send;
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode.GetHashCode() == 401)
                {
                    Debug.WriteLine("Unauth");
                    return Constants.Unauth;
                }

                else if (response.IsSuccessStatusCode)
                {
                    return Constants.OK;
                }

                else
                {
                    try
                    {
                        JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content);
                        Debug.WriteLine(jwtDynamic.ToString());
                        var message = jwtDynamic.Value<string>("message");

                        if (message.Equals("Token is invalid!"))
                        {
                            Debug.WriteLine("Invalid password");
                            return Constants.Unauth;
                        }

                        Debug.WriteLine("Bad new Password");
                        return Constants.BadPassword;
                    }

                    catch
                    {
                        Debug.WriteLine("Catch Error");
                        Debug.WriteLine(content.ToString());
                        return Constants.Error;
                    }
                }
            }

            catch
            {
                return Constants.Offline;
            }

        }

        public async Task<string> ChangeSpotNickName(string accessToken, string SpotNickNameStr)
        {
            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("x-access-token", accessToken);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            var jsonData = new Dictionary<string, string>
            {
                { "new_nicknames", SpotNickNameStr }
            };

            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";
            var request = new HttpRequestMessage(
                HttpMethod.Post, BaseApiAddress + "api/Account/ChangeSpotNickName");

            var json = JsonConvert.SerializeObject(jsonData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var send = client.SendAsync(request);

            // Test web communication twice (with relevant timeouts)
            // If both fails then return Offline code

            var timeout = Task.Delay(10000);
            await Task.WhenAny(timeout, send);

            if (timeout.IsCompleted)
            {
                return Constants.Offline;
            }

            try
            {
                var response = await send;
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Success Status Code changing SpotNick");
                    return Constants.OK;
                }

                Debug.WriteLine(content.ToString());
                return Constants.Error;
            }

            catch
            {
                return Constants.Offline;
            }

        }

        public async Task<string> RegisterStep1(string qr_text)
        {
            Debug.WriteLine(qr_text);

            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";

            var timeout = Task.Delay(10000);
            var send = client.GetAsync(BaseApiAddress + "api/register/" + qr_text);

            await Task.WhenAny(timeout, send);

            if (timeout.IsCompleted)
            {
                Debug.WriteLine("Time out complete");
                return Constants.Offline;
            }

            try
            {
                var response = await send;
                if (response.IsSuccessStatusCode)
                {
                    var received = await response.Content.ReadAsStringAsync();
                    JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(received);
                    var spotid = jwtDynamic.Value<string>("SpotID");
                    Debug.WriteLine(spotid);

                    return spotid;

                }

                else if (response.StatusCode.GetHashCode() == 409)
                {
                    return Constants.UserExists;
                }

                else
                {
                    return Constants.Error;
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                return Constants.Error;
            }

        }

        public async Task<string> RegisterStep2(string qr_text, Dictionary<string, string> jsonData)
        {
            // Allow self signed SSL
            var httpClientHandler = new HttpClientHandler();

            if (Device.RuntimePlatform == Device.Android)
            {
                httpClientHandler = DependencyService.Get<IHTTPClientHandlerCreationService>().GetInsecureHandler();
            }

            else
            {
                //httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }

            var client = new HttpClient(httpClientHandler);

            var browser = DeviceInfo.Platform + " " + DeviceInfo.VersionString;

            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "User-Agent", browser);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "App-Version", AppInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
            "GUID", Settings.GUID);

            var BaseApiAddress = "https://" + Settings.BaseIPAddress + ":" + Settings.BasePortNumber + "/";

            var request = new HttpRequestMessage(
                HttpMethod.Post, BaseApiAddress + "api/register/" + qr_text);

            var json = JsonConvert.SerializeObject(jsonData);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var send = client.SendAsync(request);

            var timeout = Task.Delay(10000);
            await Task.WhenAny(timeout, send);


            var result = await send;

            if (timeout.IsCompleted)
            {
                Debug.WriteLine("Time out complete");
                return Constants.Offline;
            }

            Debug.WriteLine(result.StatusCode.ToString());

            if (result.IsSuccessStatusCode)
            {
                Settings.AccessToken = "";
                Settings.SpotID = "";
                return Constants.OK;
            }

            else if (result.StatusCode.GetHashCode() == 409)
            {
                return Constants.UserExists;
            }

            else
            {
                
                return Constants.Error;
            }

        }
    }
}
