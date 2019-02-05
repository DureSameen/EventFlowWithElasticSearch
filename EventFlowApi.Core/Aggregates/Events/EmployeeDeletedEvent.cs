using EventFlow.Aggregates;
using EventFlow.EventStores;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    [EventVersion("EmployeeDeleted", 1)]
    public class EmployeeDeletedEvent : AggregateEvent<EmployeeAggregate, EmployeeId>
    {
    }
}
