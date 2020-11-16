using System;
using System.Collections.Generic;
using System.Text;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlowApi.Core.Aggregates;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;

namespace EventFlowApi.Core.ReadModels
{
    public class EmployeeReadModel : IReadModel, IAmReadModelFor<EmployeeAggregate, EmployeeId, EmployeeAddedEvent> 
    {
        public string Id { get;   set; }
        public string FirstName { get;   set; }
        public string LastName { get;  set; }

        public void Apply(IReadModelContext context, IDomainEvent<EmployeeAggregate, EmployeeId, EmployeeAddedEvent> domainEvent)
        {
            Id = domainEvent.AggregateEvent.EmployeeRecord.Id.Value;
            FirstName = domainEvent.AggregateEvent.EmployeeRecord.FirstName;
            LastName = domainEvent.AggregateEvent.EmployeeRecord.LastName;
        }

        public Employee ToEmployee()
        {
            return new Employee(EmployeeId.With(Id), FirstName, LastName);
        }
    }
}
