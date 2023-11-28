using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinServLib.Objects;

namespace WinServLib.Jobs
{
    public class JobManager
    {
        public JobManager()
        {
        }

        public List<Job> GetActiveJobs()
        {
            return GetJobs();
        }

        public List<Job> GetArchivedJobs()
        {
            return GetJobs(archived: true);
        }

        public Job GetJobWithId(int jobId)
        {
            var job = GetJob(jobId, false);
            if (job == null)
                job = GetJob(jobId, true);
            return job;
        }

        public List<Job> SearchJobs(string search, Job.JobStatusType jobStatusType)
        {
            var jobs =  GetJobs(jobStatusType == Job.JobStatusType.Archived);

            var searchedJobs = jobs.Where(j =>
                j.JobID.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                j.SiteName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                j.CompleteJobDescription.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                j.Technician.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                j.RefName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            jobs.Clear();

            return searchedJobs;
        }

        public Job GetJob(int jobbNr, bool archived)
        {
            Job job = null;
            List<JobDescription> jobDescriptions = new List<JobDescription>();

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT JOBBNR, TEXTRAD, RAD FROM dbo.JOBBFT" + (archived ? "H" : "") + " WHERE JOBBNR=@jobbnr;";

                try
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        sqlCmd.Parameters.AddWithValue("@jobbnr", jobbNr);
                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var desc = reader.GetString(1);
                                jobDescriptions.Add(new JobDescription(reader.GetDecimal(0), desc, (int)reader.GetDecimal(2)));
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                query = "SELECT JOBBNR, KUNDNR, INSTALL, MNR, REF, TEL, EMAIL, TEKN, JOBBTYP, INDAT, GATA, ORT, PNR, STATUS, PLANNERTASKID FROM dbo.JOBB" + (archived ? "HIST" : "") + " WHERE JOBBNR=@jobbnr;";

                try
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        sqlCmd.Parameters.AddWithValue("@jobbnr", jobbNr);
                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                job = new Job(reader.GetDecimal(0), "",
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetString(5),
                                    reader.GetString(6),
                                    reader.GetString(7),
                                    archived,
                                    reader.GetString(8),
                                    reader.GetDateTime(9),
                                    reader.GetString(13))
                                { Address = reader.GetString(10), City = reader.GetString(11), PostNumber = reader.GetString(12) };
                                job.PlannerTaskId = !reader.IsDBNull(14) ? reader.GetString(14) : null;

                            }

                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                sqlConn.Close();
            }

            PairDescriptions(jobDescriptions);
            PairDescriptionsWithJobs(new List<Job>() { job }, jobDescriptions);

            //job.JobDescriptions = jobDescriptions;
            return job;
        }

        internal List<Job> GetJobs(bool archived = false, int specific = -1)
        {
            List<JobDescription> jobDescriptions = new List<JobDescription>();
            List<Job> jobList = new List<Job>();

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT JOBBNR, TEXTRAD, RAD FROM dbo.JOBBFT" + (archived ? "H" : "") + (specific != -1 ? " WHERE JOBBNR=@jobbnr" : "");

                try
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        if (specific != -1)
                            sqlCmd.Parameters.AddWithValue("@jobbnr", specific);

                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var desc = reader.GetString(1);
                                jobDescriptions.Add(new JobDescription(reader.GetDecimal(0), desc, (int)reader.GetDecimal(2)));
                            }
                            reader.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                query = "SELECT JOBBNR, KUNDNR, INSTALL, MNR, REF, TEL, EMAIL, TEKN, JOBBTYP, INDAT, GATA, ORT, PNR, STATUS, PLANNERTASKID FROM dbo.JOBB" + (archived ? "HIST" : "") + (specific != -1 ? " WHERE JOBBNR=@jobbnr" : "");

                try
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        if (specific != -1)
                            sqlCmd.Parameters.AddWithValue("@jobbnr", specific);

                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var job = new Job(reader.GetDecimal(0), "",
                                    reader.GetString(1),
                                    reader.GetString(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    reader.GetString(5),
                                    reader.GetString(6),
                                    reader.GetString(7),
                                    archived,
                                    reader.GetString(8),
                                    reader.GetDateTime(9),
                                    reader.GetString(13))
                                { Address = reader.GetString(10), City = reader.GetString(11), PostNumber = reader.GetString(12) };
                                //PlannerTaskId = reader.GetString(14)
                                job.PlannerTaskId = !reader.IsDBNull(14) ? reader.GetString(14) : null;
                                jobList.Add(job);
                            }

                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                sqlConn.Close();
            }

            PairDescriptions(jobDescriptions);
            PairDescriptionsWithJobs(jobList, jobDescriptions);

            return jobList;
        }

        public List<TimeReport> GetTimeReportsReport(string technician, DateTime startDate, DateTime endDate)
        {
            var timeReports = new List<TimeReport>();
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = string.Format("SELECT TEKN, JOBBNR, STARTDAT, ARBTID, RESTID, KOMMENTAR, TRAKTAMENTE FROM dbo.JOBBTEKN WHERE TEKN=@tech AND STARTDAT>=@start AND STARTDAT<=@end;", technician);
                
                for (int i = 0; i < 2; i++)
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        sqlCmd.Parameters.AddWithValue("@tech", technician);
                        sqlCmd.Parameters.AddWithValue("@start", startDate.ToString());
                        sqlCmd.Parameters.AddWithValue("@end", endDate.Date.AddSeconds((24 * 60 * 60) - 1).ToString());

                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bool trakt = false;
                                int boolean = reader.GetOrdinal("TRAKTAMENTE");
                                if (!reader.IsDBNull(boolean))
                                    trakt = reader.GetBoolean(boolean);

                                timeReports.Add(new TimeReport()
                                {
                                    Technician = reader.GetString(0),
                                    JobID = (int)reader.GetDecimal(1),
                                    Date = reader.GetDateTime(2),
                                    WorkTime = (float)reader.GetDecimal(3),
                                    TravelTime = (float)reader.GetDecimal(4),
                                    Comment = reader.GetString(5),
                                    Traktamente = trakt,
                                    ArchivedJob = i == 0
                                });
                            }
                            reader.Close();
                        }
                    }

                    query = query.Replace("dbo.JOBBTEKN", "dbo.TEKNJOBB");
                }

                for (int i = 0; i < timeReports.Count; i++)
                {
                    using (var reader = new SqlCommand("SELECT INSTALL FROM dbo.JOBB WHERE JOBBNR='" + (double)timeReports[i].JobID + "'", sqlConn).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeReports[i].SiteName = reader.GetString(0);
                        }
                        reader.Close();
                    }
                    if (string.IsNullOrEmpty(timeReports[i].SiteName))
                    {
                        using (var reader = new SqlCommand("SELECT INSTALL FROM dbo.JOBBHIST WHERE JOBBNR='" + (double)timeReports[i].JobID + "'", sqlConn).ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                timeReports[i].SiteName = reader.GetString(0);
                            }
                            reader.Close();
                        }
                    }
                }
                sqlConn.Close();
            }
            return timeReports;
        }

        private void PairDescriptions(List<JobDescription> jobDescriptions)
        {
            for (int i = 0; i < jobDescriptions.Count; i++)
            {
                if (i + 1 != jobDescriptions.Count)
                {
                    if (jobDescriptions[i + 1].Row == 1)
                    {
                        jobDescriptions[i].IsPrimary = true;
                    }
                    else if (!jobDescriptions[i].IsProcessed)
                    {
                        int wIndex = 1;

                        try
                        {

                            while (jobDescriptions[i + wIndex].Row != 1)
                            {
                                jobDescriptions[i].CompleteJobText += jobDescriptions[i + wIndex].JobText;
                                jobDescriptions[i].Children.Add(jobDescriptions[i + wIndex]);
                                jobDescriptions[i + wIndex].IsProcessed = true;
                                if (i + wIndex == jobDescriptions.Count)
                                    break;

                                wIndex++;
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void PairDescriptionsWithJobs(List<Job> jobList, List<JobDescription> jobDescriptions)
        {
            for (int i = jobList.Count; i-- > 0;)
            {
                for (int x = jobDescriptions.Count; x-- > 0;)
                {
                    if (jobList[i].JobID == jobDescriptions[x].JobID && jobDescriptions[x].Row == 1)
                    {
                        jobList[i].CompleteJobDescription = jobDescriptions[x].CompleteJobText;
                        jobList[i].JobDescriptions.Add(jobDescriptions[x]);
                        //jobList[i].JobDescriptions.Add(jobDescriptions[x].Children);
                        foreach (JobDescription jobDesc in jobDescriptions[x].Children)
                        {
                            jobList[i].JobDescriptions.Add(jobDesc);
                        }
                        jobDescriptions.RemoveAt(x);
                        break;
                    }
                }
            }
        }

        public List<Site> GetSites()
        {
            var sites = new List<Site>();
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT MNR, INSTALL, KUNDNR, MTYP, MODELL, GATUADR, POSTNR, ORT FROM dbo.MASKINER";

                SqlDataReader reader = new SqlCommand(query, sqlConn).ExecuteReader();

                while (reader.Read())
                {
                    sites.Add(new Site()
                    {
                        SiteID = reader.GetString(0),
                        Name = reader.GetString(1),
                        CustomerID = reader.GetString(2),
                        ModelType = reader.GetString(3),
                        Model = reader.GetString(4),
                        Address = reader.GetString(5),
                        PostNR = reader.GetString(6),
                        City = reader.GetString(7)
                    });
                }
            }
            return sites;
        }

        public decimal GetTotalTimeFromDay(string technician, DateTime date)
        {
            string query = string.Format("SELECT ARBTID, RESTID FROM dbo.TEKNJOBB WHERE TEKN='{0}' AND STARTDAT>='{1}' AND STARTDAT<='{2}'",
                technician, date.Date.ToString(), date.Date.AddHours(24));

            decimal totalTime = 0;

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var arb = reader.GetDecimal(0);
                        var res = reader.GetDecimal(1);

                        totalTime += arb + res;
                    }
                }
            }
            return totalTime;
        }

        public IEnumerable<TimeReport> SearchTimeReports(string text)
        {
            List<TimeReport> timeReportResults = new List<TimeReport>();

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();
                using (var sqlCmd = new SqlCommand("SELECT TOP (500) TEKN, JOBBNR, KOMMENTAR, STARTDAT FROM dbo.JOBBTEKN WHERE KOMMENTAR LIKE @search ORDER BY STARTDAT DESC;", sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("@search", $"%{text}%");

                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeReportResults.Add(new TimeReport()
                            {
                                Technician = (string)reader["TEKN"],
                                JobID = (int)(decimal)reader["JOBBNR"],
                                Comment = (string)reader["KOMMENTAR"],
                                ArchivedJob = true,
                                Date = (DateTime)reader["STARTDAT"],
                            });
                        }
                    }
                }

                using (var sqlCmd = new SqlCommand("SELECT TEKN, JOBBNR, KOMMENTAR, STARTDAT FROM dbo.TEKNJOBB WHERE KOMMENTAR LIKE @search;", sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("@search", $"%{text}%");

                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            timeReportResults.Add(new TimeReport()
                            {
                                Technician = (string)reader["TEKN"],
                                JobID = (int)(decimal)reader["JOBBNR"],
                                Comment = (string)reader["KOMMENTAR"],
                                ArchivedJob = false,
                                Date = (DateTime)reader["STARTDAT"],
                                
                            });
                        }
                    }
                }
            }
            return timeReportResults;
        }
    }
}
