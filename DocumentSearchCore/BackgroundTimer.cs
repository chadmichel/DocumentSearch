using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DocumentSearchCore
{
    public class BackgroundTimer : IHostedService
    {
        private IDocumentManager _documentManager;
        private ILogger _logger;
        private Timer _timer;
        IConfiguration _configuration;
        
        public BackgroundTimer(IDocumentManager documentManager, ILogger logger,
            IConfiguration configuration)
        {
            _documentManager = documentManager;            
            _logger = logger;
            _configuration = configuration;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, 
                TimeSpan.FromSeconds(60));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
        private void DoWork(object state)
        {
            _logger.Debug("Background timer start");

            var path = _configuration["AppSettings:DocumentDirectory"];
            _logger.Debug("Indexing path = " + path);
            _documentManager.BuildIndex(path);

            _logger.Debug("Background timer end");
        }
    }
}