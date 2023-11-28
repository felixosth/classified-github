using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryggRetail.Actions
{
    public partial class CustomActionEditor_UsrControl : UserControl
    {
        public CustomActionEditor_UsrControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public virtual CustomAction GetCustomAction()
        {
            return null;
        }

        public virtual bool InputIsValid()
        {
            return true;
        }
    }
}
