using Newtonsoft.Json;
using SarvAI.Milestone.Verifier.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.Proxy.AlarmClient;

namespace SarvAI.Milestone.Verifier
{
    public class AlarmExporter
    {
        MilestoneConnection milestoneConnection;
        Action<string> logAction;
        private int preAndPostTime;

        public AlarmExporter(Action<string> logAction, bool subscribeToAlarms, int preAndPostTimeMs = 3000)
        {
            this.preAndPostTime = preAndPostTimeMs;
            this.logAction = logAction;
            milestoneConnection = new MilestoneConnection();
            milestoneConnection.OnNewAlarm += MilestoneConnection_OnAlarm;

            milestoneConnection.Login(subscribeToAlarms);
        }

        private void MilestoneConnection_OnAlarm(object sender, Alarm alarm)
        {
            if (alarm.EventHeader.Priority != SarvAIService.AlarmPriority)
                return;

            FQID fqidToExport = null;

            var source = alarm.EventHeader.Source?.FQID;
            if (source != null && source.Kind == Kind.Camera)
                fqidToExport = source;
            else
            {
                fqidToExport = alarm.ReferenceList?.FirstOrDefault(reference => reference.FQID.Kind == Kind.Camera)?.FQID;
            }


            if (fqidToExport == null)
                return;

            //service.Log("Incoming alarm...");

            ThreadPool.QueueUserWorkItem((state) =>
            {
                ProcessAlarm(fqidToExport, alarm, isPostProcess: false);
            });
        }

        public IAlarmClient GetAlarmClient(FQID fqid)
        {
            return milestoneConnection.GetAlarmClient(fqid);
        }

        public UltinousResponse ProcessAlarm(FQID fqidToExport, Alarm alarm, bool isPostProcess = false, bool deleteVideoAfter = true)
        {
            UltinousResponse result = null;
            var alarmClient = GetAlarmClient(alarm.EventHeader.Source.FQID);

            var cameraItem = Configuration.Instance.GetItem(fqidToExport);

            if (cameraItem == null)
                return result;


            if(!isPostProcess)
                alarmClient.UpdateAlarm(alarm.EventHeader.ID, "Washing...", -1, -1, DateTime.UtcNow, "");

                if(!isPostProcess)
                    Thread.Sleep(preAndPostTime);

            var exportedFile = Export(
                cameraItem,
                alarm.EventHeader.Timestamp.AddMilliseconds(-preAndPostTime),
                alarm.EventHeader.Timestamp.AddMilliseconds(preAndPostTime));

            if (exportedFile != null)
            {
                logAction?.Invoke("Exported file to " + exportedFile);
                try
                {
                    result = ProcessExport(exportedFile, fqidToExport, alarm.EventHeader.Timestamp).Result;

                    if (deleteVideoAfter)
                        File.Delete(result.LocalVideoFile);

                    if(!isPostProcess)
                        alarmClient.UpdateAlarm(alarm.EventHeader.ID, "Washing complete. Score: " + result.Score, 11, result.Event ? 1 : 3, DateTime.UtcNow, "");
                    if (result.Event == true && !isPostProcess)
                    {
                        milestoneConnection.SendAlarm(fqidToExport, "Confirmed alarm", 1, alarm.EventHeader.Timestamp);
                    }
                }
                catch (Exception ex)
                {
                    if(!isPostProcess)
                        alarmClient.UpdateAlarm(alarm.EventHeader.ID, "An error occured", 11, 2, DateTime.UtcNow, "");
                    logAction?.Invoke(ex.ToString());
                }

                //var folderPath = Path.Combine("Exports", MakeStringPathValid(alarm.EventHeader.Name));
                //Directory.CreateDirectory(folderPath);
                //var exportPath = Path.Combine(folderPath, MakeStringPathValid(alarm.EventHeader.Timestamp.ToString()));

                //File.Move(exportedFile, exportPath + ".mkv");
                //File.Delete(exportedFile);
            }
            else
            {
                logAction?.Invoke("Export failed");
            }
            return result;
        }


        private async Task<UltinousResponse> ProcessExport(string filePath, FQID camera, DateTime date)
        {   
            var verifyResult = await UploadAndVerify(filePath);
            logAction?.Invoke("Result : " + verifyResult);

            verifyResult.LocalVideoFile = filePath;

            return verifyResult;
        }

        private async Task<UltinousResponse> UploadAndVerify(string file)
        {
            using (var httpClient = new HttpClient() { BaseAddress = new Uri("http://34.141.188.53:6006") })
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StreamContent(fileStream), "file", "file");

                var result = await httpClient.PostAsync("/event", formData);

                var resultStr = await result.Content.ReadAsStringAsync();
                logAction?.Invoke("Result str: " + resultStr);

                return JsonConvert.DeserializeObject<UltinousResponse>(resultStr);
            }
        }



        private string Export(Item camera, DateTime from, DateTime to)
        {
            var tmpFile = Path.GetTempFileName();
            if(File.Exists(tmpFile))
                File.Delete(tmpFile);

            IExporter exporter = new MKVExporter { Filename = tmpFile };
            exporter.Path = tmpFile;
            exporter.Init();

            exporter.CameraList = new List<VideoOS.Platform.Item>() { camera };

            try
            {
                if(exporter.StartExport(from.ToUniversalTime(), to.ToUniversalTime()))
                {
                    while(exporter.Progress < 100)
                    {
                        Thread.Sleep(50);

                        if(exporter.LastError > 0)
                        {
                            logAction?.Invoke("Exporter error: " + exporter.LastErrorString);
                        }
                    }

                    var finalFilename = (exporter as MKVExporter).Filename;
                    exporter.EndExport();
                    exporter = null;
                    return finalFilename;
                }
                else
                {
                    int lastError = exporter.LastError;
                    string lastErrorString = exporter.LastErrorString;
                }
            }
            catch (Exception ex)
            {
                logAction?.Invoke(ex.ToString());
            }
            return null;
        }

        internal void Close()
        {
            milestoneConnection.OnNewAlarm -= MilestoneConnection_OnAlarm;
            milestoneConnection.Close();
        }

        static string MakeStringPathValid(string unsafeString)
        {
            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            string result = unsafeString;
            foreach (var invalidCharacter in invalidCharacters)
            {
                result = result.Replace(invalidCharacter, '_');
            }
            return result;
        }
    }
}
