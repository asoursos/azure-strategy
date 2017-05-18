using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTrade.App.Models
{
    public class ZipEntrySearchInput
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int Distance { get; set; }
    }
}
