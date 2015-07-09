using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using MyConstants;
using System.Net.Http;
using Newtonsoft.Json.Linq;


namespace OpenIdConnectWPFHybridClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginWebView _login;

        AuthorizeResponse _response;

        JObject payload;

        public MainWindow()
        {
            InitializeComponent();

            _login = new LoginWebView();
            _login.Done += _login_Done;

            Loaded += MainWindow_Loaded;

        }

        private void _login_Done(object sender, AuthorizeResponse e)
        {
            _response = e;
            Textbox1.Text = e.Raw;
        }

        #region event handlers

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _login.Owner = this;
        }

        private void LoginWithProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // To get code
            RequestCode("openid profile", "code id_token");
        }

        private void LoginWithProfileAndAccessTokenButton_Click(object sender, RoutedEventArgs e)
        {
            //To get code
            RequestCode("openid profile read write offline_access", "code id_token token");
        }

        private async void UseCodeButton_Click(object sender, RoutedEventArgs e)
        {
            //To get token
            if (_response != null && _response.Values.ContainsKey("code"))
            {
                var response = await RequestToken(_response.Code);

                payload = JObject.Parse(response);

                Textbox1.Text = response;
            }
          
        }

        private void CallUserInfo_Click(object sender, RoutedEventArgs e)
        {
            /**
             *  var client = new HttpClient
            {
                BaseAddress = new Uri(Constants.UserInfoEndpoint)
            };

            if (_response != null && _response.Values.ContainsKey("access_token"))
            {  
                client.SetBearerToken(_response.AccessToken);
            }

            var response = await client.GetAsync("");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                Textbox1.Text = JObject.Parse(json).ToString();
            }
            else
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
             * **/

            MessageBox.Show("Not implemented");
        }

        private void ShowIdTokenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_response.Values.ContainsKey("id_token"))
            {
                var viewer = new IdentityTokenViewer();
                viewer.IdToken = _response.Values["id_token"];
                viewer.Show();
            }
        }

        private void ShowAccessTokenButton_Click(object sender, RoutedEventArgs e)
        {
            if (payload !=null && payload.SelectToken("access_token") !=null)
            {
                var viewer = new IdentityTokenViewer();
                viewer.AccessToken = payload.SelectToken("access_token").ToString();
                viewer.Show();
            }
        }

        private async void CallServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (payload != null && payload.SelectToken("access_token") != null)
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                        Paths.ResourceServerOpenIdConnectBaseAddress + Paths.APIPath);


                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                        payload.SelectToken("access_token").ToString());

                    var response = await client.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    Textbox1.Text = await response.Content.ReadAsStringAsync();
                }
            }
        }

        private void ValidateIdTokenButton_Click(object sender, RoutedEventArgs e)
        {
            /**
             *  if (_response != null && _response.Values.ContainsKey("id_token"))
            {
                var client = new HttpClient();

                var response = await client.GetAsync(Constants.IdentityTokenValidationEndpoint + "?token=" + _response.Values["id_token"] + "&client_id=hybridclient");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Textbox1.Text = JObject.Parse(json).ToString();
                }
                else
                {
                    MessageBox.Show(response.StatusCode.ToString());
                }
            }
             * **/

            MessageBox.Show("Not implemented");
        }
        #endregion event handlers

        #region helpers

        private void RequestCode(string scope, string responseType)
        {
            // Redirect to authorization server

            var url = CreateAuthorizeUrl(
                // Authorize endpoint 
                Paths.OpenIdConnectServerBaseAddress + Paths.OpenIdAuthorizePath,
                Clients.Client3.Id,
                //No client secret for the first leg of hybrid  
                "code id_token",
                //Scope
                "openid profile read",
                //Redirectedurl
                Clients.Client3.RedirectUrl,
                //State
                "123",
                //nonce
                "should_be_random"
                );

            _login.Show();
            _login.Start(new Uri(url), new Uri(Clients.Client3.RedirectUrl));

        }

        //Requesting code does not need client secret, secret only wanted when exchanging code for token
        private string CreateAuthorizeUrl(string authorizeEndpoint, string clientId, string responseType,
            string scope, string redirectUri, string state = null, string nonce = null)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("response_type", responseType);
            segments.Add("scope", scope);
            segments.Add("redirect_uri", redirectUri);

            if (!string.IsNullOrWhiteSpace(state))
            {
                segments.Add("state", state);
            }

            if (!string.IsNullOrWhiteSpace(nonce))
            {
                segments.Add("nonce", nonce);
            }

            var qs = string.Join("&", segments.Select(kvp => String.Format("{0}={1}", WebUtility.UrlEncode(kvp.Key), WebUtility.UrlEncode(kvp.Value))).ToArray());
            return string.Format("{0}?{1}", authorizeEndpoint, qs);
        }


        private async Task<string> RequestToken(string code)
        {
            //Request for access token 
            using (var client = new HttpClient())
            {
              
                var request = new HttpRequestMessage(HttpMethod.Post,
                    Paths.OpenIdConnectServerBaseAddress + Paths.OepnIdTokenPath);
                var tokenRequestElements = CreateTokenRequestElements(
                      Clients.Client3.Id,
                      Clients.Client3.Secret,
                      "authorization_code",
                      code,
                      Clients.Client3.RedirectUrl
                    );

                request.Content = new FormUrlEncodedContent(tokenRequestElements);

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
         

            }
        }

        //Requesting token 
        private Dictionary<string, string> CreateTokenRequestElements(string clientId, string clientSecret,
            string grantType, string code, string redirectUri)
        {
            var segments = new Dictionary<string, string>();
            segments.Add("client_id", clientId);
            segments.Add("client_secret", clientSecret);
            segments.Add("grant_type", grantType);
            segments.Add("code", code);
            segments.Add("redirect_uri", redirectUri);

            return segments;
        }


        #endregion helpers
    }
}
