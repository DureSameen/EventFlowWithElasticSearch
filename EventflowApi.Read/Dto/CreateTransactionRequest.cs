using System;

namespace EventflowApi.Read.Dto
{
    public class CreateTransactionRequest
    {
        public string  EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public double Salary { get; set; }
    }
}
