using DMS.GrpcConsul.Client.Consul;
using DMS.GrpcConsul.Client.Framework.Entity;
using DMS.GrpcConsul.Client.LoadBalance;
using DMS.GrpcConsul.Client.RpcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DMS.GrpcConsul.Client.Framework
{
    /*
     *  IServiceCollection 依赖注入生命周期
     *  AddTransient 每次都是全新的
     *  AddScoped    在一个范围之内只有同一个实例(同一个线程,同一个浏览器请求只有一个实例)
     *  AddSingleton 单例
     */
    public static class DependencyInitialize
    {
        /// <summary>
        /// 注册对象
        /// </summary>
        /// <param name="services">The services.</param>
        /*
         * IAppFind AppFind;
         * 构造函数注入使用 IAppFind appFind
         * AppFind = appFind;
         */
        public static void AddImplement(this IServiceCollection services)
        {
            //添加 json 文件路径
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //创建配置根对象
            var configurationRoot = builder.Build();

            //注册全局配置
            services.AddConfigImplement(configurationRoot);

            //注册服务发现
            services.AddScoped<IAppFind, AppFind>();

            //注册负载均衡
            if (configurationRoot["LoadBalancer"].Equals("WeightRound", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddSingleton<ILoadBalance, WeightRoundBalance>();
            }

            //注册Rpc客户端
            services.AddTransient<IMsgClient, MsgClient>();
        }

        /// <summary>
        /// 注册全局配置
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configurationRoot">The configurationRoot.</param>
        /*  
         *  IOptions<GrpcServiceSettings> GrpcSettings;
         *  构造函数注入使用 IOptions<GrpcServiceSettings> grpcSettings
         *  GrpcSettings = grpcSettings;
         */
        public static void AddConfigImplement(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            //注册配置对象
            services.AddOptions();
            services.Configure<GrpcServiceSettings>(configurationRoot.GetSection(nameof(GrpcServiceSettings)));
            services.Configure<ConsulService>(configurationRoot.GetSection(nameof(ConsulService)));
        }
    }
}
