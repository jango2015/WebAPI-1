using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;

[assembly: OwinStartup(typeof(UseFileServer.Startup))]

namespace UseFileServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            // display yellow page of death
            app.UseErrorPage(); 
#endif
            /**
            // Remap '/' to '.\defaults\'.
            //.\ goes to bin\debug
            //\ goes to c:\
            // Turns on static files and default files.
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@"..\..\defaults"),
            });
            **/

         
            // Remap '/public' to '.\defaults\'.
            // Turns on static files, directory browsing, and default files.
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = new PathString("/public"),
                FileSystem = new PhysicalFileSystem(@"..\..\defaults"),
                EnableDirectoryBrowsing = true,
            });
         

            /**
            // this is static file, one component of UseFileServer which has static files, browser
            // and static file only serves files with certain extensions
            // Only serve files requested by name.
            app.UseStaticFiles("/files");
            **/

            /**
            
           

            // Browse the root of your application (but do not serve the files).
            // NOTE: Avoid serving static files from the root of your application or bin folder,
            // it allows people to download your application binaries, config files, etc..
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                RequestPath = new PathString("/src"),
                FileSystem = new PhysicalFileSystem(@""),
            });
            **/

            // Using EmbededFileSystem
            // The folders intererpted as . so for style2.css 
            // the path is /embeded/Html.style.css and /embeded/Html.Page2.html
            // They are case sensitive
            app.UseFileServer(new FileServerOptions()
                    {
                        RequestPath = new PathString("/embeded"),
                        FileSystem = new EmbeddedResourceFileSystem(typeof(Startup).Assembly,
                            "UseFileServer.Assets"), //assembly and base namespace
                    }
                );

            // Anything not handled will land at the welcome page.
            app.UseWelcomePage();
        }
    }
}
