using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.GrpcConsul.Client.Framework.Entity
{
    public class GrpcServiceSettings
    {
        public List<GrpcService> GrpcServices { get; set; }
    }

    public class GrpcService
    {
        public string ServiceName { get; set; }
        public string ServiceID { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int Weight { get; set; }
    }
}
