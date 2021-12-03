using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Models;

namespace DocumentServiceListener.Boundary.Request
{
    public class SnsEvent
    {
        public string EventType { get; set; }
        public User User { get; set; }
        public Guid TargetId { get; set; }
        public DateTime DateTime { get; set; }
        public EventData EventData { get; set; }
    }

    public class EventData
    {
        public object OldData { get; set; }
        public object NewData { get; set; }
    }

    public static class EventTypes {
        public const string DeleteDirectoryEvent = "DeleteDirectoryEvent";
    }
}
