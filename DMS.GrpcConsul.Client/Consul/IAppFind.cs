using System.Collections.Generic;

namespace DMS.GrpcConsul.Client.Consul
{
    public interface IAppFind
    {
        IEnumerable<string> FindConsul(string ServiceName);
    }
}
