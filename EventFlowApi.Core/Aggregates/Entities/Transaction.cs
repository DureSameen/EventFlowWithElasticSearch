using System;
using EventFlow.Entities;

namespace EventFlowApi.Core.Aggregates.Entities
{
    public class Transaction : Entity<TransactionId>
    {
        public string TenantId { get;  }
        public string EmployeeId { get;  }
        public DateTime Date { get;   }
        public double Salary { get;   }

        public Transaction(string employeeId, TransactionId id, DateTime date, double salary, string tenantId)
            : base(id)
        {
            if (salary <= 0 ) throw new ArgumentNullException(nameof(salary));
    
            Date = date;
            Salary = salary;
            EmployeeId = employeeId;
            TenantId = tenantId;
        }
    }
}
