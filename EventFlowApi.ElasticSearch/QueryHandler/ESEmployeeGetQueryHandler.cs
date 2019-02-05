using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Elasticsearch.ReadStores;
using EventFlow.Elasticsearch.ValueObjects;
using EventFlow.Queries;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Queries;
using EventFlowApi.ElasticSearch.ReadModels;
using Nest;

namespace EventFlowApi.ElasticSearch.QueryHandler
{
    public class ESEmployeeGetQueryHandler : IQueryHandler<EmployeeGetQuery, Employee>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IReadModelDescriptionProvider _readModelDescriptionProvider;

        public ESEmployeeGetQueryHandler(IElasticClient elasticClient, IReadModelDescriptionProvider readModelDescriptionProvider)
        {
            _elasticClient = elasticClient;
            _readModelDescriptionProvider = readModelDescriptionProvider;
        }

        public async Task<Employee> ExecuteQueryAsync(EmployeeGetQuery query, CancellationToken cancellationToken)
        {
            ReadModelDescription readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<EmployeeReadModel>();
            string indexName = "eventflow-" + readModelDescription.IndexName.Value;

            await _elasticClient.FlushAsync(indexName, 
                    d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken)
                    .ConfigureAwait(false);

            await _elasticClient.RefreshAsync(indexName, 
                    d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken)
                    .ConfigureAwait(false);

            IGetResponse<EmployeeReadModel> searchResponse = await _elasticClient.GetAsync<EmployeeReadModel>(query.EmployeeId.Value, 
                d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)).Index(readModelDescription.IndexName.Value), cancellationToken)
                .ConfigureAwait(false);

            return searchResponse.Source.ToEmployee();
        }
    }
}
