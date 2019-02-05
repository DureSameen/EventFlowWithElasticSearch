using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.RabbitMQ.Integrations;
using EventFlow.Subscribers;

namespace EventFlow.RabbitMQ
{
    public class RabbitMqDomainEventSubscriber : ISubscribeSynchronousToAll
    {
        private readonly IRabbitMqSubscriber _rabbitMqSubscriber;
        private readonly IRabbitMqMessageFactory _rabbitMqMessageFactory;

        public RabbitMqDomainEventSubscriber(
            IRabbitMqSubscriber  rabbitMqSubscriber,
            IRabbitMqMessageFactory rabbitMqMessageFactory)
        {
            _rabbitMqSubscriber = rabbitMqSubscriber;
            _rabbitMqMessageFactory = rabbitMqMessageFactory;
        }

        public Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            var rabbitMqMessages = domainEvents.Select(e => _rabbitMqMessageFactory.CreateMessage(e)).ToList();

          // return _rabbitMqSubscriber.SubscribeAsync( rabbitMqMessages, cancellationToken);
          return Task.FromResult(0);
        }
    }
}
