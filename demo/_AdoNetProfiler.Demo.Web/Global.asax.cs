using System.Web.Mvc;
using System.Web.Routing;
using Glimpse.AdoNetProfiler;

namespace AdoNetProfiler.Demo.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AdoNetProfilerFactory.Initialize(typeof(GlimpseAdoNetProfiler));
        }
    }
}
