using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Diagnostics;

namespace EM_Webapi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        public override void Init()
        {
            base.Init();
            this.AcquireRequestState += showRouteValues;
        }

        protected void showRouteValues(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));

            if (routeData != null)
            {
                for (int i = 0; i < routeData.Values.Keys.Count; i++)
                    Debug.Write(string.Format(" {0}: {1}", routeData.Values.Keys.ElementAt(i), routeData.Values.Values.ElementAt(i)));
                Debug.WriteLine("");
            }
        }
    }
}
