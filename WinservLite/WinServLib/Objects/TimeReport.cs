using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    [Serializable]
    public class TimeReport
    {
        public const int CommentCharCap = 44;

        public string DelayCode { get; set; }

        public string Technician { get; set; }
        public string Comment { get; set; }
        public int JobID { get; set; }
        public bool ArchivedJob { get; set; }
        public bool Traktamente { get; set; }

        public DateTime Date { get; set; }
        public DateTime DateOnly => Date.Date;
        public float WorkTime { get; set; }
        public float TravelTime { get; set; }

        public string SiteName { get; set; }

        public int UniqueID { get; set; }

        public string JobType { get; set; }
        public string JobTimeTypeName { get; set; }

        public bool SetJobID(int newJobId)
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "UPDATE dbo.TEKNJOBB SET JOBBNR=@jobbnr WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.Parameters.AddWithValue("@jobbnr", newJobId);
                    cmd.Parameters.AddWithValue("@recnum", UniqueID);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        JobID = newJobId;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Save()
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                //string query = "UPDATE dbo.TEKNJOBB SET JOBBNR=@jobbnr, TEKN=@tekn, JOBBTYP=@jobbtyp, PLANDAT=@plandat, DELAYCODE=@delaycode, STARTDAT=@plandat, " +
                string query = "UPDATE dbo.TEKNJOBB SET TEKN=@tekn, JOBBTYP=@jobbtyp, PLANDAT=@plandat, DELAYCODE=@delaycode, STARTDAT=@plandat, " +
                "SLUTDAT=@plandat, ARBTID=@arbtid, RESTID=@restid, KOMMENTAR=@kommentar, TRAKTAMENTE=@trakt WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    //cmd.Parameters.Add("@jobbnr", SqlDbType.Decimal).Value = JobID;
                    cmd.Parameters.Add("@tekn", SqlDbType.NVarChar).Value = Technician;
                    cmd.Parameters.Add("@plandat", SqlDbType.DateTime).Value = Date.ToString();
                    cmd.Parameters.Add("@delaycode", SqlDbType.NVarChar).Value = DelayCode;
                    cmd.Parameters.Add("@jobbtyp", SqlDbType.NVarChar).Value = JobType;
                    cmd.Parameters.Add("@arbtid", SqlDbType.Decimal).Value = WorkTime;
                    cmd.Parameters.Add("@restid", SqlDbType.Decimal).Value = TravelTime;
                    cmd.Parameters.Add("@kommentar", SqlDbType.NVarChar).Value = Comment;
                    cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = UniqueID;
                    cmd.Parameters.Add("@trakt", SqlDbType.TinyInt).Value = Traktamente;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool Delete()
        {
            if (WinServ.SQLConnectionString == null)
                throw new InvalidOperationException("WinServ is not initialized.");

            int result = -1;
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "DELETE FROM dbo.TEKNJOBB WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = this.UniqueID;
                    result = cmd.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
            return result > 0;
        }
    }
}
