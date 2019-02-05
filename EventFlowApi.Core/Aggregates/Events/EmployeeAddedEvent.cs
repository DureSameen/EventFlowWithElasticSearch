using EventFlow.Aggregates;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    public class EmployeeAddedEvent : AggregateEvent<EmployeeAggregate, EmployeeId>
    {
        public Employee EmployeeRecord { get; }

        public EmployeeAddedEvent(Employee employeeRecord)
        {
            EmployeeRecord = employeeRecord;
        }
    }
}
