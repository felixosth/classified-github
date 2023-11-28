using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class JobComparer : IEqualityComparer<Job>
    {
        public JobComparer()
        {

        }

        public bool Equals(Job x, Job y)
        {
            return 
                x.JobID == y.JobID && 
                x.CompleteJobDescription == y.CompleteJobDescription && 
                x.JobStatus == y.JobStatus && x.Status == y.Status;
        }

        public int GetHashCode(Job obj)
        {
            return obj == null ? 0 : obj.JobID.GetHashCode();
        }
    }
}
