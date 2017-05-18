using System.Security.Claims;
using System.Threading.Tasks;
using EasyTrade.App.Messaging;
using EasyTrade.App.RequestHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyTrade.App.Controllers
{
    [Authorize]
    public class TradeController : BaseController
    {
        public TradeController(IDispatcher dispatcher) : base(dispatcher)
        {
        }

        public IActionResult Index()
        {
            var vm = new TradingVm { Identity = (ClaimsIdentity)User.Identity};

            return View(vm);
        }

        [Route("api/[controller]/list")]
        [HttpGet]
        public Task<TradeResult> Get()
        {
            return Task.FromResult(new TradeResult());
        }

        [Route("api/[controller]")]
        [HttpPost]
        public async Task<TradeResult> Post(AddTradeRequest request)
        {
            return await Handle(request);
        }

        #region View Model

        public class TradingVm
        {
            public ClaimsIdentity Identity { get; set; }
        }

        #endregion
    }
}