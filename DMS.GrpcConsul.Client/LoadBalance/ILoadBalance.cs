namespace DMS.GrpcConsul.Client.LoadBalance
{
    /*
     * 负载均衡接口
     */
    public interface ILoadBalance
    {
        string GetGrpcService(string ServiceName);
    }
}
