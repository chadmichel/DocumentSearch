using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSearchCore.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class DownloaderController : ControllerBase
    {
        [HttpGet]
        [Route("DownloadIt")]
        public FileResult Download(string query)
        {
            var path = query;

            var fi = new FileInfo(path);
            
            var response = new HttpResponseMessage();
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            return File(stream, 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                fi.Name);
        }
    }
}