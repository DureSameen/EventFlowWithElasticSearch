using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Logs;

namespace EventFlowApi.EventStore.EventStore
{

    public class CustomEventJsonSerializer : IEventJsonSerializer
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IEventDefinitionService _eventDefinitionService;
        private readonly IDomainEventFactory _domainEventFactory;
        private readonly ILog _log;
        public CustomEventJsonSerializer(ILog log,
            IJsonSerializer jsonSerializer,
            IEventDefinitionService eventDefinitionService,
            IDomainEventFactory domainEventFactory)
        {
            _jsonSerializer = jsonSerializer;
            _eventDefinitionService = eventDefinitionService;
            _domainEventFactory = domainEventFactory;
            _log = log;
        }

        public SerializedEvent Serialize(
            IDomainEvent domainEvent)
        {
            return Serialize(domainEvent.GetAggregateEvent(), domainEvent.Metadata);
        }

        /// <summary>
        /// Serializes a event
        /// </summary>
        /// <param name="aggregateEvent"></param>
        /// <param name="metadatas"></param>
        /// <returns></returns>
        public SerializedEvent Serialize(IAggregateEvent aggregateEvent, IEnumerable<KeyValuePair<string, string>> metadatas)
        {
            var eventDefinition = _eventDefinitionService.GetDefinition(aggregateEvent.GetType());

            var metadata = new Metadata(metadatas
                .Where(kv => kv.Key != MetadataKeys.EventName && kv.Key != MetadataKeys.EventVersion) // TODO: Fix this
                .Concat(new[]
                    {
                        new KeyValuePair<string, string>(MetadataKeys.EventName, eventDefinition.Name),
                        new KeyValuePair<string, string>(MetadataKeys.EventVersion, eventDefinition.Version.ToString(CultureInfo.InvariantCulture)),
                    }));

            var dataJson = _jsonSerializer.Serialize(aggregateEvent);
            var metaJson = _jsonSerializer.Serialize(metadata);

            return new SerializedEvent(
                metaJson,
                dataJson,
                metadata.AggregateSequenceNumber,
                metadata);
        }

        public IDomainEvent Deserialize(string eventJson, string metadataJson)
        {
            var metadata = (IMetadata)_jsonSerializer.Deserialize<Metadata>(metadataJson);
            return Deserialize(eventJson, metadata);
        }

        public IDomainEvent Deserialize(string json, IMetadata metadata)
        {
            return Deserialize(metadata.AggregateId, json, metadata);
        }

        public IDomainEvent Deserialize(ICommittedDomainEvent committedDomainEvent)
        {
            var metadata = (IMetadata)_jsonSerializer.Deserialize<Metadata>(committedDomainEvent.Metadata);
            return Deserialize(committedDomainEvent.AggregateId, committedDomainEvent.Data, metadata);
        }

        public IDomainEvent<TAggregate, TIdentity> Deserialize<TAggregate, TIdentity>(
            TIdentity id,
            ICommittedDomainEvent committedDomainEvent)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return (IDomainEvent<TAggregate, TIdentity>)Deserialize(committedDomainEvent);
        }
        /// <summary>
        /// Deserialize the json object but swallow errors and return null on error.
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="json"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private IDomainEvent Deserialize(string aggregateId, string json, IMetadata metadata)
        {
            try
            {
                var eventDefinition = _eventDefinitionService.GetDefinition(
                    metadata.EventName,
                    metadata.EventVersion);

                var aggregateEvent = (IAggregateEvent)_jsonSerializer.Deserialize(json, eventDefinition.Type);

                var domainEvent = _domainEventFactory.Create(
                    aggregateEvent,
                    metadata,
                    aggregateId,
                    metadata.AggregateSequenceNumber);

                _log.Verbose(() =>
                    $"Deserialize { metadata.EventName} events at time stamp { metadata.Timestamp.DateTime}");
                return domainEvent;
            }

            catch
            {
                return null;
            }
        }
    }
}
