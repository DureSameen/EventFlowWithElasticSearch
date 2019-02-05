using EventFlow.Aggregates;
using EventFlow.Subscribers;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlowApi.Core;
using EventFlowApi.Core.Aggregates.Commands;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;
using System.Collections.Generic;
using System.Linq;

namespace EventflowApi.Read.Subscribers
{
    public class AllEventsSubscriber : ISubscribeSynchronousToAll
    {

        protected readonly ICommandBus CommandBus;
        public AllEventsSubscriber(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

         
        public async Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
        {
            var domainEvent = domainEvents.FirstOrDefault();

            var employeeCommand = new EmployeeAddCommand((EmployeeId)domainEvent.GetIdentity() , ((EmployeeAddedEvent)domainEvent.GetAggregateEvent()).EmployeeRecord);

            await CommandBus.PublishAsync(employeeCommand, CancellationToken.None).ConfigureAwait(false);


        }


    }
}
