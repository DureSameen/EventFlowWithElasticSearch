using EventFlow.Core;

namespace EventFlowApi.Core.Aggregates.Entities
{
    public class EmployeeId : Identity<EmployeeId>
    {
        public EmployeeId(string value) 
            : base(value)
        {

        }
    }
}
