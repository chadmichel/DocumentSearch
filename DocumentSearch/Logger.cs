using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using java.sql;

namespace DocumentSearch
{
    public interface ILogger
    {
        Task Debug(string message);

        Task<string> List();
    }
    
    public class Logger : ILogger
    {

        List<string> _messages = new List<string>();
        
        public async Task Debug(string message)
        {
            Console.WriteLine(message);

            _messages.Add(message);

            if (_messages.Count > 100)
            {
                _messages.RemoveAt(0);
            }

            var now = DateTime.Now;
            var file = $"Log_{now.Year}_{now.Month}_{now.Day}_{now.Hour}.txt";
            using (var f = File.CreateText(file))
            {
                await f.WriteLineAsync(message);
            }
        }

        public async Task<string> List()
        {
            var sb = new StringBuilder();

            foreach (var item in (_messages))
            {
                sb.AppendLine(item);
            }

            return sb.ToString();
        }
    }
}