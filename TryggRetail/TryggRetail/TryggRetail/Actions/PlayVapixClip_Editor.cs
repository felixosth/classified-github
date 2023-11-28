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
    public partial class PlayVapixClip_Editor : CustomActionEditor_UsrControl
    {
        public PlayVapixClip_Editor()
        {
            InitializeComponent();
        }

        public override CustomAction GetCustomAction()
        {
            return new PlayVapixClip_Action(urlTxtBox.Text);
        }

        public override bool InputIsValid()
        {
            return !string.IsNullOrEmpty(urlTxtBox.Text);
        }
    }
}
