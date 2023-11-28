using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WinServLite2.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            WinServLib.WinServ.Initialize($"Data Source=192.168.168.202\\\\WINSERV,1436;Initial Catalog=wsdb;User ID=wslite;Password=wslite;");
        }
    }
}