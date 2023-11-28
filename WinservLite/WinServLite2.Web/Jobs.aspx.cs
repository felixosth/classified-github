using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WinServLib;

namespace WinServLite2.Web
{
    public partial class Jobs : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            SetSource();
        }

        private void SetSource()
        {
            var jobs = WinServ.JobManager.GetActiveJobs().OrderBy(job => job.SiteName).ThenByDescending(job => job.JobID);
            jobsTable.DataSource = jobs;
            jobsTable.DataBind();
        }
    }
}