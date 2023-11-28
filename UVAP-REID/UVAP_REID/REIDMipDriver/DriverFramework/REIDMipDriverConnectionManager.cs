using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REIDShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using VideoOS.Platform.DriverFramework.Data.Settings;
using VideoOS.Platform.DriverFramework.Exceptions;
using VideoOS.Platform.DriverFramework.Managers;
using VideoOS.Platform.DriverFramework.Utilities;


namespace REIDMipDriver
{
    /// <summary>
    /// Class handling connection to one hardware
    /// </summary>
    public class REIDMipDriverConnectionManager : ConnectionManager
    {
        internal KafkaConnection KafkaConnection { get; set; } = new KafkaConnection();
        internal NodeREDConnection NodeREDConnection { get; set; }

        private new REIDMipDriverContainer Container => base.Container as REIDMipDriverContainer;

        //private InputPoller _inputPoller;

        //private Uri _uri;
        //private string _userName;
        //private SecureString _password;
        private bool _connected = false;

        public REIDMipDriverConnectionManager(REIDMipDriverContainer container) : base(container)
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

            Container.Uri = uri;

            NodeREDConnection = new NodeREDConnection(uri);

            if (NodeREDConnection.Connect())
            {
                string broker = NodeREDConnection.GetKafkaBroker();
                _connected = KafkaConnection.Connect(broker); // Returns true if we successfully connect to the Kafka server

                if (_connected)
                {
                    NodeREDConnection.StartPoll();
                }
            }
                //_connected = KafkaConnection.Connect($"{uri.Host}:{uri.Port}");

        }


        /// <summary>
        /// Implementation of the DFW platform method.
        /// </summary>
        public override void Close()
        {
            _connected = false;

            if(NodeREDConnection != null)
            {
                NodeREDConnection.Close();
                NodeREDConnection = null;
            }


            KafkaConnection.Close();
            KafkaConnection = null;
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
