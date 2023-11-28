using AuthenticationSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.Messaging;

namespace TryggLogin.Admin
{
    /// <summary>
    /// This class is created once for all Items of a specific Kind and is responsible for getting and saving configuration for 
    /// all Items of that Kind<br/>
    /// <br/>
    /// This class is also responsible for creating, filling and saving 
    /// UserControl and relevant fields changed by the user in the administrators.<br/>
    /// Normally there will be one ItemManager for each ItemNode.<br/>
    /// If the Items being managed are stored on another server, then this class can utilize
    /// the Init() and Close() methods to setup sessions to the remote server in anticipation 
    /// of calls to GetItems().<br/>
    /// <br/>
    /// <b>EventServer and Items on MAP</b><br/>
    /// This class is also involved in defining ContextMenu on the SmartClient MAP, because the plugin 
    /// executing in the Event Server, are providing the Smart Client MAP ViewItem with 
    /// all relevant plugins and their definitions.<br/>
    /// As the Event Server uses authorization for Items, the ItemNode needs to define the SecurityActions
    /// relevant for the items being managed by the ItemManager. A minimum of two SecurityActions must always be defined
    /// for the Items to be available in the Smart Client.<br/>
    /// <br/>
    /// <b>Item Selection in the Administrators</b><br/>
    /// Selection of an Item can happen via mouse click on the TreeView or via ContentMenu Property selection in Enterprise.<br/>
    /// The user action will result in the following sequences of events:<br/>
    /// a)	ItemManager is accessed through the ItemNode<br/>
    /// b)	The GenerateUserControl() is called to allow ItemManager to create the User Control, setup the ConfigurationChangedByUserEvent, and return the UserControl to the caller.<br/>
    /// c)	The caller will add the UserControl to its relevant panel and set userControl.Dock = DockStyle.Fill to fill available area.<br/>
    /// d)	If executing in the enterprise administrator and the floating property window is opened, the size of the window is adjusted to a relevant size based on the UserControl.Size<br/>
    /// e)	Existing configuration is fetched by calling the ItemManager.GetItem(FQID)<br/>
    /// f)	The ItemManager.FillUserControl(item) method is called for let the ItemManager populate the UserControl.<br/>
    /// <br/>
    /// <br/>
    /// When de-selecting an item, the ItemManager.FillUserControl(null) is called to let the ItemManager clear the UserControl. The parent UserControl will also have Enabled=false; set shortly after this call.<br/>
    /// <br/>
    /// <b>Item edit and update</b><br/>
    /// After an Item is selected or created, the system needs to know if any updates have been done by the administrator.  
    /// Ensure the ConfigurationChangedByUserHandler is called whenever the administrator changes something on the UI.  
    /// This will allow the administration application to enable Save and Apply buttons as appropriate.<br/>
    /// When the administrator presses a Save or Apply button, the:<br/>
    ///     public bool ValidateAndSaveUserEntry()<br/> 
    /// is called to let the ItemManager and the underlying UserControl validate the entered values. If fields are valid the method returns true.
    /// <br/>
    /// <br/>
    /// <b>TreeView name change</b><br/>
    /// On the TreeView itself, it is possible to press F2 and edit the name directly. In this case the updated name is passed to the ItemManager via the 
    /// SetItemName(name) method.<br/>
    /// The ItemManager should save the new name immediately.<br/>
    /// </summary>
    public class TryggLoginItemManager : ItemManager
    {
        private AdminUserControl _userControl;
        private Guid _kind;
        internal static Guid licObjectId = new Guid("{03CD139F-D51C-4BAF-AFC9-F44A3739F045}");
        internal static Guid usrObjectId = new Guid("{B69D46C6-EFAF-403B-B5EA-7FB23865345A}");

        MessageCommunication msgCom;


        #region Constructors

        public TryggLoginItemManager(Guid kind) : base()
        {
            _kind = kind;
        }

        public override void Init()
        {
            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            msgCom = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);

            var items = GetItems();
            if (items.Count == 0)
            {
                var usrItem = new Item(GenerateFQID(usrObjectId), "Users");
                var licItem = new Item(GenerateFQID(licObjectId), "License");

                Configuration.Instance.SaveItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, licItem);
                //usrItem.Properties["users"] = MessagingWrapper.Packer.Serialize(new TryggLogin.Admin.SubControls.UserManagement());
                Configuration.Instance.SaveItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, usrItem);
            }
        }

        FQID GenerateFQID(Guid id) => new FQID(EnvironmentManager.Instance.MasterSite.ServerId, Guid.Empty, id, FolderType.UserDefined, TryggLoginDefinition.TryggLoginKind);

        public override void Close()
        {
        }

        #endregion

        #region UserControl Methods

        /// <summary>
        /// Generate the UserControl for configuring a type of item that this ItemManager manages.
        /// </summary>
        /// <returns></returns>
        public override UserControl GenerateDetailUserControl()
        {
            _userControl = new AdminUserControl();
            _userControl.ConfigurationChangedByUser += new EventHandler(ConfigurationChangedByUserHandler);
            return _userControl;
        }

        //public override List<DetailedUserControl> GenerateDetailUserControlList()
        //{
        //    return new List<DetailedUserControl>()
        //    {
        //        new DetailedUserControl() { Width=100, Height = 100, BackColor = Color.Green, TabDisplayName = "First"},
        //        new DetailedUserControl() { Width= 150, Height = 150, BackColor = Color.Red, TabDisplayName = "Second"}
        //    };
        //}

        /// <summary>
        /// The UserControl is no longer being used, and related resources can be released.
        /// </summary>
        public override void ReleaseUserControl()
        {
            if (_userControl != null)
            {
                _userControl.ConfigurationChangedByUser -= new EventHandler(ConfigurationChangedByUserHandler);
                _userControl = null;
            }
        }

        /// <summary>
        /// Clear all user entries on the UserControl, all visible fields should be blank or default values.
        /// </summary>
        public override void ClearUserControl()
        {
            CurrentItem = null;
            if (_userControl != null)
            {
                _userControl.ClearContent();
            }
        }

        /// <summary>
        /// Fill the UserControl with the content of the Item or the data it represent.
        /// </summary>
        /// <param name="item">The Item to work with</param>
        public override void FillUserControl(Item item)
        {
            CurrentItem = item;

            if (_userControl != null && item != null)
            {
                _userControl.FillContent(item);
            }
        }

        /// <summary>
        /// Create a UserControl to be used for adding a new item.<br/>
        /// Just fill the user control with default content, but do NOT add it to your configuration yet.<br/>
        /// Implementation of this method will determine the flow of calls for creating new Items.<br/>
        /// When implemented, this UserControl is shown to the user, and upon OK pressed the ValidateAddUserControl is called
        /// for the plugin to validate the entry, and a OK response will result in closing the form containing the AddUserControl and 
        /// a call the the Create() method including the AddUserControl.<br/>
        /// If not implementing this method, the Create() method WITHOUT the AddUserControl parameter is called, and a default
        /// Item is then created, stored (where ever this ItemManager stores it configuration), and displayed in the
        /// TreeView.<br/>
        /// Last step is that the new item node is selected and the ItemManager.FillUserControl is called
        /// to fill the large configuration form.
        /// </summary>
        /// <returns></returns>
        public override UserControl GenerateAddUserControl(Item parentItem, FQID suggestedFQID)
        {
            return new TryggLoginAddUserControl();
        }

        public override ItemNodeUserControl GenerateOverviewUserControl() => new HelpPage();

        /// <summary>
        /// Validate the content of the AddUserControl generated by this class's GenerateAddUserControl.
        /// </summary>
        /// <param name="addUserControl"></param>
        /// <returns>True if the content is valid.</returns>
        public override bool ValidateAddUserControl(UserControl addUserControl)
        {
            return true;
        }
        #endregion

        #region Working with currentItem

        /// <summary>
        /// Get the name of the current Item.
        /// </summary>
        /// <returns></returns>
        public override string GetItemName()
        {
            if (_userControl != null)
            {
                return _userControl.DisplayName;
            }
            return "";
        }

        public override void SetItemName(string name)
        {
            if (_userControl != null)
            {
                _userControl.DisplayName = name;
            }
        }

        /// <summary>
        /// Validate the user entry, and return true for OK.<br/>
        /// External configuration should be saved during this call.<br/>
        /// Any errors should be displayed to the user, and the field in 
        /// error should get focus.
        /// </summary>
        /// <returns>Indicates error in user entry.  True is a valid entry</returns>
        public override bool ValidateAndSaveUserControl()
        {
            if (CurrentItem != null)
            {
                if (_userControl != null)
                {
                    //Get user entered fields
                    _userControl.UpdateItem(CurrentItem);
                }

                //In this template we save configuration on the VMS system
                Configuration.Instance.SaveItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, CurrentItem);

                if (CurrentItem.Name == "Users")
                {
                    //var newUserList = SubControls.UserManagement.GetUsers();
                    //msgCom.TransmitMessage(new VideoOS.Platform.Messaging.Message(AuthenticationSystem.AuthSystem.updateUsersFilter,
                    //    MessagingWrapper.Packer.Serialize(new MessagingWrapper.MessageData(newUserList))), null, null, null);
                }
                
            }
            return true;
        }

        #endregion

        #region Configuration Access Methods

        public override Item CreateItem(Item parentItem, FQID suggestedFQID)
        {
            CurrentItem = new Item(suggestedFQID, "Enter a name");
            if (_userControl != null)
            {
                _userControl.FillContent(CurrentItem);
            }
            Configuration.Instance.SaveItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, CurrentItem);
            return CurrentItem;
        }

        public override Item CreateItem(Item parentItem, FQID suggestedFQID, UserControl addUserControl)
        {
            CurrentItem = new Item(suggestedFQID, (addUserControl as TryggLogin.Admin.TryggLoginAddUserControl).ItemName);


            Configuration.Instance.SaveItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, CurrentItem);
            return CurrentItem;
        }

        public override void DeleteItem(Item item)
        {
            if (item != null)
            {
                Configuration.Instance.DeleteItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, item);
            }

        }

        /// <summary>
        /// Returns a list of all Items of this Kind
        /// </summary>
        /// <returns>A list of items.  Allowed to return null if no Items found.</returns>
        public override List<Item> GetItems()
        {
            //All items in this sample are stored with the Video, therefor no ServerIs or parent ids is used.
            List<Item> items = Configuration.Instance.GetItemConfigurations(TryggLoginDefinition.TryggLoginPluginId, null, _kind);
            return items;
        }

        /// <summary>
        /// Returns a list of all Items from a specific server.
        /// </summary>
        /// <param name="parentItem">The parent Items</param>
        /// <returns>A list of items.  Allowed to return null if no Items found.</returns>
        public override List<Item> GetItems(Item parentItem)
        {
            List<Item> items = Configuration.Instance.GetItemConfigurations(TryggLoginDefinition.TryggLoginPluginId, parentItem, _kind);
            return items;
        }

        /// <summary>
        /// Returns the Item defined by the FQID. Will return null if not found.
        /// </summary>
        /// <param name="fqid">Fully Qualified ID of an Item</param>
        /// <returns>An Item</returns>
        public override Item GetItem(FQID fqid)
        {
            Item item = Configuration.Instance.GetItemConfiguration(TryggLoginDefinition.TryggLoginPluginId, _kind, fqid.ObjectId);
            return item;
        }

        #endregion

        #region Messages and Status

        /// <summary>
        /// Return all the Event groups this Kind of Item can deliver to the Event Server and Alarm Definition.
        /// This list is used for configuring alarms.
        /// </summary>
        /// <returns></returns>		
        public override System.Collections.ObjectModel.Collection<VideoOS.Platform.Data.EventGroup> GetKnownEventGroups(System.Globalization.CultureInfo culture)
        {
            return new System.Collections.ObjectModel.Collection<VideoOS.Platform.Data.EventGroup>();
        }

        /// <summary>
        /// Return all the Event Types, within the group this Kind of Item can deliver to the Event Server.
        /// This list is used for configuring alarms.
        /// </summary>
        /// <returns></returns>		
        public override System.Collections.ObjectModel.Collection<VideoOS.Platform.Data.EventType> GetKnownEventTypes(System.Globalization.CultureInfo culture)
        {
            return new System.Collections.ObjectModel.Collection<VideoOS.Platform.Data.EventType>();
        }

        /// <summary>
        /// Return the operational state of a specific Item.
        /// This is used by the Event Server.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override OperationalState GetOperationalState(Item item)
        {
            return OperationalState.Ok;     // Everything is OK for the sepcified Item
        }

        /// <summary>
        /// Just before a context menu is displayed, each line on the context menu is checked for it should be enabled or disabled.
        /// This method is called with the following command (If allowed by the ItemNode definition)<br/>
        ///   "ADD" - for the "Add new ..." <br/>
        ///   "DELETE" - for the "Delete ..."<br/>
        ///   "RENAME" - for rename<br/>
        /// If your plugin has the configuration stored on another server, and management is not possible
        /// via the ItemManager, then this method can be used to disable all contextmenu actions.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public override bool IsContextMenuValid(string command)
        {
            return Debugger.IsAttached;
        }
        #endregion

    }
}

