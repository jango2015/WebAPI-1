using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationFunction = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;


namespace KatanaBasics
{
    /**
     * This is a component, that implements Application Function AppFunc which is invoked externally
     * 
     * A component is also called middleware
     * The components form a chain of actions where each component calls upon the next component’s application function
     * in the pipeline using the data in the Environment dictionary 
     * 
     * **/
    public class WelcomeComponent
    {

        private readonly ApplicationFunction _nextComponent;
        // The argument is the AppFunc in the invocation chain
       
        public WelcomeComponent(ApplicationFunction appFunc)
        {
            if (appFunc==null)
                throw new ArgumentNullException("AppFunc of next component");
            _nextComponent = appFunc;
        }

        //The methods we tested with IAppBuilder, i.e. Run and UseWelcomePage, use the same technique behind the scenes. They are simply wrappers around a much more elaborate Invoke method. 
        /**
         * 
         * public static class AppBuilderExtensions
{
    public static void UseWelcomeComponent(this IAppBuilder appBuilder)
    {
        appBuilder.Use<WelcomeComponent>();
    }
}

         * 
         * **/
        public async Task Invoke(IDictionary<string, object> environment)
        {
             await _nextComponent(environment);
        }

    }
}
