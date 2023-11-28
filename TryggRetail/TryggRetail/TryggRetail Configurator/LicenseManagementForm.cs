using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryggRetail.Admin;

namespace TryggRetail_Configurator
{
    public partial class LicenseManagementForm : Form
    {
        public LicenseManagementForm()
        {
            InitializeComponent();


            var licPage = new HelpPage(ConfigForm.LicenseID, ConfigForm.TryggRetailPluginID);
            licPage.Init(null);
            this.Controls.Add(licPage);
        }

        private void LicenseManagementForm_Load(object sender, EventArgs e)
        {

        }
    }
}
