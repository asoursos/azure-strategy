using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyTrade.App.Storage
{
    public abstract class BaseDocumentDbRepository<T> where T : class
    {
        private static readonly DocumentClient client;

        public string DatabaseId { get; protected set; }
        public string CollectionId { get; protected set; }

        static BaseDocumentDbRepository()
        {
            client = new DocumentClient(new Uri(Constants.DocumentDbEndpoint), Constants.DocumentDbAuthKey);
        }

        protected BaseDocumentDbRepository(string databaseId, string collectionId)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        /// <summary>
        /// Checks if the document with the specified identifier exists in the database.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool Exists(string id)
        {
            return client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                .Where(d => d.Id == id)
                .Select(d => d.Id)
                .AsEnumerable()
                .Any();
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="storedProcedureId">The stored procedure identifier.</param>
        /// <param name="procedureParams">The procedure parameters.</param>
        /// <returns></returns>
        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedure<TValue>(string storedProcedureId, params dynamic[] procedureParams)
        {
            return await client.ExecuteStoredProcedureAsync<TValue>(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storedProcedureId), procedureParams);
        }

        /// <summary>
        /// Gets an item by its identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="maxItemCount">The maximum item count.</param>
        /// <param name="enableScanInQuery">if set to <c>true</c> [enable scan in query].</param>
        /// <returns></returns>
        public async Task<IEnumerable<TResult>> GetQuery<TResult>(Func<IOrderedQueryable<T>, IQueryable<TResult>> func, int? maxItemCount = null, bool enableScanInQuery = false)
        {
            var queryable = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions
                {
                    MaxItemCount = maxItemCount ?? -1,
                    EnableScanInQuery = enableScanInQuery
                });


            var resultQuery = func(queryable);
            var documentQuery = resultQuery.AsDocumentQuery();

            var results = await documentQuery.ExecuteNextAsync<TResult>();
            return results;
        }

        /// <summary>
        /// Gets the items asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="maxItemCount">The maximum item count.</param>
        /// <param name="enableScanInQuery">if set to <c>true</c> [enable scan in query].</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, int? maxItemCount = null, bool enableScanInQuery = false)
        {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions
                {
                    MaxItemCount = maxItemCount ?? -1,
                    EnableScanInQuery = enableScanInQuery
                })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        /// <summary>
        /// Creates the item asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<Document> CreateItemAsync(T item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item, disableAutomaticIdGeneration: true);
        }

        /// <summary>
        /// Updates the item asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="item">The item.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async Task<Document> UpdateItemAsync(string id, T item, RequestOptions options = null)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item, options);
        }

        /// <summary>
        /// Upserts the item asynchronous.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public async Task<Document> UpsertItemAsync(T item, RequestOptions options = null)
        {
            return await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item, disableAutomaticIdGeneration: true);
        }

        /// <summary>
        /// Deletes the item asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task DeleteItemAsync(string id)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
        }
    }
}
