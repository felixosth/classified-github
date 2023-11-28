using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;

namespace InSupport.Drift.Plugins
{
    internal class MqttBroker
    {
        private readonly IMqttServer mqttServer = new MqttFactory().CreateMqttServer();

        internal event EventHandler<MqttApplicationMessageInterceptorContext> OnMessage;
        internal event EventHandler<MqttConnectionValidatorContext> OnConnect;
        internal event EventHandler<MqttSubscriptionInterceptorContext> OnSubscribe;

        internal void StartServer()
        {
            if (!mqttServer.IsStarted)
                RunMQTTServer();
        }
        internal async Task StartServerAsync()
        {
            if (!mqttServer.IsStarted)
                await RunMQTTServerAsync();
        }

        private IMqttServerOptions GetMQTTOptions()
        {
            return new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .WithConnectionValidator(ConnectionValidator)
                .WithSubscriptionInterceptor(SubscriptionInterceptor)
                .WithApplicationMessageInterceptor(ApplicationMessageInterceptor)
                .Build();
        }


        private void RunMQTTServer()
        {
            mqttServer.StartAsync(GetMQTTOptions()).GetAwaiter().GetResult();
        }

        private async Task RunMQTTServerAsync()
        {
            await mqttServer.StartAsync(GetMQTTOptions());
        }

        private void ConnectionValidator(MqttConnectionValidatorContext ctx)
        {
            ctx.ReasonCode = MqttConnectReasonCode.Success;
            OnConnect?.Invoke(this, ctx);
        }

        private void SubscriptionInterceptor(MqttSubscriptionInterceptorContext ctx)
        {
            ctx.AcceptSubscription = true;
            OnSubscribe?.Invoke(this, ctx);
        }

        private void ApplicationMessageInterceptor(MqttApplicationMessageInterceptorContext ctx)
        {
            ctx.AcceptPublish = true;
            OnMessage?.Invoke(this, ctx);
        }

        internal void StopServer()
        {
            if (mqttServer.IsStarted)
                mqttServer.StopAsync();
        }

        internal async Task StopServerAsync()
        {
            if (mqttServer.IsStarted)
                await mqttServer.StopAsync();
        }
    }
}
