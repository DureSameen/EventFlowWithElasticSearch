using EventFlowApi.EventStore.Extensions;
using System;
using System.Reflection;

namespace EventFlowApi.EventStore.EventStore
{
    /// <summary>
    /// Configuration of data retrieval process.
    /// </summary>
    public class DataRetrievalConfiguration : IDataRetrievalConfiguration
    {
        /// <summary>
        /// Disable or enabled the process.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// From range of data retrieval
        /// </summary>
        public DateTime? FromDate { get; set; }
        /// <summary>
        ///  To Range of the data retrieval process.
        /// </summary>
        public DateTime? ToDate { get; set; }

        public Assembly ReadModelAssembly { get; set; }
        public DataRetrievalConfiguration()
        {
        }
        /// <summary>
        /// This constructor get data retrieval configuration from appsettings files and set assembly.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="readModelAssembly"></param>
        public DataRetrievalConfiguration(DataRetrieval dataRetrieval, Assembly readModelAssembly)
        {
            Enabled = dataRetrieval.Enabled;
            ReadModelAssembly = readModelAssembly;
        }
    }
}
