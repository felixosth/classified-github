using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProtectWebStream.Convert
{
    public class FFMPEG
    {
        string ffmpegPath;

        public string ConvertMethod { get; set; } = "software";

        public string ErrorOutput { get; private set; }
        public string StandardOutput { get; private set; }

        public FFMPEG(string ffmpegPath)
        {
            if (!File.Exists(ffmpegPath))
                throw new FileNotFoundException("FFMPEG not found.");
            this.ffmpegPath = ffmpegPath;
        }

        public string ConvertVideo(string file)
        {
            StandardOutput = "";
            ErrorOutput = "";

            //var tmpFile = Path.GetTempFileName();
            var tmpFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".mp4");
            // crf 0 = lossless, 51 = worst

            /* - preset 
            ultrafast
            superfast
            veryfast
            faster
            fast
            medium – default preset
            slow
            slower
            veryslow */

            // GPU ffmpeg -hwaccel cuda -hwaccel_output_format cuda -i in.mkv -c:v h264_nvenc -preset fast out.mp4

            string defaultLine = null;

            switch (ConvertMethod.ToLower())
            {
                case "nvidia":
                    defaultLine = $"-hwaccel cuda -hwaccel_output_format cuda -y -i \"{file}\" -c:v h264_nvenc -b:v 1M -preset fast -f mp4 \"{tmpFile}\"";
                    break;

                case "quicksync":
                    defaultLine = $"-hwaccel qsv -c:v h264_qsv -y -i \"{file}\" -c:v h264_qsv -preset veryfast -f mp4 \"{tmpFile}\"";

                    break;

                default:
                    defaultLine = $"-y -i \"{file}\" -vcodec libx264 -crf 28 -preset fast -tune fastdecode -f mp4 \"{tmpFile}\"";

                    break;

            }


            var ffmpeg = GetFFMPEG(defaultLine);


            //var ffmpeg = GetFFMPEG($"-y -i \"{file}\" -vcodec libx264 -crf 28 -preset superfast -f mp4 \"{tmpFile}\"");

            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.ErrorDataReceived += Ffmpeg_ErrorDataReceived;
            ffmpeg.OutputDataReceived += Ffmpeg_OutputDataReceived;
            ffmpeg.Start();

            ffmpeg.BeginOutputReadLine();
            ffmpeg.BeginErrorReadLine();

            ffmpeg.WaitForExit();
            return tmpFile;
        }

        private void Ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            ErrorOutput += e.Data + Environment.NewLine;
        }

        private void Ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            StandardOutput += e.Data + Environment.NewLine;

            //throw new NotImplementedException();
        }

        public string ConvertImage(string file, int fps = 3, int time = 1)
        {
            var tmpFile = Path.GetTempFileName();
            //var ffmpeg = GetFFMPEG($"-y -framerate 1 -loop 1 -i \"{file}\" -t 1 -pix_fmt yuv420p -vf scale=-2:trunc(ih/2)*2 -f mp4 \"{tmpFile}\"");
            //var ffmpeg = GetFFMPEG($"-y -framerate {fps} -loop 1 -i \"{file}\" -t {time} -pix_fmt yuv420p -vf scale=-2:trunc(ih/2)*2 -f mp4 \"{tmpFile}\"");
            var ffmpeg = GetFFMPEG($"-y -framerate {fps} -loop 1 -i \"{file}\" -t {time} -vf scale=-2:trunc(ih/2)*2 -f mp4 \"{tmpFile}\"");

            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);
            ffmpeg.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            ffmpeg.Start();

            ffmpeg.BeginOutputReadLine();
            ffmpeg.BeginErrorReadLine();

            ffmpeg.WaitForExit();
            return tmpFile;
        }

        Process GetFFMPEG(string args)
        {
            //Console.WriteLine("Starting FFMPEG with args: " + args);
            Process proc = new Process();
            proc.StartInfo.FileName = ffmpegPath;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            return proc;
        }
    }


    public class FFPROBE
    {
        string ffprobePath;


        public FFPROBE(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("FFPROBE not found.");
            ffprobePath = path;
        }

        public FFPROBEResult Probe(string file)
        {
            var proc = GetFFPROBE($"-v quiet -show_format -print_format json \"{file}\"");

            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();

            return JsonConvert.DeserializeObject<FFPROBEResult>(output);
        }

        private Process GetFFPROBE(string args)
        {
            //Console.WriteLine("Starting FFMPEG with args: " + args);
            Process proc = new Process();
            proc.StartInfo.FileName = ffprobePath;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            return proc;
        }
    }

    // ffprobe -v quiet -show_format -print_format json .\in.mp4
    public class FFPROBEResult
    {
        public Format format { get; set; }
    }

    public class Format
    {
        public string filename { get; set; }
        public int nb_streams { get; set; }
        public int nb_programs { get; set; }
        public string format_name { get; set; }
        public string format_long_name { get; set; }
        public string start_time { get; set; }
        public string duration { get; set; }
        public string size { get; set; }
        public string bit_rate { get; set; }
        public int probe_score { get; set; }
        public Tags tags { get; set; }
    }

    public class Tags
    {
        public string major_brand { get; set; }
        public string minor_version { get; set; }
        public string compatible_brands { get; set; }
        public string encoder { get; set; }
    }

}
