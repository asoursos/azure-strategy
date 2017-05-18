using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App.Models
{
    public abstract class BaseDocument
    {
        [JsonProperty(PropertyName = "id")]
        public virtual string UId
        {
            get { return $"{GetType().Name}-{Id}"; }
        }

        [JsonProperty(PropertyName = "EntityId")]
        public virtual object Id { get; set; }
    }
}
