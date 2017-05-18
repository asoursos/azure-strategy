using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App
{
    public static class Constants
    {
        /// <summary>
        /// The document database authentication key
        /// </summary>
        public const string DocumentDbAuthKey = "[YOUR_DOCUMENT_DB_AUTH_KEY]";

        /// <summary>
        /// The document database endpoint
        /// </summary>
        public const string DocumentDbEndpoint = "https://[DOCUMENTDB_DOMAIN].documents.azure.com:443/";

        /// <summary>
        /// The ad search database identifier
        /// </summary>
        public const string CosmosDatabaseId = "easytrade";

        /// <summary>
        /// The analysis document collection identifier
        /// </summary>
        public const string AnalysisCollectionId = "AnalysisCollection";

        /// <summary>
        /// The zip code collection identifier
        /// </summary>
        public const string ZipCollectionId = "ZipCollection";
    }
}
