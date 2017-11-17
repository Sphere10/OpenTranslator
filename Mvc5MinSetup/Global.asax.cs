using System.Web.Mvc;
using System.Web.Routing;

using Mvc5MinSetup.App_Start;

namespace Mvc5MinSetup
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
