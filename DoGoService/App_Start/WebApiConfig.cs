using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using DoGoService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Net.Http.Headers;
using Parse;

namespace DoGoService
{
    [AllowAnonymous]
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            Database.SetInitializer(new MobileServiceInitializer());
            ParseClient.Initialize("ClCDxIalYQPR6IrVXUHtHQW99tazxTZOAFUnanLB", "XMg5TpHke8IRgNiEjY9kl7KbdRp6Ux9e6jPviy4x");
        }
    }

    public class MobileServiceInitializer : DropCreateDatabaseIfModelChanges<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            base.Seed(context);
        }
    }
}

