using System;
using System.Web;
using System.Web.Routing;
using SitemapXMLFilterService;

namespace SitemapXMLFilterService
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("filter", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(SitemapXMLService)));
            
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }
    }
}
