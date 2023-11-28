using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InSupport.Drift.Plugins
{
    class MqttClient
    {
        IMqttClient client = new MqttFactory().CreateMqttClient();
        bool reconnect;
        IMqttClientOptions options;
        public Action<string, string> OnMessageReceived { get; set; }

        public void ConnectAndSubscribe()
        {
            reconnect = true;

            var host = Dns.GetHostEntry("localhost");
            options = new MqttClientOptionsBuilder()
            .WithClientId("AxisCameraStatusClient")
            .WithTcpServer(host.HostName)
            .Build();

            client.UseDisconnectedHandler(OnDisconnect);
            client.UseApplicationMessageReceivedHandler(_OnMessageReceived);
            client.UseConnectedHandler(OnConnect);
            client.ConnectAsync(options, CancellationToken.None);
        }

        private void OnConnect(MqttClientConnectedEventArgs arsg)
        {
            // Subscribe to a topic
            client.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("simqtt/#").Build());
        }

        async private void OnDisconnect(MqttClientDisconnectedEventArgs arg)
        {
            if (!reconnect)
                return;

            //Wait, then try to reconnect
            await Task.Delay(TimeSpan.FromSeconds(5));
            await client.ConnectAsync(options, CancellationToken.None);
        }

        private void _OnMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            OnMessageReceived?.Invoke(args.ApplicationMessage.ConvertPayloadToString(), args.ApplicationMessage.Topic);
        }

        public void Disconnect()
        {
            reconnect = false;
            client.DisconnectAsync();
        }
    }
}
