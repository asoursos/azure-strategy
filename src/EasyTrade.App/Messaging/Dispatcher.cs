using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyTrade.App.Messaging
{
    public interface IMessage { }

    public interface IMessage<out TResult> { }

    public delegate IEnumerable<object> ServiceFactory(Type serviceType);

    public interface IHandler<in TRequest> where TRequest : IMessage
    {
        Task HandleAsync(TRequest message);
    }

    public interface IHandler<in TRequest, TResponse>
        where TRequest : IMessage<TResponse>
    {
        Task<TResponse> HandleAsync(TRequest message);
    }

    public interface IHandlerDecorator<TRequest, TResponse> : IHandler<TRequest, TResponse>
        where TRequest : IMessage<TResponse>
    {
        IHandler<TRequest, TResponse> Decorate(IHandler<TRequest, TResponse> innerHandler);
    }

    public interface IDispatcher
    {
        Task<TResponse> Send<TResponse>(IMessage<TResponse> request);
    }

    #region [ Impl ]
    public class HandlerFactory
    {
        private readonly ServiceFactory serviceFactory;

        public HandlerFactory(ServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
        }

        public IHandler<TRequest, TResult> Create<TRequest, TResult>() where TRequest : IMessage<TResult>
        {
            var handler = this.serviceFactory(typeof(IHandler<TRequest, TResult>)).OfType<IHandler<TRequest, TResult>>().Single();
            var decorators = this.serviceFactory(typeof(IHandlerDecorator<TRequest, TResult>)).OfType<IHandlerDecorator<TRequest, TResult>>();

            if (decorators?.Any() == true)
                return decorators.Aggregate(handler, (innerHandler, decorator) => decorator.Decorate(innerHandler));

            return handler;
        }
    }

    public class Dispatcher : IDispatcher
    {
        private readonly ConcurrentDictionary<string, Func<Dispatcher, object, object>> doSendFuncCache;
        private readonly HandlerFactory handlerFactory;

        public Dispatcher(HandlerFactory handlerFactory)
        {
            this.doSendFuncCache = new ConcurrentDictionary<string, Func<Dispatcher, object, object>>();
            this.handlerFactory = handlerFactory;
        }

        /// <summary>
        /// Dispatches the message to its respective handler.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <returns>The handler's response.</returns>
        public Task<TResponse> Send<TResponse>(IMessage<TResponse> message)
        {
            var messageType = message.GetType();
            if (!doSendFuncCache.TryGetValue(messageType.FullName, out Func<Dispatcher, object, object> doSendFunc))
            {
                doSendFunc = BuildDoSendFunc(message);
                doSendFuncCache.TryAdd(messageType.FullName, doSendFunc);
            }

            var result = (Task<TResponse>)doSendFunc(this, message);
            return result;
        }

        /// <summary>
        /// The actual method that dispatches the message to the related handler.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <returns>The response from the respective handler.</returns>
        private async Task<TResponse> DoSend<TRequest, TResponse>(TRequest message) where TRequest : IMessage<TResponse>
        {
            var handler = this.handlerFactory.Create<TRequest, TResponse>();
            var result = await handler.HandleAsync(message);
            return result;
        }

        /// <summary>
        /// Builds the expression which compiles and builds the delegate to dispatch the message (actually calls the DoSend method).
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <returns>The func which does the dispatch of a message (accepts as first argument the Dispatcher instance, and as a second the message instance.</returns>
        private Func<Dispatcher, object, object> BuildDoSendFunc<TResponse>(IMessage<TResponse> message)
        {
            var messageType = message.GetType();
            var argumentParam = Expression.Parameter(typeof(object), "argument");
            var dispatcherParam = Expression.Parameter(typeof(Dispatcher), "dispatcher");
            var methodInfo = typeof(Dispatcher).GetMethod(nameof(DoSend), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(messageType, typeof(TResponse));
            Expression callExpr = Expression.Call(
                dispatcherParam,
                methodInfo,
                Expression.Convert(argumentParam, messageType));
            var func = Expression.Lambda<Func<Dispatcher, object, object>>(callExpr, dispatcherParam, argumentParam).Compile();

            return func;
        }
    }
    #endregion
}
