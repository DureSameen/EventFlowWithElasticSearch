using EventFlow.Aggregates;
using EventFlow.EventStores;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    [EventVersion("EmployeeDomainErrorAfterFirst", 1)]
    public class EmployeeDomainErrorAfterFirstEvent : AggregateEvent<EmployeeAggregate, EmployeeId>
    {
    }
}
