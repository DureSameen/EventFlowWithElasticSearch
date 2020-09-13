using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventflowApi.Read.Subscribers;
using EventFlow;
using EventFlow.AspNetCore.Extensions;
using EventFlow.AspNetCore.Middlewares;
using EventFlow.Autofac.Extensions;
using EventFlow.Configuration;
using EventFlow.Elasticsearch.Extensions;
using EventFlow.Extensions;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using EventFlow.RabbitMQ.Integrations;
using EventFlow.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Swashbuckle.AspNetCore.Swagger;
using EventFlowApi.Core;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Events;
using EventFlowApi.Core.Aggregates.Locator;
using EventFlowApi.Core.Aggregates.Queries;
using EventFlowApi.Core.ReadModels;
using EventFlowApi.ElasticSearch.QueryHandler;
using Microsoft.OpenApi.Models;


namespace EventFlowApi.Read
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IEventFlowOptions Options { get; set; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {   
            services.AddControllers();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Read Api Eventflow Demo - API", Version = "v1" });
                
                
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            string elasticSearchUrl = Environment.GetEnvironmentVariable("ELASTICSEARCHURL");
            ContainerBuilder containerBuilder = new ContainerBuilder();
            Uri node = new Uri(elasticSearchUrl);
            ConnectionSettings settings = new ConnectionSettings(node);

            settings.DisableDirectStreaming();

            ElasticClient elasticClient = new ElasticClient(settings);
            string rabbitMqConnection = Environment.GetEnvironmentVariable("RABBITMQCONNECTION");

              Options = EventFlowOptions.New
                .UseAutofacContainerBuilder(containerBuilder)
                .AddDefaults(typeof(Employee).Assembly)
                .ConfigureElasticsearch(() => elasticClient)
                .RegisterServices(sr => sr.Register<IScopedContext, ScopedContext>(Lifetime.Scoped))
                .RegisterServices(sr => sr.RegisterType(typeof(EmployeeLocator)))
                .UseElasticsearchReadModel<EmployeeReadModel, EmployeeLocator>()
                .UseElasticsearchReadModel<TransactionReadModel, EmployeeLocator>()
                .AddQueryHandlers(typeof(ESTransactionGetQueryHandler), typeof(ESEmployeeGetQueryHandler))
                .AddAsynchronousSubscriber<EmployeeAggregate, EmployeeId, EmployeeAddedEvent, AddNewEmployeeSubscriber>()
                .AddSubscribers (typeof(AllEventsSubscriber))
                .Configure(c => c.IsAsynchronousSubscribersEnabled = true)
                .Configure(c => c.ThrowSubscriberExceptions = true)
                .SubscribeToRabbitMq(
                    RabbitMqConfiguration.With(new Uri(rabbitMqConnection),
                        true, 5, "eventflow"))
                
                .AddAspNetCore ();

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var subscriber = scope.Resolve<IRabbitMqSubscriber>();
                var configuration = scope.Resolve<IRabbitMqConfiguration>();
                var domainEventPublisher = scope.Resolve<IDomainEventPublisher>();
                subscriber.SubscribeAsync(configuration.Exchange, configuration.Exchange + "Queue", EventFlowOptionsRabbitMqExtensions.Listen,
                    domainEventPublisher, cancellationToken: CancellationToken.None).Wait();
            }
          
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
        {

            app.UseDeveloperExceptionPage(); 
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Read API Version 1");

            });

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); 
         
            app.UseRouting();
            app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
           
        }
    }
}
