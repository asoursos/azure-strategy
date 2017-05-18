using EasyTrade.App.RequestHandlers;
using Xunit;

namespace AcceptanceTests.Trading
{
    [Trait("Trading", "Valid request received.")]
    public class ValidRequestReceived : IClassFixture<TestFixture<TestStartup>>
    {
        public ValidRequestReceived(TestFixture<TestStartup> fixture)
        {
            _result = fixture.ApiDriver.AddTrade(new AddTradeRequest()).Result;
        }

        private readonly TradeResult _result;

        [Trait("Trade happened.", "")]
        [Fact]
        public void TradeHappened()
        {
            Assert.NotNull(_result);
        }
    }
}