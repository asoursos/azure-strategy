using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App.Models
{
    public abstract class BaseTrackedDocument : BaseDocument
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ChangeSet> ChangeSets { get; set; }
    }

    public class ChangeSet
    {
        public long TimeStamp { get; set; }
        public Dictionary<string, object> Changes { get; set; }
    }
}
