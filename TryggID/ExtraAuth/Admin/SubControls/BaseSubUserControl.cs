using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoOS.Platform;

namespace TryggLogin.Admin.SubControls
{
    public partial class BaseSubUserControl : UserControl
    {
        protected AdminUserControl AdminParent { get; set; }

        public BaseSubUserControl(AdminUserControl parent)
        {
            InitializeComponent();
            this.AdminParent = parent;
        }

        public BaseSubUserControl()
        {
            InitializeComponent();
        }

        public virtual void Fill(Item item)
        {
        }

        public virtual void UpdateItem(Item item)
        {

        }

        protected void OnUserChange(object sender, EventArgs e)
        {
            AdminParent.OnUserChange(sender, e);
        }
    }
}
