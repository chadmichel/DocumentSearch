using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DocumentSearch
{
    public class SearchController : ApiController
    {
        [HttpGet]
        public SearchResult Search(string query)
        {
            var searchResult = new SearchResult()
            {
                Query =  query
            };
            var items = new List<SearchResultItem>();
            
            if (query.Contains(" "))
            {
                SearchPhrase(query, items);
            }

            SearchFuzzy(query, items);
            
            searchResult.Items = items.ToArray();
            return searchResult;
        }

        void SearchFuzzy(string query, IList<SearchResultItem> items)
        {
            var response = Program.CreateElasticClient().Search<Doc>(s => s
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
                            items.Add(new SearchResultItem()
                            {
                                FilePath =  hit.Source.FileName,
                                Highlight = highlightItem
                            });
                        }
                    }
                }   
            }
        }
        
        void SearchPhrase(string phrase, IList<SearchResultItem> items)
        {
            var response = Program.CreateElasticClient().Search<Doc>(s => s
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
                                return;
                            
                            items.Add(new SearchResultItem()
                            {
                                FilePath =  hit.Source.FileName,
                                Highlight = highlightItem
                            });
                        }
                    }
                }   
            }
        }
    }
}