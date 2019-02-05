using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventFlow.Snapshots;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Snapshots
{
    [SnapshotVersion("transaction", 3)]
    public class TransactionSnapshot : ISnapshot
    {
        public IReadOnlyCollection<Transaction> TransactionsReceived { get; }
        public IReadOnlyCollection<TransactionSnapshotVersion> PreviousVersions { get; }

        public TransactionSnapshot(IEnumerable<Transaction> transactionsReceived, IEnumerable<TransactionSnapshotVersion> previousVersions)
        {
            TransactionsReceived = (transactionsReceived ?? Enumerable.Empty<Transaction>()).ToList();
            PreviousVersions = (previousVersions ?? Enumerable.Empty<TransactionSnapshotVersion>()).ToList();
        }
    }
}
