using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Configuration;
using EventFlow.ReadStores;

namespace EventFlowApi.EventStore.EventStore
{
    public class ReadModelRebuilder : IBootstrap
    {
        private readonly IReadModelPopulator _populator;
        private readonly IDataRetrievalConfiguration _dataRetrievalConfiguration;

        public ReadModelRebuilder(IReadModelPopulator populator, IDataRetrievalConfiguration dataRetrievalConfiguration)
        {
            _populator = populator;
            _dataRetrievalConfiguration = dataRetrievalConfiguration;
        }

        public Task BootAsync(CancellationToken cancellationToken)
        {
            if (!_dataRetrievalConfiguration.Enabled) return Task.CompletedTask;

            var typeList = _dataRetrievalConfiguration.ReadModelAssembly.DefinedTypes.Where(type => type.Name != "LookUpEnumReadModel" && !type.IsInterface && !type.IsAbstract && type.ImplementedInterfaces.Any(inter => inter == typeof(IReadModel))).ToList();
            typeList.ForEach(async x =>
            {
                await _populator.PopulateAsync(x, cancellationToken);
            });
            return Task.CompletedTask;
        }

    }
}
