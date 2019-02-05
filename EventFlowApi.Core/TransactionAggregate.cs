using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Exceptions;
using EventFlow.Snapshots;
using EventFlow.Snapshots.Strategies;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;
using EventFlowApi.Core.Aggregates.Queries;
using EventFlowApi.Core.Aggregates.Snapshots;

namespace EventFlowApi.Core
{
    [AggregateName("transaction")]
    public class TransactionAggregate : SnapshotAggregateRoot<TransactionAggregate, TransactionId, TransactionSnapshot>, IEmit<EmployeeDomainErrorAfterFirstEvent>
    {
        private readonly IScopedContext _scopedContext;
        private readonly List<Transaction> _transactionsReceived = new List<Transaction>();

        public const int SnapshotEveryVersion = 10;
        public bool DomainErrorAfterFirstReceived { get; private set; }
        public IReadOnlyCollection<Transaction> TransactionsReceived => _transactionsReceived;
        public IReadOnlyCollection<TransactionSnapshotVersion> SnapshotVersions { get; private set; } = new TransactionSnapshotVersion[] { };

        public TransactionAggregate(TransactionId id, IScopedContext scopedContext)
            : base(id, SnapshotEveryFewVersionsStrategy.With(SnapshotEveryVersion))
        {
            _scopedContext = scopedContext;

            Register<TransactionAddedEvent>(e => _transactionsReceived.Add(e.Transaction));
        }

        public void DomainErrorAfterFirst()
        {
            if (DomainErrorAfterFirstReceived)
            {
                throw DomainError.With("DomainErrorAfterFirst already received!");
            }

            Emit(new TransactionDomainErrorAfterFirstEvent());
        }

        public void AddTransaction(Transaction transaction)
        {
            Emit(new TransactionAddedEvent(transaction));
        }

        public IExecutionResult TransactionMaybe(Employee employee, Transaction transaction, bool isSuccess)
        {
            Emit(new TransactionAddedEvent(transaction));

            return isSuccess ? ExecutionResult.Success() : ExecutionResult.Failed();
        }

        public void Apply(EmployeeDomainErrorAfterFirstEvent e)
        {
            DomainErrorAfterFirstReceived = true;
        }

        protected override Task<TransactionSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new TransactionSnapshot(
                TransactionsReceived,
                Enumerable.Empty<TransactionSnapshotVersion>()));
        }

        protected override Task LoadSnapshotAsync(TransactionSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
        {
            _transactionsReceived.AddRange(snapshot.TransactionsReceived);

            SnapshotVersions = snapshot.PreviousVersions;

            return Task.FromResult(0);
        }
    }
}
