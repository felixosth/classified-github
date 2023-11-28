using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform;
using System.IO;
using VideoOS.Platform.Data;
using System.Threading;
using XProtectWebStream.Convert;
using System.Reflection;
using XProtectWebStream.Global;

namespace XProtectWebStream.Milestone
{
    internal class MilestoneVideoExporter
    {
        internal event EventHandler<string> OnLog;
        Convert.FFMPEG ffmpeg;
        FFPROBE ffprobe;

        internal MilestoneVideoExporter()
        {
            VideoOS.Platform.SDK.Export.Environment.Initialize();
            var ffmpegDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ffmpeg");

            ffmpeg = new FFMPEG(Path.Combine(ffmpegDir, "ffmpeg.exe"));
            ffmpeg.ConvertMethod = Config.Instance.Camera.ConvertMethod;

            ffprobe = new FFPROBE(Path.Combine(ffmpegDir, "ffprobe.exe"));

            //var r = ffprobe.Probe(@"C:\Users\felix.osth\Desktop\ffmpeg\tmp7A39.tmp");
            //r = ffprobe.Probe(@"C:\Users\felix.osth\Desktop\ffmpeg\x264.mp4");
        }

        internal ExportJob GetExportJob(Item camera, DateTime from, DateTime to)
        {
            return new ExportJob(camera, from, to, ffmpeg, ffprobe);
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }
    }

    internal class ExportJob
    {
        internal event EventHandler<string> OnLog;
        internal event EventHandler<ExportProgress> OnReportProgress;

        private IExporter exporter;
        private string tmpFileName;
        internal DateTime From { get; private set; }
        internal DateTime To { get; private set; }
        internal Item Camera { get; private set; }

        private FFMPEG ffmpeg;
        private FFPROBE ffprobe;

        private string vidDir;

        internal ExportJob(Item camera, DateTime from, DateTime to, FFMPEG ffmpeg, FFPROBE ffprobe)
        {
            this.Camera = camera;
            this.From = from;
            this.To = to;

            vidDir = Path.Combine(Config.WorkDir, "Videos");
            Directory.CreateDirectory(vidDir);

            tmpFileName = GetUniqueFilePath(vidDir);
            this.ffmpeg = ffmpeg;
            this.ffprobe = ffprobe;

            exporter = new MKVExporter { Path = vidDir, Filename = tmpFileName };
            exporter.Init();
            exporter.CameraList = new List<Item>() { camera };

        }
        
        private string GetUniqueFilePath(string dir)
        {
            var file = Path.GetRandomFileName();
            return File.Exists(Path.Combine(dir, file)) ? GetUniqueFilePath(dir) : file;
        }

        public string ExportAndConvert()
        {
            var start = DateTime.Now;
            var exportedFile = Export(Camera, From, To);

            if (exportedFile == null)
                return null;

            string log = $"Exported {Camera.Name} to {exportedFile}, operation process time: {(DateTime.Now - start).TotalSeconds} sec";
            OnReportProgress?.Invoke(this, new ExportProgress(100, "Converting..."));

            start = DateTime.Now;
            var convertedFile = ffmpeg.ConvertVideo(exportedFile);

            // verify conversion
            var convertedFileProbe = ffprobe.Probe(convertedFile);

            if(convertedFileProbe.format == null) // Conversion failed
            {
                OnReportProgress?.Invoke(this, new ExportProgress("Conversion failed"));
                log += "\r\nConversion FAILED, ffmpeg output:\r\n" + ffmpeg.ErrorOutput;
                Log(log);
                return null;
            }

            double sizeDif = -1;
            try
            {
                sizeDif = Math.Round(new FileInfo(convertedFile).Length / (double)new FileInfo(exportedFile).Length * 100);
            }
            catch { }

            log += "\r\n" + $"Converted {exportedFile} to {convertedFile}, size dif is {sizeDif}% compared to original, operation process time: {(DateTime.Now - start).TotalSeconds} sec";

            try
            {
                File.Delete(exportedFile);
                log += "\r\nDeleted " + exportedFile;
            }
            catch
            {
                log += "\r\nFAILED to deleted " + exportedFile;
            }
            Log(log);

            return convertedFile;
        }

        private string Export(Item camera, DateTime from, DateTime to)
        {
            try
            {
                if (exporter.StartExport(from.ToUniversalTime(), to.ToUniversalTime()))
                {
                    int lastProgress = 0;
                    while (exporter.Progress < 100)
                    {
                        if (lastProgress != exporter.Progress)
                        {
                            lastProgress = exporter.Progress;
                            OnReportProgress?.Invoke(this, new ExportProgress(exporter.Progress, "Exporting..."));
                        }

                        Thread.Sleep(25);

                        if (exporter.LastError > 0)
                        {
                            OnReportProgress?.Invoke(this, new ExportProgress(exporter.LastErrorString));
                            Log("Exporter error: " + exporter.LastErrorString);
                            break;
                        }
                    }

                    var finalFilename = Path.Combine(exporter.Path, (exporter as MKVExporter).Filename);
                    exporter.EndExport();
                    exporter = null;
                    return finalFilename;
                }
                else
                {
                    OnReportProgress?.Invoke(this, new ExportProgress(exporter.LastErrorString));
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                OnReportProgress?.Invoke(this, new ExportProgress(ex.Message));
            }
            return null;
        }

        private void Log(string msg)
        {
            OnLog?.Invoke(this, msg);
        }
    }

    [Serializable]
    internal class ExportProgress : EventArgs
    {
        internal int Progress { get; set; }
        internal string Message { get; set; }
        internal string Error { get; set; }

        internal ExportProgress(int progress, string msg)
        {
            this.Progress = progress;
            this.Message = msg;
        }
        internal ExportProgress(string error)
        {
            this.Error = error;
        }
    }

    
}
