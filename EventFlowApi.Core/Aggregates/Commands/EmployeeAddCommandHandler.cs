using System.Threading;
using System.Threading.Tasks;
using EventFlow.Commands;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Commands
{
    public class EmployeeAddCommandHandler : CommandHandler<EmployeeAggregate, EmployeeId, EmployeeAddCommand>
    {
        public override Task ExecuteAsync(EmployeeAggregate aggregate, EmployeeAddCommand command, CancellationToken cancellationToken)
        {
            aggregate.AddRecord(command.EmployeeRecord);

            return Task.FromResult(0);
        }
    }
}
