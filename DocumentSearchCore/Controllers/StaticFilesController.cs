using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Console = System.Console;
using System;

namespace DocumentSearchCore.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class StaticFilesController : ControllerBase
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
            var staticContent = System.IO.File.ReadAllText(path);
            response.Content = new StringContent(staticContent);
            
            response.Content.Headers.ContentType = ContentType(url);
            return response;
        }
    }
}