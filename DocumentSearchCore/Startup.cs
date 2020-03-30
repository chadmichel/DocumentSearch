using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using org.apache.http.conn.routing;

namespace DocumentSearchCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSpaStaticFiles(config => { config.RootPath = "../DocumentSearchClient"; });
            
            services.Add(new ServiceDescriptor(typeof(ILogger), new Logger()));
            services.Add(new ServiceDescriptor(typeof(IElasticAccess), typeof(ElasticAccess), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IDocumentManager), typeof(DocumentManager), ServiceLifetime.Transient));
            services.AddHostedService<BackgroundTimer>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../DocumentSearchClient";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            
            LogRoutes(actionDescriptorCollectionProvider);
        }

        private void LogRoutes(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            foreach (var route in actionDescriptorCollectionProvider.ActionDescriptors.Items)
            {
                var controller = route.RouteValues["Controller"];
                var action = route.RouteValues["Action"];
                var template = route.AttributeRouteInfo?.Template;
                Console.WriteLine($"Controller={controller} Action={action} Template={template}");
            }
        }
    }
}