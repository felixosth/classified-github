using Confluent.Kafka;
using REIDMipDriver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester
{
    public class KafkaWorkerTest
    {
        List<Consumer> consumers;
        KafkaConnection kafkaConnection;

        bool close = false;

        ConcurrentDictionary<string, Dictionary<string, List<ConsumeResult<string, string>>>> newRawDict;
        List<Dictionary<string, List<ConsumeResult<string, string>>>> committedQueue;

        ConcurrentQueue<Dictionary<string, List<ConsumeResult<string, string>>>> testQueue;

        object lockObject = new object();

        int topics = 0;
        int secondsToClean = 5;

        int channel;

        public KafkaWorkerTest(string broker, int channel)
        {
            this.channel = channel;
            committedQueue = new List<Dictionary<string, List<ConsumeResult<string, string>>>>();
            newRawDict = new ConcurrentDictionary<string, Dictionary<string, List<ConsumeResult<string, string>>>>();
            testQueue = new ConcurrentQueue<Dictionary<string, List<ConsumeResult<string, string>>>>();

            consumers = new List<Consumer>();

            kafkaConnection = new KafkaConnection();
            kafkaConnection.Connect(broker);
        }

        public void AddConsumer(string topic)
        {
            consumers.Add(kafkaConnection.Subscribe(topic, $"Milestone_{channel}_"));
            topics++;
        }

        public void Start()
        {
            foreach (var consumer in consumers)
            {
                new Thread(() => NewConsume(consumer)).Start();
                //Thread.Sleep(25);
            }

            //new Thread(NewCleaner).Start();
        }

        void NewCleaner()
        {
            while (!close)
            {
                Thread.Sleep(secondsToClean * 1000);
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                while (!close)
                {
                    var getEntry = committedQueue.FirstOrDefault();

                    if (getEntry != null)
                    {
                        try
                        {
                            if (now - long.Parse(getEntry.First().Value.First().Message.Key) >= secondsToClean * 1000)
                                committedQueue.Remove(getEntry);
                            else break;
                        }
                        catch
                        {
                            committedQueue.Remove(getEntry);
                        }
                    }
                    else break;
                }
                GC.Collect();
            }
        }

        public Dictionary<string, List<ConsumeResult<string, string>>> GetConsumption()
        {
            Dictionary<string, List<ConsumeResult<string, string>>> q = null;

            while (!testQueue.TryDequeue(out q)) { }

            return q;
        }

        public void Close()
        {
            foreach (var consumer in consumers)
            {
                consumer.Close();
            }

            close = true;
        }

        void NewConsume(Consumer _consumer)
        {
            bool skipFirst = true;
            var list = new List<ConsumeResult<string, string>>();
            Stopwatch sw = new Stopwatch();

            while (!_consumer.IsCancelled && !close)
            {
                bool didCommit = false;
                sw.Restart();
                var consumption = _consumer.Consume();
                var key = consumption.Message.Key;

                list.Add(consumption);

                if (key.Split('_').Length == 1) // last frame
                {// commit
                    didCommit = true;
                    if (skipFirst)
                        skipFirst = false;
                    else
                    {
                        //if (!newRawDict.ContainsKey(key))
                        //{
                        //    while(!newRawDict.TryAdd(key, new Dictionary<string, List<ConsumeResult<string, string>>>())) { }
                        //}

                        //var dict = newRawDict.GetOrAdd(key, new Dictionary<string, List<ConsumeResult<string, string>>>());
                        //dict.Add(_consumer.Topic, new List<ConsumeResult<string, string>>(list));

                        newRawDict.AddOrUpdate(key, new Dictionary<string, List<ConsumeResult<string, string>>>() {{ _consumer.Topic, new List<ConsumeResult<string, string>>(list) }},
                            (k, l) => { l.Add(_consumer.Topic, new List<ConsumeResult<string, string>>(list)); return l; });

                        if (newRawDict[key].Count == topics)
                        {
                            didCommit = true;
                            testQueue.Enqueue(newRawDict[key]);
                            //newRawDict.
                            while(!newRawDict.TryRemove(key, out _)) { }
                        }
                    }
                    list.Clear();
                }
                sw.Stop();
                Console.WriteLine("Elapsed frame: {0}ms, did commit = {1}", sw.ElapsedMilliseconds, didCommit);
            }
        }
    }
}
