using System;
using System.Collections.Generic;
using System.Security;
using UvapShared.Driver;
using VideoOS.Platform.DriverFramework;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Definitions;

namespace UVAPMipDriver
{
    /// <summary>
    /// The main entry point for the device driver. Milestone will find this class and create a instance if a device is using this driver.
    /// </summary>
    public class UVAPMipDriverDriverDefinition : DriverDefinition
    {
        /// <summary>
        /// Create session to device, or throw exceptions if not successful
        /// </summary>
        /// <returns>Container representing a device</returns>
        protected override Container CreateContainer(Uri uri, string userName, SecureString password, ICollection<HardwareSetting> hardwareSettings)
        {
            return new UVAPMipDriverContainer(this);
        }

        // Builtin method, invoked by Milestone. Returns basic information about the driver. Constants.Product is in the Shared project.
        protected override DriverInfo CreateDriverInfo()
        {
            return new DriverInfo(Constants.DriverId, "UVAPMipDriver", "UVAPMipDriver group", "1.0", new[] { Constants.Product });
        }

    }
}
