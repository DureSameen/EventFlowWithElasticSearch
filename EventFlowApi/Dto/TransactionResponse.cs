using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlowApi.Dto
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
