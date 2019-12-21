using System;
using System.IO;
using System.Linq;
using Nest;
using TikaOnDotNet.TextExtraction;

namespace DocumentSearch
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Check the command line parameters
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Pass in the folder to index");
                return;
            }

            // Use TikaOnDotNet to extract the contents of the document.
            var textExtractor = new TextExtractor();
            
            
            // Use ElasticSearch to index the document
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("docs");
            var client = new ElasticClient(settings);

            // Loop through all files in the passed in directory
            var directory = new DirectoryInfo(args[0]);
            if (directory.Exists)
            {
                foreach (var file in (directory.GetFiles()))
                {
                    var contents = textExtractor.Extract(file.FullName);
                    if (!string.IsNullOrWhiteSpace(contents.Text))
                    {

                        Console.WriteLine("Indexing " + file.FullName);
                        
                        var doc = new Doc()
                        {
                            FileName = file.Name,
                            Path = file.FullName,
                            Text =  contents.Text
                        };
                        client.IndexDocument(doc);
                    }
                }
            }

            // Loop until user types 'exit'
            while (true)
            {
                Console.WriteLine("Enter search text (type 'exit' to exit)");
                var search = Console.ReadLine();

                if (search == "exit")
                {
                    break; // exit while loop
                }
                else
                {
                    // Search elastic for query
                    var response = client.Search<Doc>(s => s
                        .From(0)
                        .Size(10)
                        .Query(q => q
                            .Fuzzy(m => m
                                .Field(f => f.Text)
                                .Value(search)))
                        .Highlight(h => h.Fields(x => x.Field(y => y.Text))));
                    
                    if (response.Documents.Count > 0)
                    {
                        // If we had hits, push them to the console
                        var index = 0;
                        foreach (var hit in response.Hits)
                        {
                            Console.WriteLine("FILE: " + hit.Source.FileName);
                            
                            foreach (var highlight in hit.Highlight)
                            {
                                foreach (var highlightItem in highlight.Value)
                                {
                                    Console.WriteLine(highlightItem);
                                }
                            }
                        }   
                    }
                    else
                    {
                        Console.WriteLine("No results found");
                    }
                }
            }
        }
    }
    
    public class Doc
    {
        public string FileName { get; set; }
        
        public string Path { get; set; }
        public string Text { get; set; }
    }
}