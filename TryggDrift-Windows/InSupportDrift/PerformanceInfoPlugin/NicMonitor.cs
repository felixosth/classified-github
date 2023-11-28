using System.Diagnostics;
using System.Threading;

namespace InSupport.Drift.Plugins.PerformanceInfo
{
    public class NicMonitor
    {
        private readonly PerformanceCounter netSentCounter, netRecCounter;

        public string Name { get; private set; }
        private long totalBytesSent = 0;
        long totalBytesRecieved = 0;
        int timesCollected = 0;

        private object collectThreadLock = new object();
        private NicMonitor(string perfCounterInstanceName)
        {
            Name = perfCounterInstanceName;


            // Initialize performance counters and thresd for the data collector
            netRecCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", perfCounterInstanceName);
            netSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", perfCounterInstanceName);
            new Thread(CollectDataThread) { IsBackground = true }.Start();
        }


        /// <summary>
        /// Get Bytes Received/sec 
        /// </summary>
        /// <returns></returns>
        private float GetBytesRecieved()
        {
            return netRecCounter.NextValue();
        }

        private float GetBytesSent()
        {
            return netSentCounter.NextValue();
        }

        public static NicMonitor[] GetNics()
        {
            // Get all available instances from the performance counters
            var instanceNames = new PerformanceCounterCategory("Network Interface").GetInstanceNames();

            var nics = new NicMonitor[instanceNames.Length];

            for (int i = 0; i < instanceNames.Length; i++)
            {
                nics[i] = new NicMonitor(instanceNames[i]);
            }

            return nics;
        }


        private void CollectDataThread()
        {
            //Collects total bytes sent and recieved, and no of times we have collected
            while (true)
            {
                lock (collectThreadLock)
                {
                    totalBytesSent += (long)GetBytesSent();
                    totalBytesRecieved += (long)GetBytesRecieved();
                    timesCollected++;
                }
                Thread.Sleep(1000);
            }
        }

        public CollectedResults Traffic => GetCollectedValues();
        //Constructor for GetCollectedValues
        public CollectedResults GetCollectedValues()
        {
            // Returns the average of our sent and recieved traffic data and converts to kilobits
            if (timesCollected == 0)
            {

                return new CollectedResults(0, 0);
            }
            var recievedAverage = (totalBytesRecieved / timesCollected) * 0.008f;
            var sentAverage = (totalBytesSent / timesCollected) * 0.008f;

            lock (collectThreadLock)
            {
                totalBytesSent = 0;
                totalBytesRecieved = 0;
                timesCollected = 0;
            }

            return new CollectedResults(recievedAverage, sentAverage);
        }
    }
    public class CollectedResults
    {
        public float Recieved { get; set; }
        public float Sent { get; set; }

        public CollectedResults(float recieved, float sent)
        {
            Recieved = recieved;
            Sent = sent;
        }
    }
}
