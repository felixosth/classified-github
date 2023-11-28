using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace InSupport.Drift.Plugins
{
    public class AcapStatusMonitor
    {
        public Dictionary<string, AcapData> Cameras { get; set; }
        MqttBroker mqttBroker = new MqttBroker();

        public void Init()
        {
            mqttBroker.OnMessage += MqttBroker_OnMessage;
            mqttBroker.StartServer();

#if DEBUG
            //Init data for testing
            var camera = new AcapData();
            camera.Performance = new Performance()
            {
                Cpu = 0.4f,
                Network = 2000f,
                Timestamp = 123
            };
            Cameras = new Dictionary<string, AcapData>();
            Cameras["B8A44F051765"] = camera;
#endif
        }

        private void MqttBroker_OnMessage(object sender, MQTTnet.Server.MqttApplicationMessageInterceptorContext e)
        {
            string message = e.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            string topic = e.ApplicationMessage?.Topic;

            if (message != null && topic != null)
                OnMessageReceived(message, topic);
        }

        private void OnMessageReceived(string message, string topic)
        {
            string baseTopic = "simqtt/";
            if (!topic.StartsWith(baseTopic))
                return;
            topic = topic.Substring(baseTopic.Length);
            dynamic messageData;
            try
            {
                messageData = JsonConvert.DeserializeObject<dynamic>(message);
            }
            catch (JsonReaderException)
            {
                return;
            }

            var serial = (string)messageData.serial ?? (string)messageData.device; // Seems like SIMQTT 1.2 use device instead of serial 

            if (serial == null)
                throw new System.NullReferenceException("Serial is null");

            if (Cameras == null)
                Cameras = new Dictionary<string, AcapData>();

            if (topic.StartsWith("connect"))
            {
                if (!Cameras.ContainsKey(serial))
                    Cameras[serial] = new AcapData();
            }
            else if (topic.StartsWith("status"))
                ReadPerformance(messageData, Cameras[serial]);
        }

        private void ReadPerformance(dynamic messageData, AcapData acapData)
        {
            if (acapData.Performance == null)
                acapData.Performance = new Performance();
            acapData.Performance.Timestamp = (ulong)messageData.timestamp;
            acapData.Performance.Cpu = (float)messageData.cpu;
            acapData.Performance.Network = (float)messageData.network;
        }

        public void Close()
        {
            mqttBroker.StopServer();
        }
    }
}
