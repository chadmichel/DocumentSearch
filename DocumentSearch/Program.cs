using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Nest;
using TikaOnDotNet.TextExtraction;
using System.Web.Http.SelfHost;
using com.sun.jmx.remote.protocol.iiop;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentSearch
{
    internal class Program
    {
        public static IServiceProvider serviceProvider;
        public static IConfiguration Config { get; set; }
        
        public async static Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Config = builder.Build();

            var collection = new ServiceCollection();
            
            collection.AddScoped(typeof(ILogger), typeof(Logger));
            collection.AddScoped(typeof(IElasticAccess), typeof(ElasticAccess));
            collection.AddScoped(typeof(IDocumentManager), typeof(DocumentManager));
            serviceProvider = collection.BuildServiceProvider();
            
            Console.WriteLine("Elastic:" + Config["ElasticUrl"]);
            
            // Check the command line parameters
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Pass in the folder to index");
                return;
            }
            
            var config = new HttpSelfHostConfiguration("http://localhost:5150");
            //config.MessageHandlers.Add(new CustomHeaderHandler());
            config.EnableCors(new EnableCorsAttribute("*", headers: "*", methods: "*"));
            
            
            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}", 
                new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                "Static", "{*url}",
                new { controller = "StaticFiles", action = "Index" });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                //BuildIndex(args[0]);
                await serviceProvider.GetService<IDocumentManager>().BuildIndex(args[0]);
                
                server.OpenAsync().Wait();

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }

        public static ElasticClient CreateElasticClient()
        {
            // Use ElasticSearch to index the document
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("docs2");
            var client = new ElasticClient(settings);
            return client;
        }
        
        static async Task BuildIndex(string startPath)
        {
            Console.WriteLine("Indexes Directory: " + startPath);
            
            // Use TikaOnDotNet to extract the contents of the document.
            var textExtractor = new TextExtractor();
            
            // Use ElasticSearch to index the document
            var client = CreateElasticClient();

            // Loop through all files in the passed in directory
            var directory = new DirectoryInfo(startPath);
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles())
                {
                    var contents = textExtractor.Extract(file.FullName);
                    if (!string.IsNullOrWhiteSpace(contents.Text))
                    {

                        Console.WriteLine("Indexing File " + file.FullName);

                        var id = HashUtility.Hash(file.FullName);
                        
                        var doc = new Doc()
                        {
                            Id = id,
                            FileName = file.Name,
                            Path = file.FullName,
                            Text =  contents.Text
                        };
                        await client.IndexDocumentAsync(doc);
                    }
                }

                foreach (var dir in directory.GetDirectories())
                {
                    await BuildIndex(dir.FullName);
                }
            }
                
        }
    }
}