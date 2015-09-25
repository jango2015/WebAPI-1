
namespace MyConstants
{
    public static class Clients
    {
        public readonly static Client Client1 = new Client
        {
            Id = "123456",
            Secret = "abcdef",
            RedirectUrl = Paths.AuthorizeCodeCallBackPath
        };

    
        public readonly static Client Client2 = new Client
        {
            Id = "7890ab",
            Secret = "7890ab",
            //RedirectUrl = Paths.OpenIdConnectHybridCallBackPath
            RedirectUrl = Paths.AuthorizeImplicitCallBackPath

        };

        public readonly static Client Client3 = new Client
        {
            Id = "7890cd",
            Secret = "7890cd",
           // RedirectUrl = Paths.OpenIdConnectHybridCallBackPath
           // RedirectUrl = Paths.OpenIdConnectCodeCallBackPath
           // RedirectUrl = Paths.OpenIdConnectImplicitCallBackPath
           // RedirectUrl = Paths.OpenIdConnectJavascriptImplicitCallBackPath
            RedirectUrl = Paths.OpenIdConnectWpfHybridCallBackPath
        };
        
    }

    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public string RedirectUrl { get; set; }
    }
}