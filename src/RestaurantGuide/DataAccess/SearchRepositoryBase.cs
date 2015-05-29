using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using RestaurantGuide.Search;

namespace RestaurantGuide.DataAccess
{
    public abstract class SearchRepositoryBase<T> where T : class
    {
        private readonly SearchIndexClient _searchIndexClient;

        protected SearchRepositoryBase(string indexName)
        {
            _searchIndexClient = new RestaurantIndexManager().GetSearchClient(indexName);
        }

        public async Task<DocumentSearchResponse<T>> SearchAsync(string searchTerm, string filter=null)
        {
            try
            {
                var searchParameters = new SearchParameters
                {
                    Facets = SearchIndexHelper.GetFacetableFieldNamesFromType<T>().ToList(),
                    Filter = filter
                };
                return await _searchIndexClient.Documents.SearchAsync<T>(searchTerm, searchParameters);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetSuggestionsAsync(string suggesterName, string searchText)
        {
            try
            {
                var suggestParameters = new SuggestParameters
                {
                    Select = SearchIndexHelper.GetSuggestersFromType<T>()[suggesterName],
                    UseFuzzyMatching = true,
                    Top = 10
                };
                var response = await _searchIndexClient.Documents.SuggestAsync(searchText, suggesterName, suggestParameters);
                return response.Results.Select(r => r.Text).Distinct();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }
    }
}