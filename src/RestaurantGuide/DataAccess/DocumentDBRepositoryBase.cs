using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace RestaurantGuide.DataAccess
{
    public abstract class DocumentDBRepositoryBase<T>
    {
        protected string DatabaseId;
        protected string CollectionId;
        private readonly Lazy<Database> _database;
        private readonly Lazy<DocumentCollection> _collection;
        private readonly Lazy<DocumentClient> _client;

        public DocumentDBRepositoryBase()
        {
            _database = new Lazy<Database>(() =>
            {
                return ReadOrCreateDatabase();
            });
            _collection = new Lazy<DocumentCollection>(() =>
            {
                return ReadOrCreateCollection(_database.Value.SelfLink);
            });
            _client = new Lazy<DocumentClient>(() =>
            {
                string endpoint = ConfigurationManager.AppSettings["endpoint"];
                string authKey = ConfigurationManager.AppSettings["authKey"];
                Uri endpointUri = new Uri(endpoint);
                return new DocumentClient(endpointUri, authKey);
            });
        }

        public IEnumerable<T> GetItems()
        {
            return _client.Value.CreateDocumentQuery<T>(_collection.Value.DocumentsLink)
                .AsEnumerable();
        }

        public IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return _client.Value.CreateDocumentQuery<T>(_collection.Value.DocumentsLink)
                .Where(predicate)
                .AsEnumerable();
        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            return _client.Value.CreateDocumentQuery<T>(_collection.Value.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public async Task<string> CreateItemAsync(T item)
        {
            var doc = await _client.Value.CreateDocumentAsync(_collection.Value.SelfLink, item);
            return doc.Resource.Id;
        }

        public async Task<string> UpdateItemAsync(string id, T item)
        {
            Document doc = GetDocument(id);
            var updatedDoc = await _client.Value.ReplaceDocumentAsync(doc.SelfLink, item);
            return updatedDoc.Resource.Id;
        }

        public async Task DeleteItemAsync(string id)
        {
            Document doc = GetDocument(id);
            var deletedDoc =  await _client.Value.DeleteDocumentAsync(doc.SelfLink);
        }

        private Database ReadOrCreateDatabase()
        {
            var db = _client.Value.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = _client.Value.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        private DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = _client.Value.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = CollectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = _client.Value.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        private Document GetDocument(string id)
        {
            return _client.Value.CreateDocumentQuery(_collection.Value.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        } 
 
    }
}