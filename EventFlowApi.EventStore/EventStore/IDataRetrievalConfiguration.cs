using System;
using System.Reflection;

namespace EventFlowApi.EventStore.EventStore
{
    public interface IDataRetrievalConfiguration
    {
        bool Enabled { get; set; }
        DateTime? FromDate { get; set; }
        DateTime? ToDate { get; set; }
        Assembly ReadModelAssembly { get; set; }
    }
}
