using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlowApi.Core;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;
using Nest;

namespace EventFlowApi.ElasticSearch.ReadModels
{    
    public class EmployeeReadModel : IReadModel, IAmReadModelFor<EmployeeAggregate, EmployeeId, EmployeeAddedEvent>
    {
        [Keyword(
            Index = true)]
        public string TenantId { get; set; }
        public string Id { get;   set; }
        public string FirstName { get;   set; }
        public string LastName { get;  set; }

        public void Apply(IReadModelContext context, IDomainEvent<EmployeeAggregate, EmployeeId, EmployeeAddedEvent> domainEvent)
        {
            TenantId = domainEvent.Metadata["tenant_Id"];
            Id = domainEvent.AggregateEvent.EmployeeRecord.Id.Value;
            FirstName = domainEvent.AggregateEvent.EmployeeRecord.FirstName;
            LastName = domainEvent.AggregateEvent.EmployeeRecord.LastName;
        }

        public Employee ToEmployee()
        {
            return new Employee(EmployeeId.With(Id), FirstName, LastName, TenantId);
        }
    }
}
