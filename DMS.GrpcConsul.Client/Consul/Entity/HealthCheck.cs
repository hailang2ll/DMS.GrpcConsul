using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.GrpcConsul.Client.Consul.Entity
{
    public class HealthCheck
    {
        public string Node { get; set; }
        public string CheckID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Output { get; set; }
        public string ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string[] ServiceTags { get; set; }
        public dynamic Definition { get; set; }
        public int CreateIndex { get; set; }
        public int ModifyIndex { get; set; }
    }
}
