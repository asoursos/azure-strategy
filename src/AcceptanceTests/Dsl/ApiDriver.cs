using System;
using System.Net.Http;
using System.Threading.Tasks;
using EasyTrade.App.RequestHandlers;

namespace AcceptanceTests.Dsl
{
    public interface IApiDriver : IDisposable
    {
        Task<TradeResult> AddTrade(AddTradeRequest request);
    }

    public class ApiDriver : IApiDriver
    {
        private readonly HttpClient _client;

        public ApiDriver(HttpClient client)
        {
            _client = client;
        }

        public Task<TradeResult> AddTrade(AddTradeRequest request)
        {
            return _client.PostAsync<TradeResult>("api/trade", request);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}