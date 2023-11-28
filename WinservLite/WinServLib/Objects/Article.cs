using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServLib.Objects
{
    public class Article
    {
        public int RecNum { get; set; }
        public string ArticleText { get; set; }
        public decimal ArticlePrice { get; set; }
        public decimal Quantity { get; set; }

        public const string _DefArticleNumber = "80000";

        public bool Delete()
        {
            int result = -1;
            using (var sqlConn = new SqlConnection(WinServ.SQLConnectionString))
            {
                sqlConn.Open();

                string query = "DELETE FROM dbo.DELAR WHERE RECNUM=@recnum";
                using (var cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.Parameters.Add("@recnum", SqlDbType.BigInt).Value = RecNum;
                    result = cmd.ExecuteNonQuery();
                }
                sqlConn.Close();
            }
            return result > 0;
        }
    }
}
