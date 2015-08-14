using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TheAgency.App_Start;
using System.Web.Http;
namespace TheAgency
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;
            config.MapHttpAttributeRoutes();
            RouteConfig.RegisterRoutes(config.Routes);
            config.Formatters.Add(new TheAgency.Formatters.AgentCSVFormatter());
            config.Formatters.JsonFormatter.Indent = true;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            config.EnsureInitialized();
        }
    }
}