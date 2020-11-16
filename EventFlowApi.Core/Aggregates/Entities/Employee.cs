using System;
using EventFlow.Entities;

namespace EventFlowApi.Core.Aggregates.Entities
{
    public class Employee : Entity<EmployeeId>
    {
        public string FirstName { get; }
        public string LastName { get; }

        public Employee(EmployeeId id, string firstName, string lastName)
            : base(id)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrEmpty(lastName)) throw new ArgumentNullException(nameof(lastName));

            FirstName = firstName;
            LastName = lastName;
        }
    }
}
