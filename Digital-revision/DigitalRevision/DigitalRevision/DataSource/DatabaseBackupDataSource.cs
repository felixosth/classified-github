using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace DigitalRevision.DataSource
{
    class DatabaseBackupDataSource : DataSourceBase
    {
        private const string _connectionString = "server=localhost;Integrated Security=True";
        private const string _tempDbExportDirectory = "C:\\temp";

        public override string Name => "Databas backup";

        public override double Version => 1.0;

        public async override Task CollectData(string folderDestination)
        {
            List<string> databaseNames;
            try
            {
                databaseNames = await GetDatabaseListAsync();
            }
            catch (SqlException ex)
            {
                ShowError(ex.Message);
                return;
            }

            List<string> exportedDatabaseFiles = new List<string>();

            Directory.CreateDirectory(_tempDbExportDirectory);

            // Export databases to _tempDbExportDirectory
            for (int i = 0; i < databaseNames.Count; i++)
            {
                this.ProgressPercentage = 100 / databaseNames.Count * (i + 1);
                exportedDatabaseFiles.Add(await BackupDatabaseAsync(databaseNames[i], _tempDbExportDirectory));
            }

            // Move .bak files to folderDestination
            foreach (var db in exportedDatabaseFiles)
            {
                File.Move(db, Path.Combine(folderDestination, Path.GetFileName(db)));
            }
        }

        /// <summary>
        /// BACKUP DATABASE {database} TO DISK='{directory}\{database}.bak'
        /// </summary>
        /// <param name="database">Database name</param>
        /// <param name="directory">Directory to put backup</param>
        /// <returns></returns>
        private static async Task<string> BackupDatabaseAsync(string database, string directory)
        {
            var backupFileName = Path.Combine(directory, $"{database}-{DateTime.Now:yyyy-MM-dd}.bak");

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand($"BACKUP DATABASE {database} TO DISK='{backupFileName}'", connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            return backupFileName;
        }

        /// <summary>
        /// List all user created databases
        /// </summary>
        /// <returns></returns>
        private static async Task<List<string>> GetDatabaseListAsync()
        {
            var list = new List<string>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SELECT db.name AS DBName, physical_name as Location FROM sys.master_files mf INNER JOIN sys.databases db ON db.database_id = mf.database_id WHERE type_desc<>'LOG' AND owner_sid<>0x01;", con))
                {
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;
        }
    }
}
