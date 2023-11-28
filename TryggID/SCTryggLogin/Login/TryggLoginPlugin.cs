using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoOS.Platform.Client;
using VideoOS.Platform;

namespace SCTryggLogin.Login
{
    class TryggLoginPlugin : LoginPlugin
    {
        Guid loginPluginId = new Guid("BF3AED40-4458-4F62-88C5-07931ED9EB39");

        public override Guid Id => loginPluginId;

        public override string Name => "TryggLogin Plugin";

        public override void Init(string surveilanceUsername)
        {
            //var enviroment = EnvironmentManager.Instance.EnvironmentType;

            //int i = 0x2;
        }

        public override void LoginFlowExecute()
        {
            base.LoginFlowExecute();
        }

        public override void Close()
        {
        }

        //public override VideoOS.Platform.Client.LoginUserControl GenerateCustomLoginUserControl() => new LoginUserControl();

        public override Image Icon => Properties.Resources.DummyItem;

        public override bool IncludeInLoginFlow => true;
    }
}
