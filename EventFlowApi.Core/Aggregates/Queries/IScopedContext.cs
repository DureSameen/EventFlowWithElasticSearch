using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlowApi.Core.Aggregates.Queries
{
    public interface IScopedContext
    {
        string Id { get; }
    }
}
