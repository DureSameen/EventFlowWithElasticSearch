using EventFlow.Aggregates;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    public class EmployeeSagaCompleteRequestedEvent : AggregateEvent<EmployeeAggregate, EmployeeId>
    {
    }
}
