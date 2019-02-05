using EventFlow.Aggregates;
using EventFlow.EventStores;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Events
{
    [EventVersion("TransactionDomainErrorAfterFirst", 1)]
    public class TransactionDomainErrorAfterFirstEvent : AggregateEvent<TransactionAggregate, TransactionId>
    {
    }
}
