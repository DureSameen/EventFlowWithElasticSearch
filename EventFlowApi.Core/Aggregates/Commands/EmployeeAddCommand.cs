using EventFlow.Commands;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Commands
{
    public class EmployeeAddCommand : Command<EmployeeAggregate, EmployeeId>
    {
        public Employee EmployeeRecord { get; }

        public EmployeeAddCommand(EmployeeId aggregateId, Employee employeeRecord)
            : base(aggregateId)
        {
            EmployeeRecord = employeeRecord;
        }
    }
}
