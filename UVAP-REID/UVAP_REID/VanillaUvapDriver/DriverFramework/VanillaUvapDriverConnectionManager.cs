using REIDShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;


namespace VanillaUvapDriver
{
    /// <summary>
    /// Class handling connection to one hardware
    /// TODO: Add methods for making the needed requests to your hardware type, and use these from the other classes
    /// </summary>
    public class VanillaUvapDriverConnectionManager : ConnectionManager
    {
        internal KafkaConnection KafkaConnection { get; set; } = new KafkaConnection();
        private Uri _uri;
        private string _userName;
        private SecureString _password;
        private bool _connected = false;

        public VanillaUvapDriverConnectionManager(VanillaUvapDriverContainer container) : base(container)
        {
        }

        /// <summary>
        /// Implementation of the DFW platform method.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hardwareSettings"></param>
        public override void Connect(Uri uri, string userName, SecureString password, ICollection<HardwareSetting> hardwareSettings)
        {
            if (_connected)
            {
                return;
            }
            _uri = uri;
            _userName = userName;
            _password = password;

            (Container as VanillaUvapDriverContainer).Uri = uri;

            // TODO: Establish connection
            _connected = KafkaConnection.Connect($"{uri.Host}:{uri.Port}");

        }

        /// <summary>
        /// Implementation of the DFW platform method.
        /// </summary>
        public override void Close()
        {
            // TODO: Disconnect/close connection
            _connected = false;
        }

        /// <summary>
        /// Implementation of the DFW platform property.
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return _connected;
            }
        }
    }
}
