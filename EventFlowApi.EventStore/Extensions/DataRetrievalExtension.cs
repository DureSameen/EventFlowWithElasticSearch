
using EventFlow;
using EventFlow.Configuration;
using EventFlow.EventStores;
using EventFlowApi.EventStore.EventStore;
using System.Reflection;

namespace EventFlowApi.EventStore.Extensions
{
    public static class DataRetrievalExtension
    {
        public static IEventFlowOptions ConfigureDataRetrieval(
            this IEventFlowOptions options, DataRetrieval configuration, Assembly assembly)
        {
            options.RegisterServices(sr => sr.Register<IBootstrap, ReadModelRebuilder>())
                .RegisterServices(sr => sr.Register<IEventStore, CustomEventStoreBase>())
                .RegisterServices(sr => sr.Register<IEventJsonSerializer, CustomEventJsonSerializer>())
                .RegisterServices(sr =>
                    sr.Register<IDataRetrievalConfiguration>(r => new DataRetrievalConfiguration(configuration, assembly)));

            return options;

        }
    }
}
