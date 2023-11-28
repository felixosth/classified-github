using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoOS.Platform.Client;

namespace InSupport.Client
{
    public class InSupportWorkSpaceViewItemManager : ViewItemManager
    {
        public InSupportWorkSpaceViewItemManager() : base("InSupportWorkSpaceViewItemManager")
        {
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new InSupportWorkSpaceViewItemUserControl();
        }

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
