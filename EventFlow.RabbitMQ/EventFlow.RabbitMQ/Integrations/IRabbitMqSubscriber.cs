using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Configuration;
using EventFlow.Subscribers;

namespace EventFlow.RabbitMQ.Integrations
{
    public interface IRabbitMqSubscriber
    {
        Task SubscribeAsync(string exchange, string queue, Action<IList<IDomainEvent>, IDomainEventPublisher> action, IDomainEventPublisher domainEventPublisher, CancellationToken cancellationToken);
    }
}
