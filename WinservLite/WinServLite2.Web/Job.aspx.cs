using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WinServLib.Objects;

namespace WinServLite2.Web
{
    public partial class Job : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Setup();
        }

        private void Setup()
        {
            int jobId;
            if (Request.QueryString.AllKeys.Contains("id") && int.TryParse(Request.QueryString["id"], out jobId))
            {
                var job = WinServLib.WinServ.JobManager.GetJobWithId(jobId);


                NewReportJobID.Value = job.JobID.ToString();

                JobHeader.Text = $"[{job.SiteID}] {job.SiteName}";
                JobDescriptionLabel.Text = job.CompleteJobDescription;

                JobContactNameLabel.Text = job.RefName;

                JobContactTelLink.Text = job.RefPhoneNumber;
                JobContactTelLink.NavigateUrl = "tel:" + job.RefPhoneNumber;

                JobContactEmailLink.Text = job.RefEmailAddress;
                JobContactEmailLink.NavigateUrl = "mailto:" + job.RefEmailAddress;

                JobLocationMapsLink.Text = string.Format("{0}, {1} {2}", job.Address, job.PostNumber, job.City);
                JobLocationMapsLink.NavigateUrl = string.Format("https://www.google.se/maps/search/{0}+{1}+{2}", job.SiteName, job.Address, job.City);
                
                var reports = job.GetTimeReports().OrderByDescending(jtr => jtr.Date);
                JobReportsTable.DataSource = reports;
                JobReportsTable.DataBind();

                NewReportTech.DataSource = WinServLib.WinServ.GetTechnicians().OrderBy(tech => tech.Name);
                NewReportTech.DataValueField = "UserName";
                NewReportTech.DataTextField = "Name";
                if (Request.Cookies.AllKeys.Contains("myTechnician"))
                    NewReportTech.SelectedValue = Request.Cookies["myTechnician"].Value;

                NewReportTech.DataBind();

                NewReportJobTimeType.DataSource = WinServLib.WinServ.GetJobTimeTypes().OrderBy(jtt => jtt.Code);
                NewReportJobTimeType.DataValueField = "Code";
                NewReportJobTimeType.DataTextField = "Name";
                NewReportJobTimeType.DataBind();

                NewReportDate.Text = DateTime.Now.ToShortDateString();
            }
            else
            {
                JobHeader.Text = "No job found";
            }
            Page.Title = JobHeader.Text;
        }

        protected void DeleteReport_Click(object sender, EventArgs e)
        {
            var commandArgSplit = (sender as Button).CommandArgument.Split(';');

            int jobId, reportId;
            if(int.TryParse(commandArgSplit[0], out jobId) && int.TryParse(commandArgSplit[1], out reportId))
            {
                var foundJob = WinServLib.WinServ.JobManager.GetJobWithId(jobId);
                if(foundJob != null)
                {
                    var report = foundJob.GetTimeReports().FirstOrDefault(tr => tr.UniqueID == reportId);
                    if(report != null)
                    {
                        report.Delete();
                    }
                }
                Response.Redirect(Request.RawUrl.ToString());
            }
        }

        protected void SubmitReport_Click(object sender, EventArgs e)
        {
            int jobId, reportId;
            double worktime, traveltime;

            var editReportId = EditReportID.Value;
            var reportTypeMode = ReportTypeMode.Value;

            var jobIdStr = NewReportJobID.Value;
            var tech = NewReportTech.SelectedValue;
            var comment = NewReportComment.Text;
            var jobTimeType = NewReportJobTimeType.SelectedValue;
            var date = NewReportDate.Text;
            var parsedDate = DateTime.Parse(date);
            var worktimeStr = NewReportWorktime.Text.Replace('.', ',');
            var traveltimeStr = NewReportTraveltime.Text.Replace('.', ',');

            if (int.TryParse(jobIdStr, out jobId) && 
                double.TryParse(worktimeStr, out worktime) && 
                double.TryParse(traveltimeStr, out traveltime) && 
                !string.IsNullOrWhiteSpace(comment) &&
                jobTimeType != "0")
            {

                if(editReportId == "" && reportTypeMode == "new") // New report
                {
                    var job = WinServLib.WinServ.JobManager.GetJobWithId(jobId);
                    if (job != null)
                    {
                        var cookie = new HttpCookie("myTechnician", tech);
                        cookie.Expires = DateTime.MaxValue;
                        Response.Cookies.Add(cookie);

                        int reports = (int)Math.Ceiling((decimal)comment.Length / TimeReport.CommentCharCap);

                        for (int i = 0; i < reports; i++)
                        {
                            string curComment = "";
                            if (comment.Length > TimeReport.CommentCharCap)
                            {
                                curComment = comment.Remove(TimeReport.CommentCharCap, comment.Length - TimeReport.CommentCharCap) + "-";
                                comment = comment.Remove(0, TimeReport.CommentCharCap);
                            }
                            else
                                curComment = comment;

                            job.AddTimeReport(new TimeReport()
                            {
                                Technician = tech,
                                JobID = jobId,
                                Date = parsedDate.AddSeconds(reports-i),
                                DelayCode = jobTimeType,
                                JobType = job.JobType,
                                WorkTime = i == 0 ? (float)worktime : 0,
                                TravelTime = i == 0 ? (float)traveltime : 0,
                                Comment = curComment,
                                Traktamente = false
                            });
                        }
                    }
                }
                else if(int.TryParse(editReportId, out reportId) && reportTypeMode == "edit")
                {
                    var job = WinServLib.WinServ.JobManager.GetJobWithId(jobId);
                    if (job != null)
                    {
                        var report = job.GetTimeReports().FirstOrDefault(tr => tr.UniqueID == reportId);
                        if(report != null)
                        {
                            report.Comment = comment;
                            report.Date = DateTime.Parse(date);
                            report.DelayCode = jobTimeType;
                            report.Technician = tech;
                            report.WorkTime = (float)worktime;
                            report.TravelTime = (float)traveltime;

                            report.Save();
                        }
                    }
                }
            }

            Response.Redirect(Request.RawUrl.ToString());
        }
    }
}