
# NRules.Integration.AspNetCore

[![Build Status](https://dev.azure.com/claudiosteppe/NRules.Integration.AspNetCore/_apis/build/status/cloudb0x.NRules.Integration.AspNetCore?branchName=master) ![Nuget](https://img.shields.io/nuget/v/NRules.Integration.AspNetCore)

An Integration For Nrules for Asp .Net Core for the built in Dependency Injection container.


 ## Getting Started

Adding the integration is as straightforward as calling two extension methods in your `Startup.cs` class.

Startup.cs 

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNRule(AppDomain.CurrentDomain.GetAssemblies()) // Loads Rules from assemblies and Registers NRule with DI Container
                    .AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseNRule(); //Add the configured DI Container as the dependency resolver for Nrules
            app.UseHttpsRedirection();
            app.UseMvc();
        }

## Usage

Simply Injecting `ISession` into your dependency is enough. This Calls the `ISessionFactory` and creates a new scoped session.

    public class SomeService : ISomeService
    {
        private readonly ISession _session;

        public SomeService(ISession session)
        {
            _session = session;
        }

        public void DoSomethingWithSession()
        {
            var fact = "I Am a Fact";

            _session.Insert(fact);

            _session.Fire();

        }
    }

