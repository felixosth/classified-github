using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{

    [Serializable]
    public class Job
    {
        public string Status { get; set; }

        public Status StatusObj => WinServ.JobStatus.FirstOrDefault(s => s.Nr == Status);

        public string Address { get; set; }
        public string City { get; set; }
        public string PostNumber { get; set; }

        public string Technician { get; set; }

        public JobStatusType JobStatus { get; set; }
        public string JobStatusString
        {
            get
            {
                return JobStatus == JobStatusType.Active ? "Aktiv" : "Arkiverad";
            }
        }

        public int JobID { get; set; }

        public List<JobDescription> JobDescriptions { get; set; }

        public string FormatJobDescriptions()
        {
            string description = "";
            foreach (JobDescription jobDesc in JobDescriptions)
            {
                if (jobDesc.JobText == "")
                    description += "\r\n";
                else
                    description += jobDesc.JobText;
            }
            return description;
        }

        public string CompleteJobDescription { get; set; }

        public string CustomerID { get; set; }

        public string SiteName { get; set; }
        public string SiteID { get; set; }

        public string RefName { get; set; }
        public string RefPhoneNumber { get; set; }
        public string RefEmailAddress { get; set; }

        public string PlannerTaskId { get; set; }
        public string JobType { get; set; }

        public DateTime DateAdded { get; set; }

        public enum JobStatusType { Archived, Active }


        List<Article> _articles = null;
        public List<Article> Articles
        {
            get
            {
                if (_articles == null)
                    _articles = GetArticles();
                return _articles;
            }
            set
            {
                _articles = value;
            }
        }

        public Job(decimal id, string desc, string cId, string siteName, string siteid, string refname, 
            string reftel, string refemail, string tekn, bool archived, string jobtype, DateTime date, string status)
        {
            JobDescriptions = new List<JobDescription>();
            Status = status;
            JobID = (int)id;
            CompleteJobDescription = desc;
            CustomerID = cId;
            SiteName = siteName;
            SiteID = siteid;
            RefName = refname;
            RefPhoneNumber = reftel;
            RefEmailAddress = refemail;
            Technician = tekn;
            //IsArchived = archived;
            if (archived)
                JobStatus = JobStatusType.Archived;
            else
                JobStatus = JobStatusType.Active;
            JobType = jobtype;
            DateAdded = date;
        }

        public Job()
        {

        }

        public Job RefreshFromDB() => WinServ.JobManager.GetJobWithId(JobID);
        //public Job RefreshFromDB() => WinServ.JobManager.GetJobWithId(JobID, JobStatus == JobStatusType.Archived);

        public bool SetPlannerTaskId(string plannerTaskId)
        {
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();
                string query = "UPDATE dbo.JOBB SET PLANNERTASKID=@taskId WHERE JOBBNR=@jobbnr;";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    if (plannerTaskId == null)
                        sqlCommand.Parameters.AddWithValue("@taskId", DBNull.Value);
                    else
                        sqlCommand.Parameters.AddWithValue("@taskId", plannerTaskId);

                    sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);
                    if(sqlCommand.ExecuteNonQuery() > 0)
                    {
                        this.PlannerTaskId = plannerTaskId;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Save(string[] newJobDescriptions = null, bool saveArticles = false)
        {
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();
                string query = "UPDATE dbo.JOBB SET STATUS=@status, REF=@ref, TEL=@tel, EMAIL=@email WHERE JOBBNR=@jobbnr";
                using (var sqlCommand = new SqlCommand(query, sqlConn))
                {
                    sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);
                    sqlCommand.Parameters.AddWithValue("@status", Status);
                    sqlCommand.Parameters.AddWithValue("@ref", RefName);
                    sqlCommand.Parameters.AddWithValue("@tel", RefPhoneNumber);
                    sqlCommand.Parameters.AddWithValue("@email", RefEmailAddress);

                    sqlCommand.ExecuteNonQuery();
                }

                if(newJobDescriptions != null)
                {
                    using (var sqlCommand = new SqlCommand("DELETE FROM dbo.JOBBFT WHERE JOBBNR=@jobbnr", sqlConn))
                    {
                        sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);

                        sqlCommand.ExecuteNonQuery();
                    }

                    for (int i = 0; i < newJobDescriptions.Length; i++)
                    {
                        query = "INSERT into dbo.JOBBFT (JOBBNR, TEXTRAD, RAD) VALUES (@jobbnr, @text, @rad)";
                        using (var sqlCommand = new SqlCommand(query, sqlConn))
                        {
                            sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);
                            sqlCommand.Parameters.AddWithValue("@rad", i + 1);
                            sqlCommand.Parameters.AddWithValue("@text", newJobDescriptions[i]);

                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    RefreshDescription(sqlConn);
                }

                if(saveArticles)
                    SaveArticles(sqlConn);

                sqlConn.Close();
            }
        }

        public void RefreshDescription(SqlConnection sqlConn)
        {
            JobDescriptions.Clear();
            string query = "SELECT JOBBNR, TEXTRAD, RAD FROM dbo.JOBBFT" + (JobStatus == JobStatusType.Archived ? "H" : "") + " WHERE JOBBNR=@jobbnr;";

            using (var sqlCmd = new SqlCommand(query, sqlConn))
            {
                sqlCmd.Parameters.AddWithValue("@jobbnr", JobID);
                using (var reader = sqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var desc = reader.GetString(1);
                        JobDescriptions.Add(new JobDescription(reader.GetDecimal(0), desc, (int)reader.GetDecimal(2)));
                    }
                    reader.Close();
                }
            }
        }

        public int GetTimeReportsCount()
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");
            string table = "DBO." + (JobStatus == JobStatusType.Archived ? "JOBBTEKN" : "TEKNJOBB");
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                using (var sqlCmd = new SqlCommand($"SELECT count(RECNUM) count FROM {table} WHERE JOBBNR=@jobbnr;", sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("@jobbnr", JobID);
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        reader.Read();
                        return (int)reader["count"];
                    }
                }
            }
        }

        public List<TimeReport> GetTimeReports()
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");


            List<TimeReport> timeReports = new List<TimeReport>();
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string table = "DBO." + (JobStatus == JobStatusType.Archived ? "JOBBTEKN" : "TEKNJOBB");

                string query = "SELECT JOBBNR, TEKN,KOMMENTAR, STARTDAT, ARBTID, RESTID, RECNUM, JOBBTYP, TRAKTAMENTE, DELAYCODE FROM " + table + " WHERE JOBBNR=@jobbnr;";

                try
                {
                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    {
                        sqlCmd.Parameters.AddWithValue("@jobbnr", JobID);
                        using (var reader = sqlCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                timeReports.Add(new TimeReport()
                                {
                                    JobID = (int)reader.GetDecimal(0),
                                    Technician = reader.GetString(1),
                                    Comment = reader.GetString(2),
                                    Date = reader.GetDateTime(3),
                                    WorkTime = (float)reader.GetDecimal(4),
                                    TravelTime = (float)reader.GetDecimal(5),
                                    UniqueID = (int)reader.GetInt64(6),
                                    JobType = reader.GetString(7),
                                    Traktamente = reader.IsDBNull(8) ? false : reader.GetBoolean(8),
                                    DelayCode = reader.GetString(9)

                                });
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
            var jobTimeTypes = WinServ.GetJobTimeTypes();

            foreach(var report in timeReports)
            {
                var thisReportsJobTimeType = jobTimeTypes.FirstOrDefault(jtt => jtt.Code == report.DelayCode);
                if (thisReportsJobTimeType != null)
                    report.JobTimeTypeName = thisReportsJobTimeType.Name;
            }

            return timeReports;
        }

        private List<Article> GetArticles()
        {
            var articles = new List<Article>();

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT RECNUM, TEXT, UTPRIS, ANTAL FROM dbo.DELAR WHERE JOBBNR=@jobbnr";
                using (var sqlCmd = new SqlCommand(query, sqlConn))
                {
                    sqlCmd.Parameters.AddWithValue("@jobbnr", this.JobID);
                    var reader = sqlCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        articles.Add(new Article()
                        {
                            RecNum = (int)reader.GetInt64(0),
                            ArticleText = reader.GetString(1),
                            ArticlePrice = reader.GetDecimal(2),
                            Quantity = reader.GetDecimal(3)
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
            }
            return articles;
        }

        public int AddTimeReport(TimeReport timeReport)
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            int result = -1;
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "INSERT into dbo.TEKNJOBB (TEKN, JOBBNR, JOBBTYP, PLANDAT, DELAYCODE, STARTDAT, SLUTDAT, ARBTID, RESTID, KOMMENTAR, TRAKTAMENTE) VALUES " +
                    "(@tekn, @jobbnr, @jobbtyp, @plandat, @delaycode, @startdat, @slutdat, @arbtid, @restid, @kommentar, @trakt)";
                using (var cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    cmd.Parameters.Add("@tekn", SqlDbType.NVarChar).Value = timeReport.Technician;
                    cmd.Parameters.Add("@jobbnr", SqlDbType.Decimal).Value = (double)JobID;
                    cmd.Parameters.Add("@plandat", SqlDbType.DateTime).Value = timeReport.Date.ToString();
                    cmd.Parameters.Add("@delaycode", SqlDbType.NVarChar).Value = timeReport.DelayCode;
                    cmd.Parameters.Add("@startdat", SqlDbType.DateTime).Value = timeReport.Date.ToString();
                    cmd.Parameters.Add("@slutdat", SqlDbType.DateTime).Value = timeReport.Date.ToString();
                    cmd.Parameters.Add("@jobbtyp", SqlDbType.NVarChar).Value = timeReport.JobType;
                    cmd.Parameters.Add("@arbtid", SqlDbType.Decimal).Value = timeReport.WorkTime;
                    cmd.Parameters.Add("@restid", SqlDbType.Decimal).Value = timeReport.TravelTime;
                    cmd.Parameters.Add("@kommentar", SqlDbType.NVarChar).Value = timeReport.Comment;
                    cmd.Parameters.Add("@trakt", SqlDbType.TinyInt).Value = timeReport.Traktamente;

                    result = cmd.ExecuteNonQuery();
                }

                sqlConn.Close();
            }
            return result;
        }

  


        public void SaveArticles(SqlConnection sqlConn)
        {
            new SqlCommand("DELETE FROM dbo.DELAR WHERE JOBBNR=" + (double)JobID, sqlConn).ExecuteNonQuery();

            for (int i = 0; i < Articles.Count; i++)
            {
                AddArticle(Articles[i], i + 1, sqlConn);
            }

            using (var sqlCommand = new SqlCommand("UPDATE dbo.JOBB SET JOBB_RAD=@artiklar WHERE JOBBNR=@jobbnr", sqlConn))
            {
                sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);
                sqlCommand.Parameters.AddWithValue("@artiklar", Articles.Count + 1);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private void AddArticle(Article article, int articlesCount, SqlConnection sqlConn)
        {
            //int articlesCount = GetArticles().Count;
            string query = "INSERT INTO dbo.DELAR (JOBBNR, RADNR, ARTNR, ANTAL, INPRIS, UTPRIS, RAD_TOTAL_IN, RAD_TOTAL_UT, TEXT, TEKN_LAGER, DATUM, KUNDNR, MNR, MTYP, FAKTURA, ARTGRUPP, KATEGORI, BEST_ANTAL, PRISENHET, LEV_DATUM) VALUES " +
                                                 "(@jobbnr, @radnr, @artnr, @antal, @inpris, @utpris, @inpris, @utpris, @text, @tekn_lager, @datum, @kundnr, @mnr, @mtyp, @faktura, @artgrupp, @kategori, @best_antal, @prisenhet, @datum)";
            using (var sqlCommand = new SqlCommand(query, sqlConn))
            {
                sqlCommand.Parameters.AddWithValue("@jobbnr", JobID);
                sqlCommand.Parameters.AddWithValue("@radnr", articlesCount + 1);
                sqlCommand.Parameters.AddWithValue("@artnr", Article._DefArticleNumber);
                sqlCommand.Parameters.AddWithValue("@antal", (decimal)article.Quantity);
                sqlCommand.Parameters.AddWithValue("@inpris", 0);
                sqlCommand.Parameters.AddWithValue("@utpris", article.ArticlePrice);
                sqlCommand.Parameters.AddWithValue("@text", article.ArticleText); //max30
                sqlCommand.Parameters.AddWithValue("@tekn_lager", "L");
                sqlCommand.Parameters.AddWithValue("@datum", DateTime.Now);
                sqlCommand.Parameters.AddWithValue("@kundnr", CustomerID);  // max 30chars
                sqlCommand.Parameters.AddWithValue("@mnr", SiteID);
                sqlCommand.Parameters.AddWithValue("@mtyp", 1);
                sqlCommand.Parameters.AddWithValue("@faktura", 1);
                sqlCommand.Parameters.AddWithValue("@artgrupp", "Tjä");
                sqlCommand.Parameters.AddWithValue("@best_antal", article.Quantity);
                sqlCommand.Parameters.AddWithValue("@prisenhet", "St");
                sqlCommand.Parameters.AddWithValue("@kategori", "02");

                sqlCommand.ExecuteNonQuery();
            }
        }

    }


    public class JobConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] parts = ((string)value).Split(new char[] { ',' });
                Job job = new Job(decimal.Parse(parts[0]), "", "", parts[1], "", "", "", "", "", false, "", DateTime.Parse(parts[2]), "");
                //room.RoomNumber = Convert.ToInt32(parts[0]);
                //room.Location = parts.Length > 1 ? parts[1] : null;
                return job;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                Job job = value as Job;
                return string.Format("{0},{1},{2}", job.JobID, job.SiteName, job.DateAdded);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
