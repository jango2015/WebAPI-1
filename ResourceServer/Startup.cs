﻿
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ResourceServer.Startup))]

namespace ResourceServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigWebAPI(app);

        }
    }
}