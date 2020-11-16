using System.Collections.Generic;
using EventFlow.Aggregates;
using EventFlow.ReadStores;
using EventFlowApi.Core.Aggregates.Events;

namespace EventFlowApi.Core.Aggregates.Locator
{
    public class EmployeeLocator : IReadModelLocator
    {
        public IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
        {
            IAggregateEvent aggregateEvent = domainEvent.GetAggregateEvent();

            switch (aggregateEvent)
            {
              
                case EmployeeAddedEvent employeeRecordAddedEvent:
                    yield return employeeRecordAddedEvent.EmployeeRecord.Id.Value;
                    break;



            }
        }
    }
}
