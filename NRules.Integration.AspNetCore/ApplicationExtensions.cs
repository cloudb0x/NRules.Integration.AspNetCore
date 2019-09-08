using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent.Dsl;
using NRules.Fluent;
using System.Linq;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Integration.AspNetCore
{
    public static class ApplicationExtensions
    {

        public static IApplicationBuilder UseNRules(this IApplicationBuilder app)
        {
            var repo = app.ApplicationServices.GetService<RuleRepository>();
            var factory = app.ApplicationServices.GetService<ISessionFactory>();

            if (factory == null || repo == null)
                throw new InvalidOperationException("Dependencies not registered. Call AddNRules in Startup.cs");

            repo.Activator = new AspNetCoreRuleActivator(app.ApplicationServices);
            factory.DependencyResolver = new AspNetCoreDependencyResolver(app.ApplicationServices);

            return app;
        }



    }
}
