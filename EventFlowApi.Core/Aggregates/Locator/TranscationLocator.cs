using System.Collections.Generic;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlowApi.Core.Aggregates.Events;

namespace EventFlowApi.Core.Aggregates.Locator
{
    public class TransactionLocator : IReadModelLocator
    {
        public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
        {
            IAggregateEvent aggregateEvent = domainEvent.GetAggregateEvent();

            switch (aggregateEvent)
            {

                case TransactionAddedEvent transactionRecordAddedEvent:
                    yield return transactionRecordAddedEvent.Transaction.Id.Value;
                    break;

            }
        }
    }
}
