using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Queries;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Queries
{
    public class TransactionsGetQuery : IQuery<IEnumerable<Transaction>>
    {
        public EmployeeId EmployeeId { get; }

        public TransactionsGetQuery(EmployeeId employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
