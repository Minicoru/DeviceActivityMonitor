using System;

namespace DeviceMonitor
{
    public class DeviceInfo
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ConnectionCount { get; set; }
        public int DisconnectionCount { get; set; }
        public bool IsCurrentlyConnected { get; set; }
    }

    public class DeviceEvent
    {
        public string EventType { get; set; } = string.Empty; // "Connected" or "Disconnected"
        public DateTime Timestamp { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
