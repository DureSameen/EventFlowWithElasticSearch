using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlowApi.EventStore.Extensions
{
    public class DataRetrieval
    {
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }

}
