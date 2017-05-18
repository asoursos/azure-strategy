using System.Threading.Tasks;
using EasyTrade.App.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace EasyTrade.App.Controllers
{
    public abstract class BaseController : Controller
    {
        protected BaseController(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        protected IDispatcher Dispatcher { get; }

        protected Task<TResult> Handle<TResult>(IMessage<TResult> message)
        {
            return Dispatcher.Send(message);
        }
    }
}