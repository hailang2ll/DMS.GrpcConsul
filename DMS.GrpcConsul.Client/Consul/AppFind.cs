using DMS.GrpcConsul.Client.Consul.Entity;
using DMS.GrpcConsul.Client.Framework.Entity;
using DMS.GrpcConsul.Client.Utils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.GrpcConsul.Client.Consul
{
    /*
     * 服务发现
     * (服务和健康信息)http://localhost:8500/v1/health/service/GrpcService
     * (健康信息)http://localhost:8500/v1/health/checks/GrpcService
     */
    public class AppFind: IAppFind
    {
        static IOptions<GrpcServiceSettings> GrpcSettings;
        static IOptions<ConsulService> ConsulSettings;

        public AppFind(IOptions<GrpcServiceSettings> grpcSettings, IOptions<ConsulService> consulSettings)
        {
            GrpcSettings = grpcSettings;
            ConsulSettings = consulSettings;
        }
        
        public IEnumerable<string> FindConsul(string ServiceName)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            var consul = ConsulSettings.Value;
            string findUrl = $"http://{consul.IP}:{consul.Port}/v1/health/checks/{ServiceName}";

            string findResult = HttpHelper.HttpGet(findUrl, headers, 5);
            if (findResult.Equals(""))
            {
                var grpcServices = GrpcSettings.Value.GrpcServices;
                return grpcServices.Where(g=>g.ServiceName.Equals(ServiceName,StringComparison.CurrentCultureIgnoreCase)).Select(s => s.ServiceID);
            }

            var findCheck = JsonConvert.DeserializeObject<List<HealthCheck>>(findResult);

            return findCheck.Where(g => g.Status.Equals("passing", StringComparison.CurrentCultureIgnoreCase)).Select(g => g.ServiceID);
        }
    }
}
