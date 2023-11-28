using System.Collections.Generic;
using System.Windows.Controls;

namespace InSupport.Drift.Plugins.Wpf
{
    public abstract class BasePluginConfigWpf : UserControl
    {
        public abstract void LoadSettings(Dictionary<string, string> config);
        public abstract bool VerifySettings();
        public abstract Dictionary<string, string> GetSettings();
    }
}
