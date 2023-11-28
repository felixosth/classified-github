using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    [Serializable]
    public class JobDescription
    {
        public JobDescription(decimal id, string text, int row)
        {
            this.Row = row;
            this.JobID = (int)id;
            this.JobText = text;
            CompleteJobText = JobText;
            Children = new List<JobDescription>();
        }

        public bool IsPrimary { get; set; }
        public bool IsProcessed { get; set; }
        public List<JobDescription> Children { get; set; }

        public int Row { get; set; }
        public int JobID { get; set; }
        public string CompleteJobText { get; set; }
        public string JobText { get; set; }
    }
}
