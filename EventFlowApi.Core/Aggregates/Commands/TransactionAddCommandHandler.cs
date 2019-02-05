using System.Threading;
using System.Threading.Tasks;
using EventFlow.Commands;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Commands
{
    public class TransactionAddCommandHandler : CommandHandler<TransactionAggregate, TransactionId, TransactionAddCommand>
    {
        public override Task ExecuteAsync(TransactionAggregate aggregate, TransactionAddCommand command, CancellationToken cancellationToken)
        {
            aggregate.AddTransaction(command.Transaction);

            return Task.FromResult(0);
        }
    }
}
