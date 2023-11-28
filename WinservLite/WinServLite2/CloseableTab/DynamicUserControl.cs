using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WinServLite2.CloseableTab
{
    public class DynamicUserControl : UserControl
    {
        public event EventHandler CloseRequested;

        public void RequestClose()
        {
            CloseRequested?.Invoke(this, new EventArgs());
           }

        public virtual void OnClosing(out bool cancel) { cancel = false; }
    }
}
