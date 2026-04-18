using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DeviceMonitor
{
    public class AppData
    {
        public Dictionary<string, DeviceInfo> Devices { get; set; } = new Dictionary<string, DeviceInfo>();
        public List<DeviceEvent> Events { get; set; } = new List<DeviceEvent>();
    }

    public class DeviceDataManager
    {
        private readonly string _filePath;
        private AppData _data;
        private readonly object _lock = new object();

        public DeviceDataManager(string filePath = "device_history.json")
        {
            _filePath = filePath;
            _data = LoadData();
        }

        public AppData Data
        {
            get
            {
                lock (_lock)
                {
                    return _data;
                }
            }
        }

        public List<DeviceInfo> GetDeviceListSafe()
        {
            lock (_lock)
            {
                return new List<DeviceInfo>(_data.Devices.Values);
            }
        }

        public List<DeviceEvent> GetEventListSafe()
        {
            lock (_lock)
            {
                return new List<DeviceEvent>(_data.Events);
            }
        }

        private AppData LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            return new AppData();
        }

        public void SaveData()
        {
            lock (_lock)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(_data, options);
                    File.WriteAllText(_filePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving data: {ex.Message}");
                }
            }
        }

        public void AddEvent(DeviceEvent deviceEvent)
        {
            lock (_lock)
            {
                _data.Events.Add(deviceEvent);

                if (!_data.Devices.ContainsKey(deviceEvent.DeviceId))
                {
                    _data.Devices[deviceEvent.DeviceId] = new DeviceInfo
                    {
                        DeviceId = deviceEvent.DeviceId,
                        Name = deviceEvent.Name,
                        Description = deviceEvent.Description
                    };
                }

                var device = _data.Devices[deviceEvent.DeviceId];
                device.Name = deviceEvent.Name; // Update in case it changed
                device.Description = deviceEvent.Description;

                if (deviceEvent.EventType == "Connected")
                {
                    device.ConnectionCount++;
                    device.IsCurrentlyConnected = true;
                }
                else if (deviceEvent.EventType == "Disconnected")
                {
                    device.DisconnectionCount++;
                    device.IsCurrentlyConnected = false;
                }

                SaveData();
            }
        }
    }
}
