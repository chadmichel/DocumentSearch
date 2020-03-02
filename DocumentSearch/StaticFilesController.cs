using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using java.io;
using opennlp.tools.parser;
using File = System.IO.File;
using System;
using Console = System.Console;

namespace DocumentSearch
{
    public class StaticFilesController : ApiController
    {
        public static string basePath = @"/Users/chadmichel/Projects/DocumentSearch/DocumentSearchClient/dist/DocumentSearchClient";

        private string GeneratePath(string url)
        {
            return Path.Combine(basePath, url);   
        }

        private MediaTypeHeaderValue ContentType(string url)
        {
            
            if (url.EndsWith(".js"))
                return new MediaTypeHeaderValue("text/javascript");
            else
                return new MediaTypeHeaderValue("text/html");
        }
        
        [HttpGet]
        public HttpResponseMessage Index(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = "index.html";

            var path = GeneratePath(url);

            Console.WriteLine("HTTP GET " + path);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(File.ReadAllText(path));
            response.Content.Headers.ContentType = ContentType(url);
            return response;
        }
    }
}