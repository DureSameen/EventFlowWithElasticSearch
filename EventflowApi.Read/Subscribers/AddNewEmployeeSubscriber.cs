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

namespace EventflowApi.Read.Subscribers
{
    public class AddNewEmployeeSubscriber : ISubscribeAsynchronousTo<EmployeeAggregate, EmployeeId, EmployeeAddedEvent >
    {

        protected readonly ICommandBus CommandBus;
        public AddNewEmployeeSubscriber(ICommandBus commandBus)
        {
            CommandBus = commandBus;
        }

        public AddNewEmployeeSubscriber()
        {
        }

        public async Task HandleAsync(IDomainEvent<EmployeeAggregate, EmployeeId, EmployeeAddedEvent> domainEvent,
            CancellationToken cancellationToken)
        {
            var employeeCommand = new EmployeeAddCommand(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.EmployeeRecord);

            await CommandBus.PublishAsync(employeeCommand, CancellationToken.None).ConfigureAwait(false);


        }


    }
}
