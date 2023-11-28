using System;
using VideoOS.Platform.DriverFramework;

namespace REIDMipDriver
{
    /// <summary>
    /// Container holding all the different managers.
    /// </summary>
    public class REIDMipDriverContainer : Container
    {
        internal Uri Uri { get; set; }

        public new REIDMipDriverConnectionManager ConnectionManager => base.ConnectionManager as REIDMipDriverConnectionManager;
        public new REIDMipDriverStreamManager StreamManager => base.StreamManager as REIDMipDriverStreamManager;

        public REIDMipDriverContainer(DriverDefinition definition)
            : base(definition)
        {
            //definition.DriverInfo.
            base.StreamManager = new REIDMipDriverStreamManager(this);
            base.ConnectionManager = new REIDMipDriverConnectionManager(this);
            base.ConfigurationManager = new REIDMipDriverConfigurationManager(this);
        }
    }
}
