using System;
namespace MQTT
{
	public class AppData
	{
        private static MQTTService _mqttService;

        public static MQTTService MQTTService
        {
            get
            {
                if (_mqttService == null)
                {
                    _mqttService = new MQTTService();
                }
                return _mqttService;
            }
        }
    }
}

