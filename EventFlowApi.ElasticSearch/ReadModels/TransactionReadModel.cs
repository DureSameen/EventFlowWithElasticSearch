using System;
using System.ComponentModel.DataAnnotations;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlowApi.Core;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;
using Nest;

namespace EventFlowApi.ElasticSearch.ReadModels
{ 
    public class TransactionReadModel : IReadModel, IAmReadModelFor<TransactionAggregate, TransactionId, TransactionAddedEvent>
    {
        [Keyword(
            Index = true)]
        public string TenantId { get; set; }
        public string TransactionId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public double Salary { get; set; }

        public void Apply(IReadModelContext context, IDomainEvent<TransactionAggregate, TransactionId, TransactionAddedEvent> domainEvent)
        {
            TenantId = domainEvent.Metadata["tenant_Id"];
            TransactionId = domainEvent.AggregateEvent.Transaction.Id.ToString();
            EmployeeId  =   domainEvent.AggregateEvent.Transaction.EmployeeId;
            Date = domainEvent.AggregateEvent.Transaction.Date;
            Salary = domainEvent.AggregateEvent.Transaction.Salary ;
        }

        public Transaction ToTransaction()
        {
            return new Transaction(EmployeeId, new TransactionId(TransactionId), Date, Salary );;
        }
    }
}
