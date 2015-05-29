using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using RestaurantGuide.Search;

namespace RestaurantGuide.DataAccess
{
    public class SearchIndexHelper
    {
        public static IEnumerable<Field> GetIndexFieldsFromType<T>()
        {
            return (from p in typeof (T).GetProperties()
                let field = CreateFieldFromProperty(p)
                where field != null
                select field);
        }

        public static IEnumerable<string> GetFacetableFieldNamesFromType<T>()
        {
            return (from field in GetIndexFieldsFromType<T>()
                    where field.IsFacetable
                    select field.Name);
        }

        public static IReadOnlyDictionary<string, List<string>> GetSuggestersFromType<T>()
        {
            var properties = from p in typeof(T).GetProperties()
                let suggestionAttribute = p.GetCustomAttribute(typeof(SearchSuggestionAttribute)) as SearchSuggestionAttribute
                     where suggestionAttribute != null
                     select p;

            Dictionary<string, List<string>> suggesters = new Dictionary<string, List<string>>();

            foreach (var suggesionProperty in properties)
            {
                foreach (var suggestionAttribute in suggesionProperty.GetCustomAttributes<SearchSuggestionAttribute>())
                {
                    if (!suggesters.ContainsKey(suggestionAttribute.SuggesterName))
                    {
                        suggesters.Add(suggestionAttribute.SuggesterName, new List<string>());
                    }

                    suggesters[suggestionAttribute.SuggesterName].Add(suggesionProperty.Name);
                }
            }

            return suggesters;
        }

        private static Field CreateFieldFromProperty(PropertyInfo propertyInfo)
        {
            var searchIntexAttribute =
                propertyInfo.GetCustomAttribute(typeof(SearchIndexAttribute)) as SearchIndexAttribute;

            if (searchIntexAttribute == null)
            {
                return null;
            }

            var field = new Field(searchIntexAttribute.Name ?? propertyInfo.Name,
                ToSearchType(propertyInfo.PropertyType))
            {
                IsKey = searchIntexAttribute.IsKey,
                IsRetrievable = searchIntexAttribute.IsRetrievable,
                IsSearchable = searchIntexAttribute.IsSearchable,
                IsSortable = searchIntexAttribute.IsSortable,
                IsFilterable = searchIntexAttribute.IsFilterable,
                IsFacetable = searchIntexAttribute.IsFacetable
            };
            return field;
        }

        private static string ToSearchType(Type type)
        {
            // see https://msdn.microsoft.com/nl-nl/library/azure/dn798938.aspx

            if (type == typeof(string)) return "Edm.String";
            if (type == typeof(bool)) return "Edm.Boolean";
            if (type == typeof(int)) return "Edm.Int32";
            if (type == typeof(long)) return "Edm.Int64";
            if (type == typeof(double)) return "Edm.Double";
            if (type == typeof(DateTimeOffset)) return "Edm.DateTimeOffset";
            if (type == typeof(GeographyPoint)) return "Edm.GeographyPoint";
            if (typeof(IEnumerable<string>).IsAssignableFrom(type)) return "Collection(Edm.String)";

            throw new ArgumentException(string.Format("Unsupported type: '{0}'", type), "type");
        }
    }
}