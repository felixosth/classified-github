using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] colors = new string[]
            {
                "red",
                "blue",
                "blå",
                "green",
                "lightblue",
                "light red",
                "FBFF00",
                "#2B8DAA"
            };


            foreach(var str in colors)
            {
                Console.WriteLine("Converting \"{0}\" to color object..", str);
                Console.Write("Result: ");
                try
                {
                    var col = (Color)ColorConverter.ConvertFromString(str);
                    Console.Write("argb({0}, {1}, {2}, {3})", col.A, col.R, col.G, col.B);
                }
                catch
                {
                    Console.Write("Error");
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.ReadKey();

            //KafkaWorkerTest worker = new KafkaWorkerTest("172.16.100.16:9092", 1);
            //worker.AddConsumer("fve.cam.1.dets.ObjectDetectionRecord.json");
            //worker.AddConsumer("fve.cam.99.reids.ReidRecord.json");
            //worker.Start();
            //long messages = 0;

            //long startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            //while (true)
            //{
            //    var c = worker.GetConsumption();
            //    if(c != null)
            //    {
            //        messages++;
            //        var timeElapsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTime;
            //        Console.Title = "Processing at " + (messages / (float)timeElapsed) + " per second";
            //    }
            //}
        }
    }
}
