using InSupport.Drift.Plugins.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace TryggDRIFT_Configurator
{
    /// <summary>
    /// Interaction logic for PluginConfigWindow.xaml
    /// </summary>
    public partial class PluginConfigWindow : Window
    {
        private readonly PluginLoader loader = new PluginLoader();
        BasePluginConfigWpf plugin;
        private readonly PluginConfig existingConfig;
        private readonly PluginConfigCache pluginConfig;
        private readonly int pluginId;
        private readonly string pluginDir;

        public PluginConfigWindow()
        {
            InitializeComponent();
        }

        public PluginConfigWindow(string dir, int pluginId)
        {
            InitializeComponent();

            pluginDir = dir;
            this.pluginId = pluginId;

            if (File.Exists(PluginConfigCache._FILENAME))
            {
                pluginConfig = JsonConvert.DeserializeObject<PluginConfigCache>(File.ReadAllText(PluginConfigCache._FILENAME));
            }
            else
            {
                pluginConfig = new PluginConfigCache();
            }
            existingConfig = pluginConfig.PluginConfigs.FirstOrDefault(pc => pc.PluginID == pluginId);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (pluginDir != null)
            {
                try
                {
                    plugin = loader.LoadPlugin(pluginDir);
                    if (plugin == null)
                    {
                        MessageBox.Show("No configuration template found!", "Error");

                        if (existingConfig == null)
                        {
                            pluginConfig.PluginConfigs.Add(new PluginConfig()
                            {
                                PluginID = pluginId,
                                Config = new Dictionary<string, string>()
                            });
                            pluginConfig.Save();
                        }

                        Environment.Exit(0);
                    }

                    pluginConfig.PluginConfigs.FirstOrDefault(pc => pc.PluginID == pluginId);
                    if (existingConfig != null)
                        plugin.LoadSettings(existingConfig.Config);
                    pluginConfigGrid.Children.Add(plugin);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetType().ToString());
                    MessageBox.Show("Error while trying to load the plugin configuration.\r\n\r\n" + ex.Message, "Error");
                    Environment.Exit(0);
                }
                this.Activate();
            }
            else
            {
                MessageBox.Show("No plugin configuration found!", "Error");
                Environment.Exit(0);
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (plugin.VerifySettings())
            {
                var configToSave = plugin.GetSettings();

                if (existingConfig != null)
                {
                    existingConfig.Config = configToSave;
                }
                else
                {
                    pluginConfig.PluginConfigs.Add(new PluginConfig()
                    {
                        PluginID = pluginId,
                        Config = configToSave
                    });
                }

                pluginConfig.Save();
                this.Close();
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the configuration?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }

    internal class PluginLoader
    {
        internal BasePluginConfigWpf LoadPlugin(string dir)
        {
            var files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly).Where(file => file.EndsWith(".exe") || file.EndsWith(".dll")).ToArray();
            foreach (var file in files)
            {
                var pluginCfg = GetPluginConfig(file);
                if (pluginCfg != null)
                    return pluginCfg;
            }
            return null;
        }

        internal BasePluginConfigWpf GetPluginConfig(string assemblyPath)
        {
            var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyPath));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BasePluginConfigWpf)))
                {
                    if (Activator.CreateInstance(type) as BasePluginConfigWpf is BasePluginConfigWpf pluginCfg)
                        return pluginCfg;
                }
            }
            return null;
        }

        internal T GetObjectOfParent<T>(string assemblyPath)
        {
            var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyPath));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(T)))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj != null)
                        return (T)obj;
                }
            }
            return default;
        }

        internal bool SubclassExistsInAssembly<T>(string assemblyPath)
        {
            var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(assemblyPath));
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(T)))
                {
                    var obj = Activator.CreateInstance(type);
                    if (obj != null)
                        return true;
                }
            }
            return false;
        }

        //private BasePluginConfigWpf[] LoadAssembly(string assemblyPath)
        //{
        //    var assembly = Assembly.LoadFrom(assemblyPath);

        //    var types = from type in assembly.GetTypes()
        //                where typeof(BasePluginConfigWpf).IsAssignableFrom(type)
        //                select type;


        //    //var allTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BasePluginConfigWpf))).ToArray();

        //    var instances = types.Select(
        //        v => (BasePluginConfigWpf)Activator.CreateInstance(v)).ToArray();

        //    return instances;
        //}
    }
}
