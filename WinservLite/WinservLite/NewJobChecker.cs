using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WinservLite
{
    public class NewJobChecker
    {
        MainWindow mw;
        bool stop = false;

        int lastJoblistCount;

        TimeSpan checkInterval;

        public NewJobChecker(MainWindow mw, TimeSpan interval)
        {
            checkInterval = interval;
            this.mw = mw;
        }

        public void Start()
        {
            lastJoblistCount = mw.GetJobList().Count;
            new Thread(CheckThread).Start();
        }

        public void SetJoblistCount(int count)
        {
            lastJoblistCount = count;
        }

        private void CheckThread()
        {
            while (!stop)
            {
                var nextCheck = DateTime.Now.Add(checkInterval);

                while(DateTime.Now < nextCheck)
                {
                    Thread.Sleep(1000);

                    if (stop)
                        break;
                }
                if (stop)
                    break;


                mw.RefreshJobList(updateGui: false);
                int newListCount = mw.GetJobList().Count;
                if(lastJoblistCount != newListCount)
                {
                    mw.NotifyNewJobs();
                    lastJoblistCount = newListCount;
                }
            }
        }
        
        public void Stop()
        {
            stop = true;
        }
    }
}
