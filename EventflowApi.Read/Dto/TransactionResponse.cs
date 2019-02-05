using System;

namespace EventflowApi.Read.Dto
{
    public class TransactionResponse
    {
        public DateTime Date { get; }
        public double Salary { get; }

        public TransactionResponse(DateTime date, double salary)
        {
            Date = date;
            Salary = salary;
        }
    }
}
