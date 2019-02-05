using EventFlow.Commands;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Commands
{
    public class TransactionAddCommand : Command<TransactionAggregate, TransactionId>
    {
        public Transaction Transaction { get; }

        public TransactionAddCommand(TransactionId aggregateId, Transaction transactionRecord)
            : base(aggregateId)
        {
            Transaction = transactionRecord;
        }
    }
}
