using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace DocumentSearchCore
{

    public interface IElasticAccess
    {
        Task IndexDocument(string fileName, string filePath, string contents);
        
        Task<IList<SearchResultItem>> Search(string query);
    }
    
    public class ElasticAccess : IElasticAccess
    {
        
        private static ElasticClient CreateElasticClient()
        {
            // Use ElasticSearch to index the document
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("docs2");
            var client = new ElasticClient(settings);
            return client;
        }

        private ElasticClient Client
        {
            get { return CreateElasticClient(); }
        }

        public async Task IndexDocument(string fileName, string filePath, string contents)
        {
            var id = HashUtility.Hash(filePath);

            var doc = new Doc()
            {
                Id = id,
                FileName = fileName,
                Path = filePath,
                Text =  contents
            };
            await CreateElasticClient().IndexDocumentAsync(doc);
        }
        
        public async Task<IList<SearchResultItem>> Search(string query)
        {
            var items = new List<SearchResultItem>();
            
            if (query.Contains(" "))
            {
                items.AddRange(await SearchPhrase(query));
            }

            items.AddRange(await SearchFuzzy(query));

            return items;
        }
        
        async Task<IList<SearchResultItem>> SearchFuzzy(string query)
        {
            var items = new List<SearchResultItem>();
            
            var response = await Client.SearchAsync<Doc>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Fuzzy(m => m
                        .Field(f => f.Text)
                        .Value(query)))
                .Highlight(h => h.Fields(x => x.Field(y => y.Text))));

            if (response.Documents.Count > 0)
            {
                // If we had hits, push them to the console
                foreach (var hit in response.Hits)
                {
                    Console.WriteLine("FILE: " + hit.Source.FileName);
                            
                    foreach (var highlight in hit.Highlight)
                    {
                        foreach (var highlightItem in highlight.Value)
                        {
                            if (items.Count > 50)
                                return items;

                            items.Add(new SearchResultItem()
                            {
                                FilePath =  hit.Source.FileName,
                                Highlight = highlightItem
                            });
                        }
                    }
                }   
            }

            return items;
        }

        async Task<IList<SearchResultItem>> SearchPhrase(string phrase)
        {
            var items = new List<SearchResultItem>();

            var response = await Client.SearchAsync<Doc>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .MatchPhrase(c => c
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Query(phrase)
                        .Slop(2)
                        .Field(f => f.Text)
                    ))
                .Highlight(h => h.Fields(x => x.Field(y => y.Text))));

            if (response.Documents.Count > 0)
            {
                // If we had hits, push them to the console
                foreach (var hit in response.Hits)
                {
                    Console.WriteLine("FILE: " + hit.Source.FileName);
                            
                    foreach (var highlight in hit.Highlight)
                    {
                        foreach (var highlightItem in highlight.Value)
                        {
                            if (items.Count > 50)
                                return items;
                            
                            items.Add(new SearchResultItem()
                            {
                                FilePath =  hit.Source.FileName,
                                Highlight = highlightItem
                            });
                        }
                    }
                }   
            }

            return items;
        }

    }
}