using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.GrpcConsul.Hosting.Consul;
using DMS.GrpcConsul.Hosting.Consul.Entity;
using DMS.GrpcConsul.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMS.GrpcConsul.Hosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container. 
        public void ConfigureServices(IServiceCollection services)
        {
            //注册全局配置
            services.AddOptions();
            services.Configure<Impl.Entity.GrpcService>(Configuration.GetSection(nameof(Impl.Entity.GrpcService)));
            services.Configure<HealthService>(Configuration.GetSection(nameof(HealthService)));
            services.Configure<ConsulService>(Configuration.GetSection(nameof(ConsulService)));

            //注册Rpc服务
            services.AddSingleton<IRpcConfig, RpcConfig>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IOptions<HealthService> healthService, IOptions<ConsulService> consulService, IRpcConfig rpc)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // 添加健康检查路由地址
            app.Map("/health", HealthMap);

            // 服务注册
            app.RegisterConsul(lifetime, healthService, consulService);

            // 启动Rpc服务
            rpc.Start();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void HealthMap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("OK");
            });
        }
    }
}
