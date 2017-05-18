using EasyTrade.App.Models;

namespace EasyTrade.App.Storage
{
    public class ZipCodeRepository : BaseDocumentDbRepository<ZipCodeEntry>
    {
        public ZipCodeRepository() 
            : base("easytrade", "ZipCollection")
        {                
        }
    }
}