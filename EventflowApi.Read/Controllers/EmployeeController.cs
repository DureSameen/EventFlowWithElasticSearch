using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventflowApi.Read.Dto;
using EventFlow;
using EventFlow.Queries;
using EventFlowApi.Core.Aggregates.Commands;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Queries;

using Microsoft.AspNetCore.Mvc;

namespace EventflowApi.Read.Controllers
{
    /// <summary>Employee api, collection of all related operations</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        /// <summary>employee api cons.</summary>
        /// <param name="commandBus"></param>
        /// <param name="queryProcessor"></param>
        public EmployeeController(ICommandBus commandBus,
              IQueryProcessor queryProcessor)
              : base(commandBus, queryProcessor)
        {

        }

        /// <summary>
        ///  Employee information retrieval.
        /// </summary>
        /// <param name="employeeId">The string employee Id</param>
        /// <returns>return employee and his transactions</returns>
        [HttpGet]
        public async Task<EmployeeResponse> Get(string employeeId)
        {
            var readModel = await QueryProcessor.ProcessAsync(
                       new EmployeeGetQuery(new EmployeeId(employeeId)), CancellationToken.None)
                       .ConfigureAwait(false);

            var transactionsReadModel = await QueryProcessor.ProcessAsync(
                      new TransactionsGetQuery(new EmployeeId(employeeId)), CancellationToken.None)

                      .ConfigureAwait(false);

            var transactions = new List<TransactionResponse>();

            if (transactionsReadModel != null) transactions.AddRange(transactionsReadModel.Select(transaction => new TransactionResponse(transaction.Date, transaction.Salary)));
            var response = new EmployeeResponse { Id = readModel.Id.GetGuid().ToString( ), FirstName = readModel.FirstName, LastName = readModel.LastName, Transactions= transactions };
            return response;
        }
        ///// <summary>
        ///// Add a new employee event.
        ///// </summary>
        ///// <param name="request">create employee request</param>
        ///// <returns>employeeid</returns>
        //[HttpPost]
        //public async Task<EmployeeId> Post(CreateEmployeeRequest request)
        //{
        //    var id = Guid.NewGuid().ToString();
        //    var employeeId = new EmployeeId("employee-" + id);
        //    var employeeRecord  = new Employee(employeeId, request.FirstName, request.LastName);
        //    var employeeCommand = new EmployeeAddCommand(employeeId, employeeRecord);

        //    await CommandBus.PublishAsync(employeeCommand, CancellationToken.None).ConfigureAwait(false);

        //    return employeeId;
           
        //}
    }
}