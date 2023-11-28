using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;


namespace UVAPMipDriver
{
    /// <summary>
    /// Class handling connection to one hardware. We try to connect to the Kafka server in this class.
    /// </summary>
    public class UVAPMipDriverConnectionManager : ConnectionManager
    {
        private bool _connected = false;

        internal KafkaConnection KafkaConnection { get; set; } = new KafkaConnection();

        private new UVAPMipDriverContainer Container => base.Container as UVAPMipDriverContainer;

        public UVAPMipDriverConnectionManager(UVAPMipDriverContainer container) : base(container)
        {
        }

        // Builtin method, invoked by Milestone when we try to add hardware with this driver
        public override void Connect(Uri uri, string userName, SecureString password, ICollection<HardwareSetting> hardwareSettings)
        {
            if (_connected)
            {
                return;
            }

            Container.Uri = uri;

            string broker = $"{uri.Host}:{uri.Port}";
                
            KafkaConnection.GroupID = $"{Environment.MachineName}:{userName}";
            
            _connected = KafkaConnection.Connect(broker); // Returns true if we successfully connect to the Kafka server
        }

        public override void Close()
        {
            _connected = false;
        }

        // Overridden boolean, required by the abstract class
        public override bool IsConnected
        {
            get
            {
                return _connected;
            }
        }
    }
}
