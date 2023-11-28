using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XProtectWebStream.Database.Objects;
using XProtectWebStream.Global;

namespace XProtectWebStream.Database
{
    internal class LocalDatabase
    {
        private SqliteConnection Connection { get; set; }
        private const string DBFileName = "data.db";

        internal AccessDatabaseManagement AccessDatabaseManagement { get; set; }

        internal event EventHandler<string> OnLog;

        internal LocalDatabase()
        {
            bool setupDb = !File.Exists(Path.Combine(Config.WorkDir, DBFileName));

            Connection = GetConnection();

            if(setupDb)
                SetupDb();

            AccessDatabaseManagement = new AccessDatabaseManagement(this);
            AccessDatabaseManagement.OnLog += AccessDatabaseManagement_OnLog;
        }

        private void AccessDatabaseManagement_OnLog(object sender, string e)
        {
            Log("AccessDatabase: " + e);
        }
        
        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }

        private SqliteConnection GetConnection()
        {
            SqliteConnection cnn = new SqliteConnection(
                new SqliteConnectionStringBuilder()
                {
                    DataSource = Path.Combine(Config.WorkDir, DBFileName),
                    Password = GetMyDbPassword()
                }.ToString()
            );
            cnn.Open();
            return cnn;
        }

        private void SetupDb()
        {
            // Log
            Command("CREATE TABLE IF NOT EXISTS webaccess (id INTEGER PRIMARY KEY, datetime TEXT NULL, ip TEXT NULL, sessionId TEXT NULL, token TEXT NULL);");
            Command("CREATE TABLE IF NOT EXISTS createdtokens (id INTEGER PRIMARY KEY, datetime TEXT NULL, user TEXT NULL, token TEXT NULL, camera TEXT NULL, liveorrec TEXT NULL, validLength TEXT NULL, validTo TEXT NULL, exportFrom TEXT NULL, exportTo TEXT NULL);");

            Command("CREATE TABLE IF NOT EXISTS bankidlogins (id INTEGER PRIMARY KEY, datetime TEXT NULL, sessionId TEXT NULL, pnr TEXT NULL);");

            Command("CREATE INDEX IF NOT EXISTS webaccess_token_idx ON webaccess(token);");
            Command("CREATE INDEX IF NOT EXISTS createdtokens_token_idx ON createdtokens(token);");

            // Access groups
            Command("CREATE TABLE IF NOT EXISTS accessgroups (id INTEGER PRIMARY KEY, name TEXT NOT NULL);");
            Command("CREATE TABLE IF NOT EXISTS accessusers (id INTEGER PRIMARY KEY, accessgroupid INTEGER NOT NULL, name TEXT NOT NULL, pnr TEXT NOT NULL);");

            // default anon group
            //Command("INSERT INTO accessgroups (name) VALUES (\"Anonymous\");");
        }

        private void Command(string q)
        {
            using (var command = new SqliteCommand(q, Connection))
                command.ExecuteNonQuery();
        }

        internal SqliteCommand GetCommand()
        {
            return Connection.CreateCommand();
        }

        internal void InsertBankIDLogin(DateTime dateTime, string sessionId, string pnr)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO bankidlogins (datetime, sessionId, pnr) VALUES ($dateTime, $sessionId, $pnr);";
            command.Parameters.AddWithValue("$dateTime", dateTime);
            command.Parameters.AddWithValue("$sessionId", sessionId);
            command.Parameters.AddWithValue("$pnr", pnr);

            ThreadPool.QueueUserWorkItem((state) =>
            {
                command.ExecuteNonQuery();
            });
        }

        internal void InsertCreatedTokens(DateTime dateTime, string user, string token, string cameraId, string liveOrRec, TimeSpan? validLength, DateTime? validTo, DateTime? exportFrom, DateTime? exportTo)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO createdtokens (datetime, user, token, camera, liveorrec, validLength, validTo, exportFrom, exportTo) VALUES ($dateTime, $user, $token, $camera, $liveorrec, $validLength, $validTo, $exportFrom, $exportTo);";
            command.Parameters.AddWithValue("$dateTime", dateTime);
            command.Parameters.AddWithValue("$user", user);
            command.Parameters.AddWithValue("$token", token);
            command.Parameters.AddWithValue("$camera", cameraId);
            command.Parameters.AddWithValue("$liveorrec", liveOrRec);
            command.Parameters.AddWithValue("$validLength", ((object)validLength) ?? DBNull.Value);
            command.Parameters.AddWithValue("$validTo", ((object)validTo) ?? DBNull.Value);
            command.Parameters.AddWithValue("$exportFrom", ((object)exportFrom) ?? DBNull.Value);
            command.Parameters.AddWithValue("$exportTo", ((object)exportTo) ?? DBNull.Value);

            ThreadPool.QueueUserWorkItem((state) =>
            {
                command.ExecuteNonQuery();
            });
        }

        internal void InsertWebAccess(DateTime dateTime, string remoteEndpoint, string sessionId, string token)
        {
            var command = Connection.CreateCommand();
            command.CommandText = "INSERT INTO webaccess (datetime, ip, sessionId, token) VALUES ($dateTime, $ip, $sessionId, $token);";
            command.Parameters.AddWithValue("$dateTime", dateTime);
            command.Parameters.AddWithValue("$ip", remoteEndpoint);
            command.Parameters.AddWithValue("$sessionId", sessionId);
            command.Parameters.AddWithValue("$token", token);

            ThreadPool.QueueUserWorkItem((state) =>
            {
                command.ExecuteNonQuery();
            });
        }

        internal List<Dictionary<string, object>> DynamicSelect(string table)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {ScrubFromMaliciousStatement(table)};";
                command.Parameters.AddWithValue("@table", table);

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                values.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            rows.Add(values);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log($"Error on dynamicselect for {table}: {ex.Message}");
                }
            }
            return rows;

        }



        internal static string GetMyDbPassword()
        {
            byte[] mguid = Encoding.UTF8.GetBytes(Shared.Utils.GetMGUID());
            return System.Convert.ToBase64String(mguid);
        }

        internal void Close()
        {
            Connection.Close();
        }

        private static string ScrubFromMaliciousStatement(string str)
        {
            string final = "";
            foreach(var c in str)
            {
                if (char.IsLetterOrDigit(c))
                    final += c;
            }
            return final;
        }
    }

}
