using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EM_Webapi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "LogsByAction",
                routeTemplate: "api/{controller}/{category}/{action}/{param}"
            );

            config.Routes.MapHttpRoute(
                name: "LogsById",
                routeTemplate: "api/{controller}/{category}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

/*            config.Routes.MapHttpRoute(
                name: "DefaultByAction",
                routeTemplate: "api/{controller}/{action}/{param}"
            );
*/
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
