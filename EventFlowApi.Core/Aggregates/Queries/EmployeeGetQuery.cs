using EventFlow.Queries;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Queries
{
    public class EmployeeGetQuery : IQuery<Employee>
    {
        public EmployeeId EmployeeId { get; }

        public EmployeeGetQuery(EmployeeId employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
