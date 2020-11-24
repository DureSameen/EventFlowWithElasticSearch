using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Extensions;
using EventFlow.Logs;
using EventFlow.Snapshots;

namespace EventFlowApi.EventStore.EventStore
{
    public class CustomEventStoreBase : IEventStore
    {

        private readonly IAggregateFactory _aggregateFactory;
        private readonly IEventJsonSerializer _eventJsonSerializer;
        private readonly IEventPersistence _eventPersistence;
        private readonly ISnapshotStore _snapshotStore;
        private readonly IEventUpgradeManager _eventUpgradeManager;
        private readonly ILog _log;
        private readonly IReadOnlyCollection<IMetadataProvider> _metadataProviders;
        private readonly IDataRetrievalConfiguration _dataRetrievalConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="aggregateFactory"></param>
        /// <param name="eventJsonSerializer"></param>
        /// <param name="eventUpgradeManager"></param>
        /// <param name="metadataProviders"></param>
        /// <param name="eventPersistence"></param>
        /// <param name="snapshotStore"></param>
        /// <param name="dataRetrievalConfiguration"></param>
        public CustomEventStoreBase(
            ILog log,
            IAggregateFactory aggregateFactory,
            IEventJsonSerializer eventJsonSerializer,
            IEventUpgradeManager eventUpgradeManager,
            IEnumerable<IMetadataProvider> metadataProviders,
            IEventPersistence eventPersistence,
            ISnapshotStore snapshotStore, IDataRetrievalConfiguration dataRetrievalConfiguration)
        {
            _eventPersistence = eventPersistence;
            _snapshotStore = snapshotStore;
            _log = log;
            _aggregateFactory = aggregateFactory;
            _eventJsonSerializer = eventJsonSerializer;
            _eventUpgradeManager = eventUpgradeManager;
            _metadataProviders = metadataProviders.ToList();
            _dataRetrievalConfiguration = dataRetrievalConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <typeparam name="TIdentity"></typeparam>
        /// <param name="id"></param>
        /// <param name="uncommittedDomainEvents"></param>
        /// <param name="sourceId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<IReadOnlyCollection<IDomainEvent<TAggregate, TIdentity>>> StoreAsync<TAggregate,
                TIdentity>(
                TIdentity id,
                IReadOnlyCollection<IUncommittedEvent> uncommittedDomainEvents,
                ISourceId sourceId,
                CancellationToken cancellationToken)
                where TAggregate : IAggregateRoot<TIdentity>
                where TIdentity : IIdentity
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (sourceId.IsNone()) throw new ArgumentNullException(nameof(sourceId));

            if (uncommittedDomainEvents == null || !uncommittedDomainEvents.Any())
            {
                return new IDomainEvent<TAggregate, TIdentity>[] { };
            }

            var aggregateType = typeof(TAggregate);
            _log.Verbose(() =>
                $"Storing {uncommittedDomainEvents.Count} events for aggregate '{aggregateType.PrettyPrint()}' with ID '{id}'");

            var batchId = Guid.NewGuid().ToString();
            var storeMetadata = new[]
            {
                    new KeyValuePair<string, string>(MetadataKeys.BatchId, batchId),
                    new KeyValuePair<string, string>(MetadataKeys.SourceId, sourceId.Value)
                };

            var serializedEvents = uncommittedDomainEvents
                .Select(e =>
                {
                    var md = _metadataProviders
                        .SelectMany(p => p.ProvideMetadata<TAggregate, TIdentity>(id, e.AggregateEvent, e.Metadata))
                        .Concat(e.Metadata)
                        .Concat(storeMetadata);
                    return _eventJsonSerializer.Serialize(e.AggregateEvent, md);
                })
                .ToList();

            var committedDomainEvents = await _eventPersistence.CommitEventsAsync(
                    id,
                    serializedEvents,
                    cancellationToken)
                .ConfigureAwait(false);

            var domainEvents = committedDomainEvents
                .Select(e => _eventJsonSerializer.Deserialize<TAggregate, TIdentity>(id, e))
                .ToList();

            return domainEvents;
        }
        /// <summary>
        /// filter only 'not null' IDomainEvent(s) and range of required date is applied.
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AllEventsPage> LoadAllEventsAsync(
            GlobalPosition globalPosition,
            int pageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
                var task = _eventPersistence.LoadAllCommittedEvents(
                       globalPosition,
                       pageSize,
                       cancellationToken);

                var allCommittedEventsPage = task.Result;
                var exception = task.Exception;

                IEnumerable<ICommittedDomainEvent> committedDomainEvents = allCommittedEventsPage.CommittedDomainEvents.ToList();
                var domainEvents = committedDomainEvents
                    .Select(e => _eventJsonSerializer.Deserialize(e)).Where(e => e != null && (_dataRetrievalConfiguration.FromDate.HasValue && _dataRetrievalConfiguration.ToDate.HasValue && e.Timestamp.DateTime >= _dataRetrievalConfiguration.FromDate && e.Timestamp.DateTime <= _dataRetrievalConfiguration.ToDate ||
                                                                                               _dataRetrievalConfiguration.FromDate.HasValue && !_dataRetrievalConfiguration.ToDate.HasValue && e.Timestamp.DateTime >= _dataRetrievalConfiguration.FromDate ||
                                                                                               !_dataRetrievalConfiguration.FromDate.HasValue && _dataRetrievalConfiguration.ToDate.HasValue && e.Timestamp.DateTime <= _dataRetrievalConfiguration.ToDate ||
                                                                                               !_dataRetrievalConfiguration.FromDate.HasValue && !_dataRetrievalConfiguration.ToDate.HasValue)).ToList();


                var domainEventsList = (IReadOnlyCollection<IDomainEvent>)domainEvents;

                domainEventsList = _eventUpgradeManager.Upgrade(domainEventsList);
                return new AllEventsPage(allCommittedEventsPage.NextGlobalPosition, domainEventsList);
            }
            catch (Exception exp)
            {
                _log.Error($"Exception : {exp.Message}");
                return null;
            }
        }




        public Task<IReadOnlyCollection<IDomainEvent<TAggregate, TIdentity>>>
                LoadEventsAsync<TAggregate, TIdentity>(
                    TIdentity id,
                    CancellationToken cancellationToken)
                where TAggregate : IAggregateRoot<TIdentity>
                where TIdentity : IIdentity
        {
            return LoadEventsAsync<TAggregate, TIdentity>(
                id,
                1,
                cancellationToken);
        }

        public virtual async Task<IReadOnlyCollection<IDomainEvent<TAggregate, TIdentity>>> LoadEventsAsync<
            TAggregate, TIdentity>(
            TIdentity id,
            int fromEventSequenceNumber,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            if (fromEventSequenceNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(fromEventSequenceNumber),
                    "Event sequence numbers start at 1");

            var committedDomainEvents = await _eventPersistence.LoadCommittedEventsAsync(
                    id,
                    fromEventSequenceNumber,
                    cancellationToken)
                .ConfigureAwait(false);
            var domainEvents = (IReadOnlyCollection<IDomainEvent<TAggregate, TIdentity>>)committedDomainEvents
                .Select(e => _eventJsonSerializer.Deserialize<TAggregate, TIdentity>(id, e))
                .ToList();

            if (!domainEvents.Any())
            {
                return domainEvents;
            }

            domainEvents = _eventUpgradeManager.Upgrade(domainEvents);

            return domainEvents;
        }

        public virtual async Task<TAggregate> LoadAggregateAsync<TAggregate, TIdentity>(
            TIdentity id,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            var aggregate = await _aggregateFactory.CreateNewAggregateAsync<TAggregate, TIdentity>(id)
                .ConfigureAwait(false);
            await aggregate.LoadAsync(this, _snapshotStore, cancellationToken).ConfigureAwait(false);
            return aggregate;
        }

        public Task DeleteAggregateAsync<TAggregate, TIdentity>(
            TIdentity id,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return _eventPersistence.DeleteEventsAsync(
                id,
                cancellationToken);
        }
    }

}
