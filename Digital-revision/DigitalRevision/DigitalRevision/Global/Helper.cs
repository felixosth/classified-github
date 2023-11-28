using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace DigitalRevision.Global
{
    public static class Helper
    {

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            while (Directory.Exists(tempDirectory))
            {
                tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public static string StringToCSVCell(string str)
        {
            if (str.Contains("\""))
            {
                str = str.Replace("\"", "\"\"");
                str = string.Format("\"{0}\"", str);
            }
            else if (str.Contains(",") || str.Contains(Environment.NewLine) || str.Contains(CultureInfo.CurrentCulture.TextInfo.ListSeparator))
            {
                str = string.Format("\"{0}\"", str);
            }
            return str;
        }

        public static void WriteCsvFileFromJson(string jsonContent, string path, Action<int> progressPercentageCallback = null)
        {
            using (var csvString = new StreamWriter(path, false, Encoding.UTF8))
            {
                CsvConfiguration configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    Delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator,
                    Encoding = Encoding.UTF8
                };

                using (var csvWriter = new CsvWriter(csvString, configuration))
                {
                    using (var dt = JsonConvert.DeserializeObject<DataTable>(jsonContent))
                    {
                        foreach (DataColumn column in dt.Columns)
                            csvWriter.WriteField(column.ColumnName);
                        csvWriter.NextRecord();

                        float progressIncrement = 100.0f / dt.Rows.Count;
                        float progress = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            progress += progressIncrement;
                            progressPercentageCallback?.Invoke((int)progress);

                            for (int i = 0; i < dt.Columns.Count; i++)
                                csvWriter.WriteField(StringToCSVCell(row[i].ToString()));

                            csvWriter.NextRecord();
                        }
                    }
                }
            }
        }

        private static string cachedMguid = null;
        public static string GetMGUID()
        {
            if (cachedMguid != null)
                return cachedMguid;

            RegistryKey keyBaseX64 = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey keyBaseX86 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey keyX64 = keyBaseX64.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            RegistryKey keyX86 = keyBaseX86.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", RegistryKeyPermissionCheck.ReadSubTree);
            object resultObjX64 = keyX64.GetValue("MachineGuid", (object)"default");
            object resultObjX86 = keyX86.GetValue("MachineGuid", (object)"default");
            keyX64.Close();
            keyX86.Close();
            keyBaseX64.Close();
            keyBaseX86.Close();
            keyX64.Dispose();
            keyX86.Dispose();
            keyBaseX64.Dispose();
            keyBaseX86.Dispose();
            if (resultObjX64 != null && resultObjX64.ToString() != "default")
            {
                cachedMguid = resultObjX64.ToString();
                return resultObjX64.ToString();
            }
            if (resultObjX86 != null && resultObjX86.ToString() != "default")
            {
                cachedMguid = resultObjX86.ToString();
                return resultObjX86.ToString();
            }

            return "not found";
        }
    }
}
