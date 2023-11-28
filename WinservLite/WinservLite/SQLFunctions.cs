using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinservLite
{
    public static class SQLFunctions
    {

        public static List<Technician> GetTechnicians(string sqlConnectionsString)
        {
            var list = new List<Technician>();

            using (SqlConnection sqlConn = new SqlConnection(sqlConnectionsString))
            {
                sqlConn.Open();

                string query = "SELECT NR, NAMN FROM dbo.TEKNIKER";
                using (var reader = new SqlCommand(query, sqlConn).ExecuteReader())
                {
                    while(reader.Read())
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
    }

    public class Technician
    {
        public string Name { get; set; }
        public string UserName { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
