using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace REIDShared
{
    public class KafkaWorker
    {
        List<Consumer> consumers;
        KafkaConnection kafkaConnection;

        bool close = false;

        ConcurrentDictionary<string, ConcurrentDictionary<string, List<ConsumeResult<string, string>>>> newRawDict;

        ConcurrentQueue<ConcurrentDictionary<string, List<ConsumeResult<string, string>>>> messageQueue;

        ConcurrentQueue<string> removeKeysQueue;

        int topics = 0;
        int secondsToClean = 5;

        int channel;

        public KafkaWorker(string broker, int channel)
        {
            this.channel = channel;
            newRawDict = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<ConsumeResult<string, string>>>>();
            messageQueue = new ConcurrentQueue<ConcurrentDictionary<string, List<ConsumeResult<string, string>>>>();

            removeKeysQueue = new ConcurrentQueue<string>();

            consumers = new List<Consumer>();

            kafkaConnection = new KafkaConnection();
            kafkaConnection.Connect(broker);
        }

        public void AddConsumer(string topic)
        {
            consumers.Add(kafkaConnection.Subscribe(topic, $"Milestone_{channel}"));
            topics++;
        }

        public void Start()
        {
            foreach (var consumer in consumers)
            {
                new Thread(() => Consume(consumer)).Start();
            }

            new Thread(Cleaner).Start();
        }

        void Cleaner()
        {
            while(!close)
            {
                Thread.Sleep(secondsToClean * 1000);

                while(removeKeysQueue.Count > 0)
                {
                    string key = null;
                    if (removeKeysQueue.TryDequeue(out key))
                    {
                        ConcurrentDictionary<string, List<ConsumeResult<string, string>>> removedValue = null;
                        while (!newRawDict.TryRemove(key, out removedValue))
                        {
                            if (removedValue == default)
                                break;
                            Thread.Sleep(50);
                        }
                    }
                    else
                        Thread.Sleep(50);
                }

                try
                {
                    var keys = newRawDict.Keys;
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var oldkeys = keys.Where(k => now - long.Parse(k.Split('_')[0]) > secondsToClean * 1000).ToList();

                    while(oldkeys.Count > 0)
                    {
                        ConcurrentDictionary<string, List<ConsumeResult<string, string>>> removedValue = null;
                        while(!newRawDict.TryRemove(oldkeys[0], out removedValue)) 
                        {
                            if (removedValue == default)
                                break;
                            Thread.Sleep(50);
                        }
                        oldkeys.RemoveAt(0);
                    }
                }
                catch
                {
                    //Toolbox.Log.LogError("KafkaWorker:Cleaner", ex.Message);
                }
            }
        }

        public IDictionary<string, List<ConsumeResult<string, string>>> GetConsumption()
        {
            ConcurrentDictionary<string, List<ConsumeResult<string, string>>> q = null;
            DateTime startFetch = DateTime.Now;

            while(!messageQueue.TryDequeue(out q))
            {
                if ((DateTime.Now - startFetch).TotalSeconds >= 1)
                    break;
                else
                    Thread.Sleep(50);
            }
            //while (! && !close) { }

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

        void Consume(Consumer _consumer)
        {
            bool skipFirst = true;
            var list = new List<ConsumeResult<string, string>>();

            while (!_consumer.IsCancelled && !close)
            {
                var consumption = _consumer.Consume();
                var key = consumption.Message.Key;

                list.Add(consumption);

                if (key.Split('_').Length == 1) // last frame
                {// commit
                    if (skipFirst)
                        skipFirst = false;
                    else
                    {

                        //new ConcurrentDictionary<string, List<ConsumeResult<string, string>>>()
                        //{
                        //    { _consumer.Topic, new List<ConsumeResult<string, string>>(list) }
                        //}
                        newRawDict.AddOrUpdate(key, (k) =>
                        {
                            var dict =  new ConcurrentDictionary<string, List<ConsumeResult<string, string>>>();
                            dict.TryAdd(_consumer.Topic, new List<ConsumeResult<string, string>>(list));
                            return dict;
                        },
                        (k, l) => 
                        { 
                            var newList = new List<ConsumeResult<string, string>>(list);
                            l.AddOrUpdate(_consumer.Topic, newList, (ik, il) =>
                            {
                                il.AddRange(newList);
                                return il;
                            });
                            return l; 
                        });

                        if (newRawDict[key].Count == topics)
                        {
                            messageQueue.Enqueue(newRawDict[key]);
                            removeKeysQueue.Enqueue(key);
                        }
                    }
                    list.Clear();
                }
            }
        }
    }

}
