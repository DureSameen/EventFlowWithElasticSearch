using EventFlow.Aggregates;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    public class TransactionAddedEvent : AggregateEvent<TransactionAggregate, TransactionId>
    {
        public Transaction Transaction { get; set; }

        public TransactionAddedEvent(Transaction transaction )
        { 
            Transaction = transaction;
        }
    }
}
