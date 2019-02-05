using System.Collections.Generic;

namespace EventflowApi.Read.Dto
{
    public class EmployeeResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<TransactionResponse> Transactions { get; set; }
    }
}
