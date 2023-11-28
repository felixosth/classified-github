namespace TryggDriftLinux
{
    public class WorkerOptions
    {
        public string URL { get; set; }
        public string AddCode { get; set; }
        public string Hostname { get; set; }
        public int SendInterval { get; set; }
        public string[] MonitorServices { get; set; }
        public string[] DockerContainers { get; set; }
    }
}
