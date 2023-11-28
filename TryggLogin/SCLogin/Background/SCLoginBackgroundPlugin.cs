using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoOS.Platform;
using VideoOS.Platform.Background;
using VideoOS.Platform.Messaging;

namespace SCLogin.Background
{
    public class SCLoginBackgroundPlugin : BackgroundPlugin
    {
        public override Guid Id => new Guid("{5F10175B-54CB-4A50-BF2F-B6B0FA36D580}");

        public override string Name => "TryggLogin Client Background Plugin";

        public override List<EnvironmentType> TargetEnvironments => new List<EnvironmentType>() { EnvironmentType.SmartClient };

        public override void Close()
        {
        }

        public override void Init()
        {
            if(!SCLoginDefinition.SuppressRefresh)
                new Thread(Reload).Start();
        }


        void Reload()
        {
            while(ClientControl.Instance.ShownWorkSpace == null)
            {
                Thread.Sleep(50);
            }

            EnvironmentManager.Instance.Log(false, "TryggLogin", "Refreshing config, workspace: " + ClientControl.Instance.ShownWorkSpace?.Name);
            EnvironmentManager.Instance.SendMessage(new Message(MessageId.SmartClient.ReloadConfigurationCommand));
        }
    }
}
