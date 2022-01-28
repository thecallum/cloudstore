using System.Collections.Generic;
using TokenService.Models;

namespace DocumentService.Gateways
{
    public class CloudStoreSnsEvent
    {
        public string EventName { get; set; }
        public User User { get; set; }
        public Dictionary<string, object> Body { get; set; }
    }
}
