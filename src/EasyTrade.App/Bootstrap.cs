using EasyTrade.App.Messaging;
using EasyTrade.App.RequestHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTrade.App
{
    public static class Bootstrap
    {
        public static IServiceCollection AddPcBlockServices(this IServiceCollection services)
        {
            // Generic Registrations
            services.AddTransient(provider => new ServiceFactory(provider.GetServices))
                .AddTransient<HandlerFactory>()
                .AddSingleton<IDispatcher, Dispatcher>();

            // Handler decorators
            services.AddTransient(typeof(IHandlerDecorator<,>), typeof(LogHandlerDecorator<,>));

            // Message Handlers registration 
            services.AddTransient<IHandler<AddTradeRequest, TradeResult>, RulesHandler>();

            return services;
        }
    }
}