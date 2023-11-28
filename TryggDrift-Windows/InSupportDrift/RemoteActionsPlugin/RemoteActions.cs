using InSupport.Drift;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace RemoteActionsPlugin
{
    public class RemoteActions : InSupport.Drift.Plugins.CustomPlugin
    {

        //test
        public List<RemoteAction> CompletedActions { get; set; }

        public RemoteActions(Dictionary<string, string> settings, HostMonitor hostMonitor) : base(settings, hostMonitor)
        {
            CompletedActions = new List<RemoteAction>();
            hostMonitor.OnReport += HostMonitor_OnReport;
        }

        private void HostMonitor_OnReport(object sender, string responseString)
        {
            CompletedActions.Clear();
            JToken r = JsonConvert.DeserializeObject<JToken>(responseString);
            if ((bool)r["success"] == true)
            {
                var actions = r["actions"].ToObject<RemoteAction[]>();
                ManageActions(actions);
            }
        }

        private void ManageActions(RemoteAction[] actions)
        {
            foreach (var action in actions)
            {
                try
                {
                    switch (action.ActionGroup)
                    {
                        case "winservice":

                            switch (action.Action)
                            {
                                case "start":
                                    StartService(action.Parameter);
                                    break;
                                case "stop":
                                    StopService(action.Parameter);
                                    break;
                                case "restart":
                                    RestartService(action.Parameter);
                                    break;
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    action.Error = ex.Message;
                    OnError(ex, action);
                }
                CompletedActions.Add(action);
            }
        }

        private void OnError(Exception ex, RemoteAction action)
        {
            HostMonitor.TriggerOnError("RemoteActions Error:\r\n" + JsonConvert.SerializeObject(action, Formatting.Indented) + "\r\n" + ex.ToString());
        }

        private void StartService(string serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            controller.Refresh();
            if (controller.Status == ServiceControllerStatus.Stopped)
                controller.Start();
        }

        private void RestartService(string serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            controller.Refresh();
            if (controller.Status == ServiceControllerStatus.Running)
                controller.Stop();

            controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 5, 0));

            if (controller.Status == ServiceControllerStatus.Stopped)
                controller.Start();

        }

        private void StopService(string serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            controller.Refresh();
            if (controller.Status == ServiceControllerStatus.Running)
                controller.Stop();
        }

        public override string PluginName => "RemoteActions";

        public override string Version => "0.1";
    }

    public class RemoteAction
    {
        public string Error { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("parameter")]
        public string Parameter { get; set; }
        [JsonProperty("actiongroup")]
        public string ActionGroup { get; set; }
    }
}
