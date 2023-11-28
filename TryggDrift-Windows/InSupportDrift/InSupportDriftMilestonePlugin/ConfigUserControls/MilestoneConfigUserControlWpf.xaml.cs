using InSupport.Drift.Plugins.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using VideoOS.Platform.SDK.Platform;

namespace InSupportDriftMilestonePlugin
{
    /// <summary>
    /// Interaction logic for MilestoneConfigUserControlWpf.xaml
    /// </summary>
    public partial class MilestoneConfigUserControlWpf : BasePluginConfigWpf
    {
        internal const string EncryptKey = "h5ah5ahaha1haha2hahah6ahaha7haha1ha%ha2ha2hh45ah1ahahahah1337ahahahahahaI_Lost_$250_By_Buying_GameStop";

        MilestoneEventConfig eventConfig;

        public MilestoneConfigUserControlWpf()
        {
            InitializeComponent();
            VideoOS.Platform.SDK.Environment.Initialize();
        }

        public override bool VerifySettings()
        {
            if (windowsAuthRadio.IsChecked == true)
            {
                MessageBox.Show("You have chosen Windows authentication, make sure the Windows account that runs the service have permission to login to the Milestone server.");
            }

            return true;
        }

        public override Dictionary<string, string> GetSettings()
        {
            var config = new Dictionary<string, string>()
            {
                { "MilestoneServer", serverTxtBox.Text },
                {"MilestoneNotResThreshold", "3" },
                {"MilestoneAlarmList", sendAlarmsChkBox.IsChecked.ToString() },
                {"MilestoneExcludeSequences", (excludeSequencesChkBox.IsChecked == true).ToString()}

            };

            if (windowsAuthRadio.IsChecked == true)
            {
                config.Add("MilestoneUseCurrent", "True");
            }
            else
            {
                config.Add("MilestoneUsername", StringCipher.Encrypt(basicUsernameTxtBox.Text, EncryptKey));
                config.Add("MilestonePassword", StringCipher.Encrypt(basicPwdBox.Password, EncryptKey));
            }

            if (eventConfig != null)
                config.Add("MilestoneEventConfig", JsonConvert.SerializeObject(eventConfig));

            return config;
        }

        private void TestMilestoneConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri(serverTxtBox.Text);

                CredentialCache cc = VideoOS.Platform.Login.Util.BuildCredentialCache(uri, basicUsernameTxtBox.Text, basicPwdBox.Password, "Basic");
                VideoOS.Platform.SDK.Environment.AddServer(secureOnly: false, uri, cc);

                VideoOS.Platform.SDK.Environment.Login(uri, false);

                bool testResult = VideoOS.Platform.SDK.Environment.IsLoggedIn(uri);

                if (!testResult)
                {
                    MessageBox.Show("Connection failed!", "Failure");
                }
                else
                {
                    MessageBox.Show("Connection established!", "Success");
                }
            }
            catch (InvalidCredentialsMIPException)
            {
                MessageBox.Show("Invalid login credentials", "Error");
            }
            catch (ServerNotFoundMIPException)
            {
                MessageBox.Show("Server not found", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            VideoOS.Platform.SDK.Environment.Logout();
            VideoOS.Platform.SDK.Environment.RemoveAllServers();
        }

        public override void LoadSettings(Dictionary<string, string> config)
        {
            if (config.ContainsKey("MilestoneServer"))
                serverTxtBox.Text = config["MilestoneServer"];

            if (config.ContainsKey("MilestoneUseCurrent"))
                windowsAuthRadio.IsChecked = true;
            else
                basicAuthRadio.IsChecked = true;

            if (config.ContainsKey("MilestoneAlarmList"))
                sendAlarmsChkBox.IsChecked = bool.Parse(config["MilestoneAlarmList"]);

            if (config.ContainsKey("MilestoneUsername"))
                basicUsernameTxtBox.Text = StringCipher.Decrypt(config["MilestoneUsername"], EncryptKey);

            if (config.ContainsKey("MilestonePassword"))
                basicPwdBox.Password = StringCipher.Decrypt(config["MilestonePassword"], EncryptKey);

            if (config.ContainsKey("MilestoneExcludeSequences"))
                excludeSequencesChkBox.IsChecked = bool.Parse(config["MilestoneExcludeSequences"]);

            if (config.ContainsKey("MilestoneEventConfig"))
                eventConfig = JsonConvert.DeserializeObject<MilestoneEventConfig>(config["MilestoneEventConfig"]);
        }

        private void configureEventsBtn_Click(object sender, RoutedEventArgs e)
        {
            var milestoneLoginWindow = new Window()
            {
                Width = 320,
                Height = 320,
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Title = "Milestone login"
            };
            var milestoneLogin = new MilestoneLoginUserControl();
            milestoneLoginWindow.Content = milestoneLogin;
            if (milestoneLoginWindow.ShowDialog() == true)
            {
                // if logged in
                var milestoneEventConfigWindow = new Window()
                {
                    Width = 680,
                    Height = 600,
                    Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Title = "Configure events"
                };
                var milestoneEventConfig = new MilestoneEventPickerUserControl(existingConfig: eventConfig);
                milestoneEventConfigWindow.Content = milestoneEventConfig;
                if (milestoneEventConfigWindow.ShowDialog() == true)
                {
                    eventConfig = milestoneEventConfig.EventConfig;
                }
            }
        }

        private void configureMQTTBtn_Click(object sender, RoutedEventArgs e)
        {

            var configMQTTUserControl = new ConfigUserControls.ConfigureMQTTUserControl();
            var configureMQTTWindow = new Window()
            {
                Width = 900,
                Height = 700,
                Content = configMQTTUserControl,
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Title = "Configure MQTT"
            };

            if(configureMQTTWindow.ShowDialog() == true)
            {

            }

        }
    }
}
