using Consul;
using DMS.GrpcConsul.Hosting.Consul.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace DMS.GrpcConsul.Hosting.Consul
{
    public static class AppRregister
    {
        // 服务注册
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, IOptions<HealthService> healthService, IOptions<ConsulService> consulService)
        {
            var consulClient = new ConsulClient(x => x.Address = new Uri($"http://{consulService.Value.IP}:{consulService.Value.Port}"));//请求注册的 Consul 地址
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔，或者称为心跳间隔
                HTTP = $"http://{healthService.Value.IP}:{healthService.Value.Port}/health",//健康检查地址
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = healthService.Value.Name + "_" + healthService.Value.Port,
                Name = healthService.Value.Name,
                Address = healthService.Value.IP,
                Port = healthService.Value.Port,
                Tags = new[] { $"urlprefix-/{healthService.Value.Name}" }//添加 urlprefix-/servicename 格式的 tag 标签，以便 Fabio 识别
            };

            consulClient.Agent.ServiceRegister(registration).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();//服务停止时取消注册
            });

            return app;
        }
    }
}
