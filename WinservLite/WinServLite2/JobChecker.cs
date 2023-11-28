using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinServLib.Objects;

namespace WinServLite2
{
    class JobChecker
    {
        JobBrowser jobBrowser;

        bool running = true;

        TimeSpan checkInterval;

        DateTime nextCheck;

        public event EventHandler<IEnumerable<Job>> OnJobsModified;
        public event EventHandler<IEnumerable<Job>> OnNewJobsFound;
        public event EventHandler<Exception> OnError;

        public JobChecker(JobBrowser jb)
        {
            jobBrowser = jb;

            checkInterval = new TimeSpan(0, 1, 0);

            nextCheck = DateTime.Now.Add(checkInterval);

            new Thread(CheckThread).Start();
        }

        public void Stop()
        {
            running = false;
        }

        private void CheckThread()
        {
            try
            {
                while (running)
                {
                    if (DateTime.Now >= nextCheck)
                    {
                        var jobs = WinServLib.WinServ.JobManager.GetActiveJobs();
                        jobs.AddRange(WinServLib.WinServ.JobManager.GetArchivedJobs());

                        var excepts = jobBrowser.JobList.Except(jobs, new JobComparer()).ToList();

                        var highestId = jobs.Where(j => j.JobStatus == Job.JobStatusType.Active).OrderByDescending(j => j.JobID).First().JobID;
                        var highestLocalId = jobBrowser.JobList.OrderByDescending(j => j.JobID).First().JobID;

                        if(highestId > highestLocalId)
                        {
                            OnNewJobsFound?.Invoke(this, jobs.Where(j => j.JobID > highestLocalId));
                        }

                        if (excepts.Count > 0)
                        {
                            OnJobsModified?.Invoke(this, excepts);
                        }

                        jobs.Clear();
                        excepts.Clear();
                        nextCheck = DateTime.Now.Add(checkInterval);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
        }
    }
}
