using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DocumentSearch
{
    public class DownloaderController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Download(string query)
        {
            var path = Path.Combine(StaticFilesController.basePath, query);
            var response = new HttpResponseMessage();
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return response;
        }
    }
}