using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlowApi.Dto
{
    public class CreateTransactionRequest
    {
        public string  EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public double Salary { get; set; }
    }
}
