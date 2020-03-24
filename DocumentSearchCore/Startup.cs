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

            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            // });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "../DocumentSearchClient";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            
            var r = actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Select(x => new RouteInfo {
                    Action = x.RouteValues["Action"],
                    Controller = x.RouteValues["Controller"],
                    Name = x.AttributeRouteInfo?.Name,
                    Template = x.AttributeRouteInfo?.Template,
                    Constraint = x.ActionConstraints == null ? "" : JsonConvert.SerializeObject(x.ActionConstraints)
                })
                .OrderBy(r => r.Template)
                .ToList();


            foreach (var route in r)
            {
                Console.WriteLine("c " + route.Controller + " " + route.Action + " " + route.Template);
            }
        }
    }
    
    public class RouteInfo
    {
        public string Template { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Constraint { get; set; }
    }
}