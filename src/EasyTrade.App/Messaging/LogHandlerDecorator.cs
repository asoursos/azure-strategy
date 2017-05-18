using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyTrade.App.Messaging
{
    public class LogHandlerDecorator<TRequest, TResponse> : IHandlerDecorator<TRequest, TResponse>
        where TRequest : IMessage<TResponse>
    {
        protected IHandler<TRequest, TResponse> InnerHandler { get; private set; }

        public IHandler<TRequest, TResponse> Decorate(IHandler<TRequest, TResponse> innerHandler)
        {
            InnerHandler = innerHandler;
            return this;
        }

        public async Task<TResponse> HandleAsync(TRequest message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string messageTypeName = message.GetType().FullName;
            Debug.WriteLine($"About to process request: {messageTypeName}");

            TResponse response = await InnerHandler.HandleAsync(message);

            Debug.WriteLine($"Request processed: {messageTypeName}, ellapsedTime: {sw.Elapsed}");

            return response;
        }
    }
}