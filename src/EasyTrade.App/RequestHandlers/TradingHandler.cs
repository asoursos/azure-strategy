using System.Threading.Tasks;
using EasyTrade.App.Messaging;

namespace EasyTrade.App.RequestHandlers
{
    public class AddTradeRequest : IMessage, IMessage<TradeResult>
    {
        public string Currency1 { get; set; }
        public string Currency2 { get; set; }
    }

    public class TradeResult
    {
        public int Id { get; set; }
    }

    public class TradingHandler : IHandler<AddTradeRequest, TradeResult>
    {
        public Task<TradeResult> HandleAsync(AddTradeRequest message)
        {
            return Task.FromResult(new TradeResult());
        }
    }
}