using System;
using System.Collections.Generic;
using System.Windows;


namespace OpenIdConnectWPFHybridClient
{
    /// <summary>
    /// Interaction logic for LoginWebView.xaml
    /// </summary>
    public partial class LoginWebView : Window
    {
        public event EventHandler<AuthorizeResponse> Done;
        Uri _callbackUri;

        public LoginWebView()
        {
            InitializeComponent();

            webView.Navigating += webView_Navigating;

            Closing += LoginWebView_Closing;
        }


        public void Start(Uri startUri, Uri callbackUri)
        {
            _callbackUri = callbackUri;
            webView.Navigate(startUri);
        }

        #region event handlers

        private void webView_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri.ToString().StartsWith(_callbackUri.AbsoluteUri))
            {
                var response = new AuthorizeResponse(e.Uri.AbsoluteUri);
                
                e.Cancel = true;
                this.Visibility = System.Windows.Visibility.Hidden;

                if (Done != null)
                {

                    Done.Invoke(this, response);
                }
            }
        }

        private void LoginWebView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        #endregion event handlers
        #region helpers

       
        #endregion helpers
    }
}
