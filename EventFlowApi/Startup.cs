using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventFlow;
using EventFlow.AspNetCore.Extensions;
using EventFlow.AspNetCore.Middlewares;
using EventFlow.Autofac.Extensions;
using EventFlow.Configuration;
using EventFlow.Extensions;
using EventFlow.RabbitMQ;
using EventFlow.RabbitMQ.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using EventFlowApi.Core;
using EventFlowApi.Core.Aggregates.Entities;
using EventFlowApi.Core.Aggregates.Locator;
using EventFlowApi.Core.Aggregates.Queries;
using EventFlowApi.Core.ReadModels;
using EventFlowApi.Infrastructure;
using EventFlowApi.EventStore.Extensions;
using Microsoft.OpenApi.Models;
using Nest;
using EventFlow.Elasticsearch.Extensions;
using EventFlowApi.ElasticSearch.Index;

namespace EventFlowApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {   services.AddControllers();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Eventflow Demo - API", Version = "v1" });
                x.OperationFilter<SwaggerAuthorizationHeaderParameterOperationFilter>();
                 
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            ContainerBuilder containerBuilder = new ContainerBuilder();
            string rabbitMqConnection = Environment.GetEnvironmentVariable("RABBITMQCONNECTION");
             string elasticSearchUrl = Environment.GetEnvironmentVariable("ELASTICSEARCHURL");
 
            Uri node = new Uri(elasticSearchUrl);
            ConnectionSettings settings = new ConnectionSettings(node);

            settings.DisableDirectStreaming();

            ElasticClient elasticClient = new ElasticClient(settings);
            EventFlowOptions.New
                .UseAutofacContainerBuilder(containerBuilder)
                .AddDefaults(typeof(Employee).Assembly)
                .ConfigureEventStore()
                .ConfigureElasticsearch(() => elasticClient)
                .PublishToRabbitMq(
                    RabbitMqConfiguration.With(new Uri(rabbitMqConnection),
                        true, 5, "eventflow"))
                .RegisterServices(sr => sr.Register<IScopedContext, ScopedContext>(Lifetime.Scoped))
                .RegisterServices(sr => sr.RegisterType(typeof(EmployeeLocator)))
                .UseElasticsearchReadModel<EmployeeReadModel, EmployeeLocator>()
                .RegisterServices(sr => sr.RegisterType(typeof(TransactionLocator)))
                .UseElasticsearchReadModel<TransactionReadModel, TransactionLocator>()
                 .AddAspNetCore ();

            containerBuilder.Populate(services);
            var _tenantIndex = new ElasticSearchIndex(elasticSearchUrl);
            _tenantIndex.CreateIndex("employeeindex", elasticSearchUrl);
            services.AddSingleton(_tenantIndex.ElasticClient);
            return new AutofacServiceProvider(containerBuilder.Build());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
        {
            app.UseDeveloperExceptionPage(); 
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "API Version 1");

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
