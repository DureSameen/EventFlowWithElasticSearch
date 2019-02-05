using System;
using System.Threading;
using System.Threading.Tasks;
using EventflowApi.Read.Dto;
using EventFlow;
using EventFlow.Queries;
using EventFlowApi.Core.Aggregates.Commands;
using EventFlowApi.Core.Aggregates.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventflowApi.Read.Controllers
{
    /// <summary>Transactions detail of employee</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : BaseController
    {
        /// <summary>constructor</summary>
        /// <param name="commandBus"></param>
        /// <param name="queryProcessor"></param>
        public TransactionController(ICommandBus commandBus,
            IQueryProcessor queryProcessor)
            :base(commandBus, queryProcessor)
        {
            
        }
        /// <summary>
        /// Add new employee transactions, that a month's salary
        /// </summary>
        /// <param name="request">EmployeeId</param>
        /// <returns>transaction id</returns>
        [HttpPost]
        public async Task<TransactionId> Post(CreateTransactionRequest request)
        {

            var id = Guid.NewGuid().ToString();
            var transactionId = new TransactionId("transaction-" + id);
            var transactionRecord = new Transaction(request.EmployeeId, transactionId, request.Date, request.Salary);
            var transactionCommand = new TransactionAddCommand(transactionId, transactionRecord);

            await CommandBus.PublishAsync(transactionCommand, CancellationToken.None).ConfigureAwait(false);

            return transactionId;

        }
    }
}