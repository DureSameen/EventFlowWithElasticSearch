using System;
using EventFlow.Logs;

namespace EventFlowApi.Core.Aggregates.Queries
{
    public class ScopedContext : IScopedContext, IDisposable
    {
        private readonly ILog _log;
        private bool _isDisposed;

        public string Id { get; } = Guid.NewGuid().ToString();

        public ScopedContext(ILog log)
        {
            _log = log;

            _log.Information($"Scoped context {Id} created");
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException($"Scoped context {Id} is already disposed");

            _isDisposed = true;

            _log.Information($"Scoped context {Id} was disposed");
        }
    }
}
