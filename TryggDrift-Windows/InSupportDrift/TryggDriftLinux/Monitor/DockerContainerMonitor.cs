using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TryggDriftLinux.Monitor
{
    public class DockerContainerMonitor : BaseMonitorLinux
    {
        List<DockerContainer> dockerContainers = new List<DockerContainer>();

        public DockerContainerMonitor(ILogger<TryggDriftWorker> logger, string[] containers) : base(logger)
        {

            foreach (var str in containers)
            {
                dockerContainers.Add(new DockerContainer() { Running = false, Name = str });
            }
        }

        public override string MonitorName => "DockerContainer";

        public override float MonitorVersion => 1;


        public IEnumerable<DockerContainer> Containers
        {
            get
            {
                foreach (var container in dockerContainers)
                {
                    string bashOutput = null;
                    DockerInspect inspect = null;
                    try
                    {
                        bashOutput = ("docker inspect --format='{{json .}}' " + container.Name).Bash();
                        inspect = JsonConvert.DeserializeObject<DockerInspect>(bashOutput);
                        container.Running = inspect.State.Running;
                        container.Status = inspect.State.Status;
                        container.Health = inspect.State.Health?.Status;

                    }
                    catch
                    {
                        Logger.LogError("Unable to get docker output output from {container}. Output: {output}. (sudo usermod -aG docker <user>?)", container.Name, bashOutput);
                        container.Running = false;
                    }
                }
                return dockerContainers;
            }
        }
    }

    public class DockerContainer
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public bool Running { get; set; }
        public string Health { get; set; }
    }


    public class DockerInspect
    {
        public string Id { get; set; }
        public string Created { get; set; }
        public string Path { get; set; }
        public string[] Args { get; set; }
        public State State { get; set; }
        public string Image { get; set; }
        public string ResolvConfPath { get; set; }
        public string HostnamePath { get; set; }
        public string HostsPath { get; set; }
        public string LogPath { get; set; }
        public string Name { get; set; }
        public int RestartCount { get; set; }
        public string Driver { get; set; }
        public string Platform { get; set; }
        public string MountLabel { get; set; }
        public string ProcessLabel { get; set; }
        public string AppArmorProfile { get; set; }
        public object ExecIDs { get; set; }


        public Config Config { get; set; }

    }

    public class State
    {
        public string Status { get; set; }
        public bool Running { get; set; }
        public bool Paused { get; set; }
        public bool Restarting { get; set; }
        public bool OOMKilled { get; set; }
        public bool Dead { get; set; }
        public int Pid { get; set; }
        public int ExitCode { get; set; }
        public string Error { get; set; }
        public string StartedAt { get; set; }
        public string FinishedAt { get; set; }
        public Health Health { get; set; }
    }

    public class Health
    {
        public string Status { get; set; }
        //public int FailingStreak { get; set; }
        //public Log[] Log { get; set; }
    }



    public class Config
    {
        public string Hostname { get; set; }
        public string Domainname { get; set; }
        public string User { get; set; }
        public bool AttachStdin { get; set; }
        public bool AttachStdout { get; set; }
        public bool AttachStderr { get; set; }
        public bool Tty { get; set; }
        public bool OpenStdin { get; set; }
        public bool StdinOnce { get; set; }
        public string[] Env { get; set; }
        public string[] Cmd { get; set; }
        public string Image { get; set; }
        public object Volumes { get; set; }
        public string WorkingDir { get; set; }
        public object Entrypoint { get; set; }
        public object OnBuild { get; set; }
    }





}
