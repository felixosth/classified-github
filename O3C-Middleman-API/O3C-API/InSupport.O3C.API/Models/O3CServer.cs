using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API.Models
{
    /// <summary>
    /// A object that represents a O3C server instance.
    /// </summary>
    public class O3CServer
    {
        /// <summary>
        /// The name of the server. Primary key.
        /// </summary>
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Name { get; set; }

        /// <summary>
        /// The external hostname/address of the server. Will replace Host if not null.
        /// </summary>
        public string ExternalHost { get; set; }

        /// <summary>
        /// The hostname/address of the server.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// The admin port of the server, default 3128.
        /// </summary>
        public int AdminPort { get; set; }
        /// <summary>
        /// The client port of the server, default 8080.
        /// </summary>
        public int ClientPort { get; set; }
        /// <summary>
        /// True if the server is online.
        /// </summary>
        public bool IsUp { get; set; }

        /// <summary>
        /// Will redirect devices to/from this server if it exceeds the device limit.
        /// </summary>
        public bool ApplyLoadBalancing { get; set; } = true;

        /// <summary>
        /// The cluster group this server belongs to.
        /// </summary>
        public int ClusterId { get; set; } = 1;
    }
}
