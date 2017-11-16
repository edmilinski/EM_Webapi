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
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ApiSandbox",
                routeTemplate: "ApiSandbox",
                defaults: new { controller = "ApiSandbox" }
            );

            config.Routes.MapHttpRoute(
                name: "LogsDefault",
                routeTemplate: "logs",
                defaults: new { controller = "Logs"}
            );

            config.Routes.MapHttpRoute(
             name: "LogsByAction",
             routeTemplate: "logs/{category}/{action}/{param}",
             defaults: new {controller = "Logs" }
         );

            config.Routes.MapHttpRoute(
                name: "LogsByCategory",
                routeTemplate: "logs/{category}",
                defaults: new { controller = "Logs", action= "GetLogsByCategory" }
            );

            config.Routes.MapHttpRoute(
                name: "GetLogsTextByCategory",
                routeTemplate: "logs/{category}/text",
                defaults: new { controller = "Logs", action = "GetLogsTextByCategory" }
            );

            config.Routes.MapHttpRoute(
             name: "LogsById",
             routeTemplate: "logs/{category}/{id}",
             defaults: new { id = RouteParameter.Optional, controller = "Logs" }
         );

         config.Routes.MapHttpRoute(
             name: "GetFilteredTodos",
             routeTemplate: "todos/filtered",
             defaults: new { controller = "Todos", action = "GetFilteredTodos" }
         );

         config.Routes.MapHttpRoute(
             name: "GetFilteredTodosPage",
             routeTemplate: "todos/filteredpage/{page}/{count}",
             defaults: new { controller = "Todos", action = "GetFilteredTodosPage"}
         );

         config.Routes.MapHttpRoute(
             name: "TodosById",
             routeTemplate: "todos/{userid}/{id}",
             defaults: new {id = RouteParameter.Optional, controller = "Todos" }
         );

         config.Routes.MapHttpRoute(
             name: "TodosDefault",
             routeTemplate: "todos/",
             defaults: new { controller = "Todos" }
         );
         
      }
   }
}
