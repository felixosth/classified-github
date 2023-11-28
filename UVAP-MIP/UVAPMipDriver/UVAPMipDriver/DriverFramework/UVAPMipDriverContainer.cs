using System;
using VideoOS.Platform.DriverFramework;

namespace UVAPMipDriver
{
    /// <summary>
    /// Container holding all the different managers. This is the second entry point after the driver definition class.
    /// </summary>
    public class UVAPMipDriverContainer : Container
    {
        internal Uri Uri { get; set; }

        public new UVAPMipDriverConnectionManager ConnectionManager => base.ConnectionManager as UVAPMipDriverConnectionManager;
        public new UVAPMipDriverStreamManager StreamManager => base.StreamManager as UVAPMipDriverStreamManager;

        public UVAPMipDriverContainer(DriverDefinition definition)
            : base(definition)
        {
            base.StreamManager = new UVAPMipDriverStreamManager(this);
            base.ConnectionManager = new UVAPMipDriverConnectionManager(this);
            base.ConfigurationManager = new UVAPMipDriverConfigurationManager(this);
        }
    }
}
