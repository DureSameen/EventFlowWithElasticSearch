using System;

namespace EventflowApi.Read.Dto
{
    public class TransactionResponse
    {
        public string TenantId { get;  }
        public DateTime Date { get; }
        public double Salary { get; }

        public TransactionResponse(DateTime date, double salary, string tenantId)
        {
            Date = date;
            Salary = salary;
            TenantId = tenantId;
        }
    }
}
