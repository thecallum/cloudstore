using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace DocumentServiceListener.Boundary
{
    public class CloudStoreSnsEvent
    {
        public string EventName { get; set; }
        public User User { get; set; }
        public Dictionary<string, object> Body { get; set; }
    }
}
