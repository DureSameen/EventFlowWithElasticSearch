using System;
using System.Collections.Generic;
using System.Text;
using Elasticsearch.Net;
using EventFlowApi.Core.ReadModels;
using Nest;

namespace EventFlowApi.ElasticSearch.Index
{
    public class ElasticSearchIndex
    {
         
            public IElasticClient ElasticClient { get; }

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="esUrl"></param> 

            public ElasticSearchIndex(string esUrl)
            {
                var connectionPool = new SingleNodeConnectionPool(new Uri(esUrl)); 
                using var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming(); 
                var client = new ElasticClient(connectionSettings);
                ElasticClient = client;
            }
            /// <summary>
            /// create index of given name, if force creation is true then existing index will be deleted and then recreated.
            /// </summary> 
            /// <returns></returns>
            public IElasticClient CreateIndex(string indexName, string esUrl)
            {
                var connectionPool = new SingleNodeConnectionPool(new Uri(esUrl));
                using var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming();
            connectionSettings
            .DefaultMappingFor<EmployeeReadModel>(m => m.IndexName(indexName))
            .DefaultMappingFor<TransactionReadModel>(m => m.IndexName(indexName));
               

                var client = new ElasticClient(connectionSettings);

            if (!IndexExists(indexName, client))
            {
                var createIndexResponse = client.CreateIndex(indexName);
                                                             
   
                if (!createIndexResponse.ApiCall.Success)
                    throw createIndexResponse.ApiCall.OriginalException;
            }
                return ElasticClient;
            }


            private bool IndexExists(string name, IElasticClient client)
            {
                var aliasResponse = client.IndexExists (name);

                return aliasResponse.Exists;

            }
         


        }
    }
