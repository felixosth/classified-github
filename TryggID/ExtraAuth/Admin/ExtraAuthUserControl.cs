using System;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;

namespace TryggLogin.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class TryggLoginUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;

        private string itemName;
        internal string DisplayName => itemName;

        public TryggLoginUserControl()
        {
            InitializeComponent();
        }

        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        internal void FillContent(Item item)
        {
            itemName = item.Name;
        }

        internal void UpdateItem(Item item)
        {
        }

        internal void ClearContent()
        {
        }

    }
}
