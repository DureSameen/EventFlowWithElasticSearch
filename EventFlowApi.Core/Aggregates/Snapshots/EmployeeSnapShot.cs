using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventFlow.Snapshots;
using EventFlowApi.Core.Aggregates.Entities;

namespace EventFlowApi.Core.Aggregates.Snapshots
{
    [SnapshotVersion("employee", 3)]
    public class EmployeeSnapshot : ISnapshot
    {
        public IReadOnlyCollection<Employee> EmployeesAdded { get; }
        public IReadOnlyCollection<EmployeeSnapshotVersion> PreviousVersions { get; }

        public EmployeeSnapshot(IEnumerable<Employee> employeesAdded, IEnumerable<EmployeeSnapshotVersion> previousVersions)
        {
            EmployeesAdded = (employeesAdded ?? Enumerable.Empty<Employee>()).ToList();
            PreviousVersions = (previousVersions ?? Enumerable.Empty<EmployeeSnapshotVersion>()).ToList();
        }
    }
}
