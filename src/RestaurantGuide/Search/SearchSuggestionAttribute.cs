using System;
using Microsoft.Azure.Search.Models;

namespace RestaurantGuide.Search
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = true)]
    public class SearchSuggestionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the suggester.
        /// </summary>
        public string SuggesterName { get; set; }
    }
}