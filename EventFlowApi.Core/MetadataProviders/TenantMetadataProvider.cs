using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.EventStores;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
 
namespace EventFlowApi.Core.MetadataProviders
{
    public class TenantMetadataProvider : IMetadataProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private static readonly IEnumerable<string> HeaderPriority = new[]
        //{
        //    "X-Forwarded-For",
        //    "HTTP_X_FORWARDED_FOR",
        //    "X-Real-IP",
        //    "REMOTE_ADDR"
        //};

        public TenantMetadataProvider(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<KeyValuePair<string, string>> ProvideMetadata<TAggregate, TIdentity>(
            TIdentity id,
            IAggregateEvent aggregateEvent,
            IMetadata metadata)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            

            //var headerInfo = HeaderPriority
            //    .Select(h =>
            //    {
            //        StringValues value;
            //        var address = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(h, out value)
            //            ? string.Join(string.Empty, value)
            //            : string.Empty;
            //        return new { Header = h, Address = address };
            //    })
            //    .FirstOrDefault(a => !string.IsNullOrEmpty(a.Address));

            //if (headerInfo == null)
            //{
            //    yield break;
            //}
            string tenantKey = "tenant_Id";

           
            yield return new KeyValuePair<string, string>(tenantKey, "tenant-" + _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString().Replace(".", ""));
         
        }
    }
     
   
}
