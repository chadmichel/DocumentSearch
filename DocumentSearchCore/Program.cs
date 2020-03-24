using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;

namespace DocumentSearchCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        
        
        public static ElasticClient CreateElasticClient()
        {
            // Use ElasticSearch to index the document
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("docs2");
            var client = new ElasticClient(settings);
            return client;
        }
    }
    
    
}