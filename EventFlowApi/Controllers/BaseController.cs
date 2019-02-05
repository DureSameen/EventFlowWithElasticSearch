using EventFlow;
using EventFlow.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EventFlowApi.Controllers
{
    /// <summary>Base controller that provide command bus and query processor</summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// The command bus, execute commands
        /// </summary>
        protected readonly ICommandBus CommandBus;
        /// <summary>
        /// The query processor, data retrieval manager
        /// </summary>
        protected readonly IQueryProcessor QueryProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="commandBus">The command bus.</param>
        /// <param name="queryProcessor">The query processor.</param>
        public BaseController(ICommandBus commandBus,
            IQueryProcessor queryProcessor)
        {
            CommandBus = commandBus;
            QueryProcessor = queryProcessor;
        }
    }
}
