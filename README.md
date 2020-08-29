EventFlowApi : It is write api . Configured with EventFlow , EventStore and RabbitMq.  s.

EventFlowApi.Read: It is read api, Configured with Eventflow and Elastic Search. It configured a RabbitMq subscriber at startup.which invoked Domain event subscribers when data is arrived.
and insert the data in ElasticSearch.

