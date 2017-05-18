using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App.Models
{
    public class ZipCodeEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("location")]
        public Point Location { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("pop")]
        public int Population { get; set; }
    }
}