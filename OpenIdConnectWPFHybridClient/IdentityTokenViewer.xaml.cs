using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace OpenIdConnectWPFHybridClient
{
    /// <summary>
    /// Interaction logic for IdentityTokenViewer.xaml
    /// </summary>
    public partial class IdentityTokenViewer : Window
    {
        public IdentityTokenViewer()
        {
            InitializeComponent();
        }

        public string IdToken
        {
            set
            {
                var token = value;
                var parts = token.Split('.');
                // Header
                var part1 = Encoding.UTF8.GetString(Base64Url.Decode(parts[0]));
                var jwt1 = JObject.Parse(part1);
                Text1.Text = jwt1.ToString();

                //Body
                var part2 = Encoding.UTF8.GetString(Base64Url.Decode(parts[1]));
                var jwt2 = JObject.Parse(part2);
                Text2.Text = jwt2.ToString();

            }
        }

        public string AccessToken
        {
            set
            {
                var token = value;

                Text2.Text = token;

            }
        }
    }
}
