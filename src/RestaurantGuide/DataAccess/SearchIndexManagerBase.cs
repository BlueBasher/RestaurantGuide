using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using RestaurantGuide.Search;

namespace RestaurantGuide.DataAccess
{
    public class SearchIndexManagerBase<T> where T : class
    {
        private readonly SearchServiceClient _searchServiceClient;

        protected SearchIndexManagerBase(string serviceUrl, string apiKey)
        {
            _searchServiceClient = new SearchServiceClient(
                new SearchCredentials(apiKey), new Uri(serviceUrl));
        }

        public SearchIndexClient GetSearchClient(string indexName)
        {
            return _searchServiceClient.Indexes.GetClient(indexName);
        }

        public async Task InitialiseIndexForDocumentDb(
            string indexName,
            string documentDbConnectionString,
            string documentDbCollectionName)
        {
            //Data source name must only contain lowercase letters, digits or dashes, cannot start or end with dashes and is limited to 128 characters.
            var dataSourceName = indexName + "-datasource";
            // Indexer name must only contain lowercase letters, digits or dashes, cannot start or end with dashes and is limited to 128 characters.
            var indexerName = indexName + "-indexer";

            // See https://msdn.microsoft.com/en-us/library/azure/dn946891.aspx for the steps

            // Create an Azure Search index whose schema is compatible with your data source.
            var index = new Index(indexName, SearchIndexHelper.GetIndexFieldsFromType<T>().ToArray());

            foreach (var suggesterInfo in SearchIndexHelper.GetSuggestersFromType<T>())
            {
                index.Suggesters.Add(new Suggester(suggesterInfo.Key, SuggesterSearchMode.AnalyzingInfixMatching, suggesterInfo.Value));
            }

            var indexDefinitionResponse =
                await _searchServiceClient.Indexes.CreateOrUpdateAsync(index);

            // Create an Azure Search data source as described in https://msdn.microsoft.com/en-us/library/azure/dn946876.aspx.
            var dataSource = new DataSource(
                dataSourceName,
                DataSourceType.DocumentDb,
                new DataSourceCredentials(documentDbConnectionString),
                new DataContainer(documentDbCollectionName));

            var dataSourceDefinitionResponse = await _searchServiceClient.DataSources.CreateOrUpdateAsync(dataSource);

            // Create an Azure Search indexer as described in https://msdn.microsoft.com/en-us/library/azure/dn946899.aspx
            var indexer = new Indexer(indexerName, dataSourceName, indexName)
            {
                // Indexing schedule shortest supported interval: 00:05:00
                Schedule = new IndexingSchedule(TimeSpan.FromMinutes(5), DateTimeOffset.UtcNow)
            };
            var indexerDefinitionResponse = await _searchServiceClient.Indexers.CreateOrUpdateAsync(indexer);
        }
    }
}