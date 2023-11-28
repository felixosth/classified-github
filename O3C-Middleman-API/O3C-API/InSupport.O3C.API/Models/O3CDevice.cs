using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InSupport.O3C.API.Models
{
    /// <summary>
    /// A object that represents a device that is connected to a O3CServer.
    /// </summary>
    public class O3CDevice
    {
        /// <summary>
        /// The MAC/Serialnumber of the device. Primary key.
        /// </summary>
        [Key]
        [Column(TypeName = "nvarchar(30)")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MAC { get; set; }

        /// <summary>
        /// The name of the connected O3CServer.
        /// </summary>
        [Column(TypeName = "nvarchar(50)")]
        public string O3CServer { get; set; }

        /// <summary>
        /// True if the device is connected to the O3CServer.
        /// </summary>
        public bool IsUp { get; set; }

        /// <summary>
        /// Product name of the device., I.E. M3065-V
        /// </summary>
        public string Product { get; set; }
        /// <summary>
        /// Firmware version of the device, I.E. 9.80
        /// </summary>
        public string Firmware { get; set; }
    }
}
