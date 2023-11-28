using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinServLib.Objects;
using WinServLib.Jobs;

namespace WinServLib
{
    public static class WinServ
    {
        internal static string SQLConnectionString { get; set; }
        public static JobManager JobManager { get; set; }

        //public WinServ(string sqlConnString)
        //{
        //    SQLConnectionString = sqlConnString;
        //    JobManager = new JobManager();
        //}

        public static void Initialize(string sqlConnString)
        {
            SQLConnectionString = sqlConnString;
            JobManager = new JobManager();
        }

        public static List<Technician> GetTechnicians()
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            var list = new List<Technician>();

            using (SqlConnection sqlConn = new SqlConnection(SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT NR, NAMN FROM dbo.TEKNIKER WHERE STATUS=0 AND RECNUM != 1;";
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Technician()
                        {
                            UserName = reader.GetString(0),
                            Name = reader.GetString(1)
                        });
                    }
                    reader.Close();
                }

                sqlConn.Close();
            }
            return list;
        }

        //private static Dictionary<string, string> _cachedJobTypes;
        //public static Dictionary<string, string> JobTypes
        //{
        //    get
        //    {
        //        if (_cachedJobTypes != null)
        //            return _cachedJobTypes;
        //        if (WinServ.SQLConnectionString == null)
        //            throw new InvalidOperationException("WinServ is not initialized.");

        //        var dict = new Dictionary<string, string>();

        //        using (SqlConnection sqlConn = new SqlConnection(SQLConnectionString))
        //        {
        //            sqlConn.Open();

        //            string query = "SELECT NR, NAMN FROM dbo.JOBBTYP";
        //            using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    dict.Add(
        //                        reader.GetString(0),
        //                        reader.GetString(1)
        //                    );
        //                }
        //                reader.Close();
        //            }

        //            sqlConn.Close();
        //        }
        //        _cachedJobTypes = dict;
        //        return dict;
        //    }
        //}

        public static IEnumerable<JobType> GetJobTypes()
        {
            var jobTypes = new List<JobType>();

            using (SqlConnection sqlConn = new SqlConnection(SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT NR, NAMN, ARTIKELNR FROM dbo.JOBBTYP";
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jobTypes.Add(new JobType()
                        {
                            NR = reader.GetString(0),
                            Name = reader.GetString(1),
                            Article = reader.GetString(2)
                        });
                    }
                    reader.Close();
                }

                sqlConn.Close();
            }
            return jobTypes;
        }

        public const string NotSpecifiedJobTimeTypeCode = "0";

        public static IEnumerable<JobTimeType> GetJobTimeTypes()
        {
            var jobTypes = new List<JobTimeType>();

            using (SqlConnection sqlConn = new SqlConnection(SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT RECNUM, CODE, DESCR FROM dbo.[DELCODE]";
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        jobTypes.Add(new JobTimeType()
                        {
                            Recnum = reader.GetInt64(0),
                            Code = reader.GetString(1),
                            Name = reader.GetString(2),
                        });
                    }
                    reader.Close();
                }

                sqlConn.Close();
            }
            return jobTypes;
        }


        private static IEnumerable<Status> _jobStatusCache;
        public static IEnumerable<Status> JobStatus
        {
            get
            {
                if (_jobStatusCache != null)
                    return _jobStatusCache;

                if (WinServ.SQLConnectionString == null)
                    throw new InvalidOperationException("WinServ is not initialized.");

                var statuses = new List<Status>();

                using (SqlConnection sqlConn = new SqlConnection(SQLConnectionString))
                {
                    sqlConn.Open();

                    string query = "SELECT NR, NAMN, COLOR FROM dbo.STATUS WHERE SLUTF<>1 ORDER BY NAMN;";
                    using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statuses.Add(
                                new Status(
                                    nr: reader.GetString(0),
                                    name: reader.GetString(1),
                                    colorStr: reader.GetString(2))
                                );
                        }
                        reader.Close();
                    }

                    sqlConn.Close();
                }

                _jobStatusCache = statuses;
                return statuses;
            }
        }

        public static IEnumerable<Site> GetSites()
        {
            if (SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            List<Site> sites = new List<Site>();

            using (var sqlConn = new SqlConnection(SQLConnectionString))
            {
                sqlConn.Open();

                string query = "SELECT MNR, INSTALL, KUNDNR, MTYP, MODELL, GATUADR, POSTNR, ORT, ERORDER FROM dbo.MASKINER";

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
                        City = reader.GetString(7),
                        RefCode = reader.GetString(8)
                    });
                }
            }
            return sites;
        }

        public static Job AddJob(string user, string displayName, Job job, List<string> description, Site selectedSite)
        {
            if (SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            int NewJobID = 0;

            using (var sqlConn = new SqlConnection(SQLConnectionString))
            {
                sqlConn.Open();

                using (var reader = new SqlCommand("SELECT JOBBNR from dbo.SYSFIL WHERE RECNUM=1", sqlConn).ExecuteReader())  // fetch next jobnumber in queue/count
                {
                    while (reader.Read())
                    {
                        NewJobID = (int)reader.GetDecimal(0) + 1;
                    }
                }

                var jobbQuery = "INSERT into dbo.JOBB " +
                    "(MNR, KUNDNR, STATUS, INDAT, TEKN, REF, TEL, JOBBPRIO, [USER], JOBBNR, JOBBTYP, MTYP, MODELL, INSTALL, REG_AV, EMAIL, PNR, ORT, GATA, BETVILLK, BETVILLK_NAMN, ERORDER) VALUES " +
                    "(@mnr, @kundnr, 'A', '" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000") + "', @tekn, @ref, @tel, '2', @user, @jobbnr, @jobbtyp, @mtyp, @modell, " +
                    "@install, @regav, @email, @pnr, @ort, @gata, 'N', 'N', @erOrder)";

                using (var cmd = new SqlCommand(jobbQuery, sqlConn))
                {
                    cmd.Parameters.AddWithValue("@mnr", selectedSite.SiteID);
                    cmd.Parameters.AddWithValue("@kundnr", selectedSite.CustomerID);
                    //cmd.Parameters.AddWithValue("@indat", DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000"));
                    cmd.Parameters.AddWithValue("@tekn", job.Technician);
                    cmd.Parameters.AddWithValue("@ref", job.RefName);
                    cmd.Parameters.AddWithValue("@tel", job.RefPhoneNumber);
                    cmd.Parameters.AddWithValue("@email", job.RefEmailAddress);
                    cmd.Parameters.AddWithValue("@jobbtyp", job.JobType);
                    cmd.Parameters.AddWithValue("@mtyp", selectedSite.ModelType);
                    cmd.Parameters.AddWithValue("@modell", selectedSite.Model);
                    cmd.Parameters.AddWithValue("@install", selectedSite.Name);
                    cmd.Parameters.AddWithValue("@regav", displayName);
                    cmd.Parameters.AddWithValue("@pnr", selectedSite.PostNR == "" ? "00000" : selectedSite.PostNR);
                    cmd.Parameters.AddWithValue("@ort", selectedSite.City == "" ? "Ej ifyllt" : selectedSite.City);
                    cmd.Parameters.AddWithValue("@gata", selectedSite.Address == "" ? "Ej ifyllt" : selectedSite.Address);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@jobbnr", (double)NewJobID);
                    cmd.Parameters.AddWithValue("@erOrder", selectedSite.RefCode);

                    cmd.ExecuteNonQuery();
                }

                var jobbFtQuery = "INSERT into dbo.JOBBFT (JOBBNR, TEXTRAD, RAD) VALUES";
                using (var cmd = new SqlCommand(jobbFtQuery, sqlConn))  //JOBBFT
                {
                    cmd.Parameters.AddWithValue("@jobbnr", (double)NewJobID);

                    for (int i = 0; i < description.Count; i++)
                    {
                        cmd.CommandText += " (@jobbnr, @textrad" + i + ", @rad" + i + ")";
                        if (i + 1 != description.Count)
                            cmd.CommandText += ",";

                        cmd.Parameters.AddWithValue("@textrad" + i, description[i]);
                        cmd.Parameters.AddWithValue("@rad" + i, i + 1);
                    }

                    cmd.ExecuteNonQuery();
                }

                new SqlCommand("UPDATE dbo.SYSFIL SET JOBBNR=" + (double)(NewJobID) + "WHERE RECNUM=1", sqlConn).ExecuteNonQuery();  // update jobbnr count

                sqlConn.Close();
            }

            return JobManager.GetJob(NewJobID, false);
        }

    }
}
