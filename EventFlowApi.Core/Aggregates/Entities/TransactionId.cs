using EventFlow.Core;

namespace EventFlowApi.Core.Aggregates.Entities
{
    public class TransactionId : Identity<TransactionId>
    {
        public TransactionId(string value)
            : base(value)
        {

        }
    }
}
