using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Configuration;
using EventFlow.Core;
using EventFlow.EventStores;
using EventFlow.Extensions;
using EventFlow.Logs;
using EventFlow.Subscribers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventFlow.RabbitMQ.Integrations
{
    public class RabbitMqSubscriber : IDisposable, IRabbitMqSubscriber
    {
        private readonly ILog _log;
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly IRabbitMqConfiguration _configuration;
        private readonly ITransientFaultHandler<IRabbitMqRetryStrategy> _transientFaultHandler;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly Dictionary<Uri, IRabbitConnection> _connections = new Dictionary<Uri, IRabbitConnection>();
        private readonly IRabbitMqMessageFactory _rabbitMqMessageFactory;
        public RabbitMqSubscriber(
            ILog log,
            IRabbitMqConnectionFactory connectionFactory,
            IRabbitMqConfiguration configuration,
            ITransientFaultHandler<IRabbitMqRetryStrategy> transientFaultHandler ,
            IRabbitMqMessageFactory rabbitMqMessageFactory
           )
        {
            _log = log;
            _connectionFactory = connectionFactory;
            _configuration = configuration;
            _transientFaultHandler = transientFaultHandler;
            _rabbitMqMessageFactory = rabbitMqMessageFactory;
        }

        public async Task SubscribeAsync(string exchange, string queue, Action<IList<IDomainEvent>, IDomainEventPublisher> action, IDomainEventPublisher domainEventPublisher,  CancellationToken cancellationToken)
        {
            Uri uri = _configuration.Uri;
            IRabbitConnection rabbitConnection = null;

            try
            {
                rabbitConnection = await GetRabbitMqConnectionAsync(uri, cancellationToken).ConfigureAwait(false);

                await _transientFaultHandler.TryAsync(
                    c => rabbitConnection.WithModelAsync(m => SubscribeAsync(m, exchange, queue, action,domainEventPublisher , c),c),
                    Label.Named("rabbitmq-subscribe"),
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (rabbitConnection != null)
                {
                    using (await _asyncLock.WaitAsync(CancellationToken.None).ConfigureAwait(false))
                    {
                        rabbitConnection.Dispose();
                        _connections.Remove(uri);
                    }
                }

                _log.Error(e, "Failed to subscribe to RabbitMQ");
                throw;
            }
        }

        private async Task<IRabbitConnection> GetRabbitMqConnectionAsync(Uri uri, CancellationToken cancellationToken)
        {
            using (await _asyncLock.WaitAsync(cancellationToken).ConfigureAwait(false))
            {
                IRabbitConnection rabbitConnection;

                if (_connections.TryGetValue(uri, out rabbitConnection))
                {
                    return rabbitConnection;
                }

                rabbitConnection = await _connectionFactory.CreateConnectionAsync(uri, cancellationToken).ConfigureAwait(false);

                _connections.Add(uri, rabbitConnection);

                return rabbitConnection;
            }
        }

        private Task<int> SubscribeAsync(IModel model, string exchange, string queue, Action<IList<IDomainEvent>, IDomainEventPublisher> action, IDomainEventPublisher domainEventPublisher,CancellationToken token)
        {
            _log.Verbose(
                "Subscribing to {0} exchange and {1} to RabbitMQ host '{2}'",
                exchange, 
                queue, 
                _configuration.Uri.Host);
            try
            {
                //model.QueueDeclare(queue: queue, durable: true);
                model.QueueDeclare(queue: queue, durable: true,
                    exclusive: false, autoDelete: false, arguments: null);
                model.ExchangeDeclare(exchange: exchange, type: "topic", durable:true); 

                model.QueueBind(queue: queue, exchange: exchange, routingKey: "eventflow.domainevent.employee.employee-added-event.1");  

                EventingBasicConsumer consumer = new EventingBasicConsumer(model);
                consumer.Received += (ch, ea) =>
                {
                  var rabbitMqMessage=  _rabbitMqMessageFactory.CreateMessage(ea);
                  var domainEvent = _rabbitMqMessageFactory.CreateDomainEvent(rabbitMqMessage);

                  action(new List<IDomainEvent>() {domainEvent}, domainEventPublisher);
              
                    model.BasicAck(ea.DeliveryTag, false);
                };
                  model.BasicConsume(queue, false, "", false, false, null, consumer);
            }
            catch (Exception exp)
            {
                _log.Verbose(
                    "Subscribing to {0} exchange and {1} to RabbitMQ host '{2}' Exception : {3}",
                    exchange,
                    queue,
                    _configuration.Uri.Host, exp.Message);
                
            }

            return Task.FromResult(0);
        }
        
        public void Dispose()
        {
            foreach (var rabbitConnection in _connections.Values)
            {
                rabbitConnection.Dispose();
            }
            _connections.Clear();
        }
    }
}
