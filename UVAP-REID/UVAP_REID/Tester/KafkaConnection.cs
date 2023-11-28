using Confluent.Kafka;
using REIDShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace REIDMipDriver
{
    // Custom Kafka connection class, used by the StreamSession classes
    internal class KafkaConnection
    {
        private List<Consumer> consumers = new List<Consumer>();


        public string Broker => broker;

        private string broker;

        public bool Connect(string broker, int timeoutSeconds = 10) // Try to connect to the Kafka bootstrap server, return true if successful
        {
            try
            {
                using (var client = new AdminClientBuilder(new AdminClientConfig() { BootstrapServers = broker }).Build())
                {
                    var meta = client.GetMetadata(timeout: TimeSpan.FromSeconds(timeoutSeconds));
                    if (meta.Brokers.Count == 0)
                        return false;
                }
            }
            catch (KafkaException) { return false; }
            this.broker = broker;

            return true;
        }

        public Consumer Subscribe(string topic, string groupIdString = "")
        {
            var config = new ConsumerConfig
            {
                GroupId = RandomString(16, Helper.StringToSeed(groupIdString + topic)),
                BootstrapServers = broker,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
            };
            Consumer consumer = new Consumer(config, topic);
            consumer.OnClose += Consumer_OnClose;
            lock (consumers)
            {
                consumers.Add(consumer);
            }
            return consumer;
        }

        public static string RandomString(int length, int seed)
        {
            var random = new Random(seed);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void Consumer_OnClose(object sender, EventArgs e)
        {
            lock (consumers)
            {
                consumers.Remove(sender as Consumer);
            }
        }

        public void Unsubscribe(string topic)
        {
            lock (consumers)
            {
                foreach (var consumer in consumers)
                {
                    if (topic == consumer.Topic)
                    {
                        consumer.Close();
                        consumers.Remove(consumer);
                        break;
                    }
                }
            }
        }
    }
    internal class Consumer
    {
        public string Topic { get; set; }
        IConsumer<string, string> consumer;
        CancellationTokenSource cts = new CancellationTokenSource();

        public event EventHandler OnClose;
        public bool IsCancelled => cts.IsCancellationRequested;

        public Consumer(ConsumerConfig cfg, string topic)
        {
            this.Topic = topic;
            consumer = new ConsumerBuilder<string, string>(cfg).Build();
            consumer.Subscribe(topic);
        }

        public ConsumeResult<string, string> Consume()
        {
            return consumer.Consume(cts.Token);
        }

        public void Close()
        {
            cts.Cancel();
            consumer.Close();
            consumer = null;
            OnClose?.Invoke(this, new EventArgs());
        }
    }
}
