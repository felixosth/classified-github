using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryggSTORE.Http
{
    internal static class Logger
    {
        internal static string LogPath { get; set; } = "application.log";
        private static ConcurrentQueue<LogObject> LogObjects { get; set; } = new ConcurrentQueue<LogObject>();
        private static bool IsFlushing = false;

        internal static void Log(string message)
        {
            StackTrace stackTrace = new StackTrace();
            var calling = stackTrace.GetFrame(2).GetMethod().Name;
            LogObjects.Enqueue(new LogObject()
            {
                Time = DateTime.Now,
                Calling = calling,
                Message = message
            });

            FlushLog();

            //using (var fileStream = new FileStream(LogPath, FileMode.Append, FileAccess.Write))
            //using(var writer = new StreamWriter(fileStream))
            //{
            //    var tab = Convert.ToChar(9);
            //    writer.Write(DateTime.Now.ToString() + tab);


            //    writer.Write(calling + tab);

            //    writer.WriteLine(message);
            //}
        }

        private static void FlushLog()
        {
            if (IsFlushing)
                return;

            IsFlushing = true;
            var separator = Convert.ToChar(9);
            using (var fileStream = new FileStream(LogPath, FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))
                while (LogObjects.TryDequeue(out var deqLogObj))
                {
                    writer.Write(deqLogObj.Time.ToString() + separator);
                    writer.Write(deqLogObj.Calling + separator);
                    writer.WriteLine(deqLogObj.Message);
                }
            IsFlushing = false;
        }
    }
    internal class LogObject
    {
        internal DateTime Time { get; set; }
        internal string Calling { get; set; }
        internal string Message { get; set; }
    }
}
