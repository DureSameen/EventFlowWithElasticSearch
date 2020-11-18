using System.Collections.Generic;
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
    public class ESTransactionGetQueryHandler : IQueryHandler<TransactionsGetQuery, IEnumerable<Transaction>>
    {
        private readonly IElasticClient _elasticClient;
        private readonly IReadModelDescriptionProvider _readModelDescriptionProvider;

        public ESTransactionGetQueryHandler(IElasticClient elasticClient, IReadModelDescriptionProvider readModelDescriptionProvider)
        {
            _elasticClient = elasticClient;
            _readModelDescriptionProvider = readModelDescriptionProvider;
        }

        public async Task<IEnumerable<Transaction>> ExecuteQueryAsync(TransactionsGetQuery query, CancellationToken cancellationToken)
        {
            ReadModelDescription readModelDescription = _readModelDescriptionProvider.GetReadModelDescription<TransactionReadModel>();
            string indexName = "eventflow-" + readModelDescription.IndexName.Value;

            await _elasticClient.FlushAsync(indexName,
                d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken)
                .ConfigureAwait(false);

            await _elasticClient.RefreshAsync(indexName,
                d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)), cancellationToken)
                .ConfigureAwait(false);

            IGetResponse<IEnumerable<Transaction>> searchResponse = await _elasticClient.GetAsync<IEnumerable<Transaction>>(query.EmployeeId.Value, 
                d => d.RequestConfiguration(c => c.AllowedStatusCodes((int)HttpStatusCode.NotFound)).Index(readModelDescription.IndexName.Value), cancellationToken)
                .ConfigureAwait(false);

            return searchResponse.Source;
        }
    }
}
