using System;
using MQTTnet;
using MQTTnet.Client;

namespace MQTT
{
    public class MQTTService
    {
        private IMqttClient _mqttClient;

        public MQTTService()
        {
            _mqttClient = new MqttFactory().CreateMqttClient();
        }

        public string MessageTopic { get; set; }
        public string TitreTopic { get; set; }

        public IMqttClient MqttClient
        {
            get { return _mqttClient; }
        }
    }
}