using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlowApi.Dto
{
    public class EmployeeResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<TransactionResponse> Transactions { get; set; }
    }
}
