using System;
using VideoOS.Platform.DriverFramework;

namespace VanillaUvapDriver
{
    /// <summary>
    /// Container holding all the different managers.
    /// TODO: If your hardware does not support some of the functionality, you can remove the class and the instantiation below.
    /// </summary>
    public class VanillaUvapDriverContainer : Container
    {
        public new VanillaUvapDriverConnectionManager ConnectionManager => base.ConnectionManager as VanillaUvapDriverConnectionManager;
        public new VanillaUvapDriverStreamManager StreamManager => base.StreamManager as VanillaUvapDriverStreamManager;
        internal Uri Uri;

        public VanillaUvapDriverContainer(DriverDefinition definition)
            : base(definition)
        {
            base.StreamManager = new VanillaUvapDriverStreamManager(this);
            base.ConnectionManager = new VanillaUvapDriverConnectionManager(this);
            base.ConfigurationManager = new VanillaUvapDriverConfigurationManager(this);
        }
    }
}
