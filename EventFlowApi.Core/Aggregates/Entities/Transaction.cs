using System;
using EventFlow.Entities;

namespace EventFlowApi.Core.Aggregates.Entities
{
    public class Transaction : Entity<TransactionId>
    {
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public double Salary { get; set; }

        public Transaction(string employeeId, TransactionId id, DateTime date, double salary)
            : base(id)
        {
            if (salary <= 0 ) throw new ArgumentNullException(nameof(salary));
    
            Date = date;
            Salary = salary;
            EmployeeId = employeeId;
        }
    }
}
