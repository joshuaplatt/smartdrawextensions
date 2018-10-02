using System;
using System.Web;
using System.Web.Routing;
using ERDFilterService;

namespace ERDFilterService
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new System.ServiceModel.Activation.ServiceRoute("filter", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(ERDFilterService)));
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
