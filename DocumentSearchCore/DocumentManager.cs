using System;
using System.IO;
using System.Threading.Tasks;
using DocumentSearch;
using Microsoft.Extensions.Configuration;
using TikaOnDotNet.TextExtraction;

namespace DocumentSearchCore
{
    public interface IDocumentManager
    {
        Task BuildIndex(string startPath);
    }
    
    public class DocumentManager : IDocumentManager
    {
        private readonly IElasticAccess _elasticAccess;
        private ILogger _logger;
        private IConfiguration _configuration;

        public DocumentManager(
            IElasticAccess elasticAccess,
            ILogger logger,
            IConfiguration configuration)
        {
            _elasticAccess = elasticAccess;
            _logger = logger;
            _configuration = configuration;
        }
        
        public async Task BuildIndex(string startPath)
        {
            await _logger.Debug("Indexing Directory: " + startPath);
            
            // Use TikaOnDotNet to extract the contents of the document.
            var textExtractor = new TextExtractor();
            
            // Loop through all files in the passed in directory
            var directory = new DirectoryInfo(startPath);
            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles())
                {
                    var contents = textExtractor.Extract(file.FullName);
                    if (!string.IsNullOrWhiteSpace(contents.Text))
                    {
                        await _logger.Debug("Indexing File " + file.FullName);
                        var id = HashUtility.Hash(file.FullName);
                        await _elasticAccess.IndexDocument(file.Name, file.FullName, contents.Text);
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