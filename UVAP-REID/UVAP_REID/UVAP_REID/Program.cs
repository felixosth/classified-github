using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UVAP_REID
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = new ConsumerConfig
            {
                GroupId = Environment.MachineName,
                BootstrapServers = "172.16.100.5:9092",
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };
            CancellationTokenSource cts = new CancellationTokenSource();

            var _consumer = new ConsumerBuilder<string, string>(config).Build();

            foreach(var topic in args)
            {
                Console.WriteLine("Subscribing to " + topic);
                //_consumer.Subscribe(topic);
                StartConsumption(config, topic, cts.Token);
            }

            //_consumer.Subscribe("fve.cam.99.reids.ReidRecord.json");


            //Console.CancelKeyPress += (s, e) =>
            //{
            //    cts.Cancel();
            //};

            while(!cts.Token.IsCancellationRequested)
            {

            }

            //Console.WriteLine("Starting consumption...");

            //while (true)
            //{
            //    var consumption = _consumer.Consume(cts.Token);

            //    Console.WriteLine("[{0}] [{1}] [{2}]: {3}", consumption.Message.Timestamp.UtcDateTime.ToString(), consumption.Topic, consumption.Message.Key, consumption.Message.Value);

            //    //var value = consumption.Message.Value;

            //    //if(value.Contains("REID_EVENT"))
            //    //{
            //    //    var reidEvent = JsonConvert.DeserializeObject<REIDEvent_root>(value);

            //    //    foreach(var match in reidEvent.reid_event.match_list)
            //    //    {

            //    //        Console.WriteLine("Recognized {0} from stream {1} with score {2:0.0}", names.ContainsKey(match.id.first_detection_key) ? names[match.id.first_detection_key] : match.id.first_detection_key, reidEvent.reid_event.input_stream_id, match.score);
            //    //    }
            //    //}
            //}
        }

        static void StartConsumption(ConsumerConfig cfg, string topic, CancellationToken ct)
        {
            var _consumer = new ConsumerBuilder<string, string>(cfg).Build();
            DateTime start = DateTime.MinValue;
            var lastCheck = DateTime.Now;
            _consumer.Subscribe(topic);
            int totalMessages = 0;
            int messages = 0;
            int mps = 0;

            new Thread(() =>
            {
                while(!ct.IsCancellationRequested)
                {
                    var consumption = _consumer.Consume(ct);
                    if (start == DateTime.MinValue)
                        start = DateTime.Now;
                    messages++;
                    totalMessages++;
                    //if (consumption.Message.Value.Contains("{\"type\":\"END_OF_INPUT_RECORD\"}") || consumption.Message.Value.Contains("{\"end_of_frame\":true}"))
                    //    continue;

                    if((DateTime.Now - lastCheck).TotalSeconds >= 1)
                    {
                        mps = messages;
                        messages = 0;
                        lastCheck = DateTime.Now;
                    }

                    double avg = totalMessages / (DateTime.Now - start).TotalSeconds;

                    Console.WriteLine("[{0:yyyy-MM-dd HH:mm:ss.fff}] [{4}mps] [{5:0.0}amps] [{1}] [{2}]: {3}", consumption.Message.Timestamp.UtcDateTime, consumption.Topic, consumption.Message.Key, consumption.Message.Value.Truncate(50), mps, avg);
                }
            }).Start();
        }



        public class REIDEvent_root
        {
            public string type { get; set; }
            public Reid_Event reid_event { get; set; }
        }

        public class Reid_Event
        {
            public Match_List[] match_list { get; set; }
            public string input_stream_id { get; set; }
        }

        public class Match_List
        {
            public Id id { get; set; }
            public float score { get; set; }
        }

        public class Id
        {
            public string first_detection_time { get; set; }
            public string first_detection_key { get; set; }
            public string first_detection_stream_id { get; set; }
        }



    }
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
