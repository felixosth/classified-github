using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace EyeTrackerTest.Client
{
    /// <summary>
    /// The ViewItemManager contains the configuration for the ViewItem. <br/>
    /// When the class is initiated it will automatically recreate relevant ViewItem configuration saved in the properties collection from earlier.
    /// Also, when the viewlayout is saved the ViewItemManager will supply current configuration to the SmartClient to be saved on the server.<br/>
    /// This class is only relevant when executing in the Smart Client.
    /// </summary>
    public class EyeTrackerTestViewItemManager : ViewItemManager
    {
        public FQID CameraFQID;
        private bool ptz = false;

        EyeTrackerTestViewItemWpfUserControl viewItem;

        public EyeTrackerTestViewItemManager() : base("EyeTrackerTestViewItemManager")
        {
            viewItem = new EyeTrackerTestViewItemWpfUserControl(this);
        }

        /// <summary>
        /// The properties for this ViewItem is now loaded into the base class and can be accessed via 
        /// GetProperty(key) and SetProperty(key,value) methods
        /// </summary>
        public override void PropertiesLoaded()
        {
            string serializedItem = GetProperty("camFqid");

            var ptzProperty = GetProperty("isPtz");
            if(ptzProperty != null)
                ptz = bool.Parse(ptzProperty);

            if (!string.IsNullOrEmpty(serializedItem))
                CameraFQID = Item.Deserialize(serializedItem).FQID;
        }

        public void SaveCam(Item cam)
        {
            CameraFQID = cam.FQID;
            SetProperty("camFqid", cam.Serialize());
            SaveProperties();

            viewItem.LoadCamera(CameraFQID);
        }

        public bool IsPTZ
        {
            get { return ptz; }
            set
            {
                SetProperty("isPtz", value.ToString());
                SaveProperties();
                ptz = value;
            }
        }

        ///// <summary>
        ///// Generate the UserControl containing the actual ViewItem Content.
        ///// 
        ///// For new plugins it is recommended to use GenerateViewItemWpfUserControl() instead. Only implement this one if support for Smart Clients older than 2017 R3 is needed.
        ///// </summary>
        ///// <returns></returns>
        //public override ViewItemUserControl GenerateViewItemUserControl()
        //{
        //	return new EyeTrackerTestViewItemUserControl(this);
        //}

        /// <summary>
        /// Generate the UserControl containing the actual ViewItem Content.
        /// </summary>
        /// <returns></returns>
        public override ViewItemWpfUserControl GenerateViewItemWpfUserControl()
        {
            return viewItem;
        }

        ///// <summary>
        ///// Generate the UserControl containing the property configuration.
        ///// 
        ///// For new plugins it is recommended to use GeneratePropertiesWpfUserControl() instead. Only implement this one if support for Smart Clients older than 2017 R3 is needed.
        ///// </summary>
        ///// <returns></returns>
        //public override PropertiesUserControl GeneratePropertiesUserControl()
        //{
        //	return new EyeTrackerTestPropertiesUserControl(this);
        //}

        /// <summary>
        /// Generate the UserControl containing the property configuration.
        /// </summary>
        /// <returns></returns>
        public override PropertiesWpfUserControl GeneratePropertiesWpfUserControl()
        {
            return new EyeTrackerTestPropertiesWpfUserControl(this);
        }

    }
}
