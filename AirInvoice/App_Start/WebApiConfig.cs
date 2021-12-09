using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Formatting;

namespace AirlineInvoice
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            //config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            //config.Formatters.XmlFormatter.UseXmlSerializer = true;
            //config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            //config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "text/xml");
            //config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "application/xml");
            //config.Formatters.Add(config.Formatters.XmlFormatter);
            
            //config.EnableSystemDiagnosticsTracing();

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}